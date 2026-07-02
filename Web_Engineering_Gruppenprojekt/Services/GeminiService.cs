using System.Text;
using System.Text.Json;
using System.Net;
using Microsoft.EntityFrameworkCore;
using Web_Engineering_Gruppenprojekt.Data;
using Web_Engineering_Gruppenprojekt.Models;

namespace Web_Engineering_Gruppenprojekt.Services;

public class GeminiRateLimitException : Exception
{
    public const string UserMessage = "Limit der Gemini API erreicht. Tageslimits werden voraussichtlich gegen 09:15 Uhr deutscher Zeit zurückgesetzt. Bitte später erneut versuchen.";

    public GeminiRateLimitException() : base(UserMessage)
    {
    }
}

public class GeminiService(IConfiguration config, AppDbContext db, IFileStorageService fileStorage, HttpClient http) : IGeminiService
{
    private const string BaseUrl = "https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent";
    private static readonly string[] RateLimitErrorTerms =
    [
        "429",
        "RESOURCE_EXHAUSTED",
        "quota",
        "quota exceeded",
        "rate limit",
        "rate_limit",
        "too many requests",
        "limit exceeded"
    ];

    public async Task<MCQuestion?> GenerateQuestionAsync(string pdfPath, int chapterId)
    {
        var apiKey = config["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey)) return null;

        var parts = new List<object>();

        if (!string.IsNullOrEmpty(pdfPath))
        {
            var pdfBytes = await fileStorage.DownloadAsync(pdfPath);
            if (pdfBytes != null)
            {
                parts.Add(new
                {
                    inline_data = new
                    {
                        mime_type = "application/pdf",
                        data = Convert.ToBase64String(pdfBytes)
                    }
                });
            }
        }

        var existingQuestions = await db.Questions
            .Where(q => q.ChapterId == chapterId)
            .Select(q => q.QuestionText)
            .ToListAsync();

        var existingQuestionsText = existingQuestions.Count > 0
            ? string.Join("\n", existingQuestions.Select(q => $"- {q}"))
            : "(Keine vorhandenen Fragen zu diesem Kapitel.)";

        parts.Add(new
        {
            text = """
                   Erstelle eine Multiple-Choice-Frage basierend auf dem Inhalt des bereitgestellten PDFs.

                   Zu diesem Kapitel existieren bereits folgende Fragen:
                   {{existingQuestions}}

                   Wähle für die neue Frage explizit ein anderes Thema bzw. einen anderen Aspekt des Inhalts
                   und wiederhole keine der oben aufgeführten Fragen, auch nicht sinngemäß oder umformuliert.

                   Antworte ausschließlich im folgenden JSON-Format, ohne weitere Erklärungen:
                   {
                     "question": "Die Frage",
                     "options": ["Option A", "Option B", "Option C", "Option D"],
                     "correct": 0
                   }
                   "correct" ist der 0-basierte Index der richtigen Antwort.
                   """.Replace("{{existingQuestions}}", existingQuestionsText)
        });

        var requestBody = new
        {
            contents = new[] { new { parts = parts.ToArray() } },
            generationConfig = new { temperature = 0.3 }
        };
        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}?key={apiKey}");
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(request);
        }
        catch (HttpRequestException ex) when (IsRateLimitError(ex))
        {
            throw new GeminiRateLimitException();
        }
        catch
        {
            return null;
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await ReadResponseBodyAsync(response);
            if (IsRateLimitError(response, errorBody))
                throw new GeminiRateLimitException();

            return null;
        }

        var json = await response.Content.ReadAsStringAsync();
        var rawText = ExtractText(json);

        var start = rawText.IndexOf('{');
        var end = rawText.LastIndexOf('}');
        if (start < 0 || end < 0) return null;

        try
        {
            using var innerDoc = JsonDocument.Parse(rawText[start..(end + 1)]);
            var root = innerDoc.RootElement;

            var questionText = root.GetProperty("question").GetString() ?? "";
            var options = root.GetProperty("options").EnumerateArray().Select(o => o.GetString() ?? "").ToList();
            var correct = root.GetProperty("correct").GetInt32();

            if (options.Count < 4 || string.IsNullOrWhiteSpace(questionText)) return null;

            var question = new MCQuestion
            {
                QuestionText = questionText,
                ChapterId = chapterId,
                Options = options.Select((text, i) => new MCAnswerOption
                {
                    AnswerText = text,
                    IsCorrect = i == correct
                }).ToList()
            };

            db.Questions.Add(question);
            await db.SaveChangesAsync();
            return question;
        }
        catch
        {
            return null;
        }
    }

    public async Task<string> CheckQuestionAsync(int questionId)
    {
        var apiKey = config["Gemini:ApiKey"];
        if (string.IsNullOrEmpty(apiKey)) return "Kein API-Schlüssel konfiguriert.";

        var question = await db.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == questionId);
        if (question == null) return "Frage nicht gefunden.";

        var optionsText = string.Join("\n", question.Options.Select((o, i) =>
            $"{(char)('A' + i)}) {o.AnswerText}{(o.IsCorrect ? " [RICHTIG]" : "")}"));

        var prompt = $"""
                      Bitte prüfe die folgende Multiple-Choice-Frage auf sprachliche Qualität und Eindeutigkeit.
                      Gib eine kurze Bewertung (2-4 Sätze) auf Deutsch.

                      Frage: {question.QuestionText}

                      Antwortoptionen:
                      {optionsText}
                      """;

        var requestBody = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { temperature = 0.0 }
        };
        var request = new HttpRequestMessage(HttpMethod.Post, $"{BaseUrl}?key={apiKey}");
        request.Content = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

        HttpResponseMessage response;
        try
        {
            response = await http.SendAsync(request);
        }
        catch (HttpRequestException ex) when (IsRateLimitError(ex))
        {
            return GeminiRateLimitException.UserMessage;
        }
        catch
        {
            return "Fehler bei der KI-Anfrage.";
        }

        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await ReadResponseBodyAsync(response);
            if (IsRateLimitError(response, errorBody))
                return GeminiRateLimitException.UserMessage;

            return "Fehler bei der KI-Anfrage.";
        }

        return ExtractText(await response.Content.ReadAsStringAsync());
    }

    private static async Task<string> ReadResponseBodyAsync(HttpResponseMessage response)
    {
        try
        {
            return await response.Content.ReadAsStringAsync();
        }
        catch
        {
            return "";
        }
    }

    private static bool IsRateLimitError(HttpResponseMessage response, string responseBody)
    {
        return response.StatusCode == HttpStatusCode.TooManyRequests || IsRateLimitError(responseBody);
    }

    private static bool IsRateLimitError(HttpRequestException exception)
    {
        return exception.StatusCode == HttpStatusCode.TooManyRequests || IsRateLimitError(exception.Message);
    }

    private static bool IsRateLimitError(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        return RateLimitErrorTerms.Any(term => text.Contains(term, StringComparison.OrdinalIgnoreCase));
    }

    private static string ExtractText(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement
                .GetProperty("candidates")[0]
                .GetProperty("content")
                .GetProperty("parts")[0]
                .GetProperty("text")
                .GetString() ?? "Keine Antwort erhalten.";
        }
        catch
        {
            return "Fehler beim Verarbeiten der KI-Antwort.";
        }
    }
}
