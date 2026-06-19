using Web_Engineering_Gruppenprojekt.Models;

namespace Web_Engineering_Gruppenprojekt.Services;

public interface IGeminiService
{
    Task<MCQuestion?> GenerateQuestionAsync(string pdfPath, int chapterId);
    Task<string> CheckQuestionAsync(int questionId);
}
