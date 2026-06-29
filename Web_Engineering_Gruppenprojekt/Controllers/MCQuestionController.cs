using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Engineering_Gruppenprojekt.Data;
using Web_Engineering_Gruppenprojekt.Models;
using Web_Engineering_Gruppenprojekt.Models.ViewModels;
using Web_Engineering_Gruppenprojekt.Services;

namespace Web_Engineering_Gruppenprojekt.Controllers;

public class MCQuestionController(AppDbContext db, IGeminiService gemini) : Controller
{
    public async Task<IActionResult> Index(int chapterId)
    {
        var chapter = await db.Chapters.Include(c => c.Course).FirstOrDefaultAsync(c => c.Id == chapterId);
        if (chapter == null) return NotFound();

        var questions = await db.Questions
            .Include(q => q.Options)
            .Where(q => q.ChapterId == chapterId)
            .ToListAsync();

        ViewBag.Chapter = chapter;
        return View(questions);
    }

    public async Task<IActionResult> Create(int chapterId)
    {
        var chapter = await db.Chapters.Include(c => c.Course).FirstOrDefaultAsync(c => c.Id == chapterId);
        if (chapter == null) return NotFound();

        ViewBag.Chapter = chapter;
        return View(new QuestionEditViewModel { ChapterId = chapterId, CorrectOption = 1 });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(QuestionEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Chapter = await db.Chapters.Include(c => c.Course).FirstOrDefaultAsync(c => c.Id == vm.ChapterId);
            return View(vm);
        }

        var question = new MCQuestion
        {
            QuestionText = vm.QuestionText,
            ChapterId = vm.ChapterId,
            Options =
            [
                new MCAnswerOption { AnswerText = vm.Option1, IsCorrect = vm.CorrectOption == 1 },
                new MCAnswerOption { AnswerText = vm.Option2, IsCorrect = vm.CorrectOption == 2 },
                new MCAnswerOption { AnswerText = vm.Option3, IsCorrect = vm.CorrectOption == 3 },
                new MCAnswerOption { AnswerText = vm.Option4, IsCorrect = vm.CorrectOption == 4 }
            ]
        };

        db.Questions.Add(question);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { chapterId = vm.ChapterId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var question = await db.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == id);
        if (question == null) return NotFound();

        var options = question.Options.ToList();
        while (options.Count < 4)
            options.Add(new MCAnswerOption { AnswerText = "" });

        var vm = new QuestionEditViewModel
        {
            Id = question.Id,
            QuestionText = question.QuestionText,
            ChapterId = question.ChapterId,
            Option1 = options.ElementAtOrDefault(0)?.AnswerText ?? "",
            Option2 = options.ElementAtOrDefault(1)?.AnswerText ?? "",
            Option3 = options.ElementAtOrDefault(2)?.AnswerText ?? "",
            Option4 = options.ElementAtOrDefault(3)?.AnswerText ?? "",
            CorrectOption = options.FindIndex(o => o.IsCorrect) + 1 is > 0 and var idx ? idx : 1
        };

        ViewBag.Chapter = await db.Chapters.Include(c => c.Course).FirstOrDefaultAsync(c => c.Id == question.ChapterId);
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(QuestionEditViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Chapter = await db.Chapters.Include(c => c.Course).FirstOrDefaultAsync(c => c.Id == vm.ChapterId);
            return View(vm);
        }

        var question = await db.Questions.Include(q => q.Options).FirstOrDefaultAsync(q => q.Id == vm.Id);
        if (question == null) return NotFound();

        question.QuestionText = vm.QuestionText;
        db.AnswerOptions.RemoveRange(question.Options);

        var newOptions = new[]
        {
            new MCAnswerOption { AnswerText = vm.Option1, IsCorrect = vm.CorrectOption == 1, QuestionId = vm.Id },
            new MCAnswerOption { AnswerText = vm.Option2, IsCorrect = vm.CorrectOption == 2, QuestionId = vm.Id },
            new MCAnswerOption { AnswerText = vm.Option3, IsCorrect = vm.CorrectOption == 3, QuestionId = vm.Id },
            new MCAnswerOption { AnswerText = vm.Option4, IsCorrect = vm.CorrectOption == 4, QuestionId = vm.Id }
        };

        db.AnswerOptions.AddRange(newOptions);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { chapterId = vm.ChapterId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var question = await db.Questions
            .Include(q => q.ExamQuestions)
            .FirstOrDefaultAsync(q => q.Id == id);
        if (question != null)
        {
            var chapterId = question.ChapterId;
            db.ExamQuestions.RemoveRange(question.ExamQuestions);
            db.Questions.Remove(question);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { chapterId });
        }
        return NotFound();
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckQuestion(int id)
    {
        var feedback = await gemini.CheckQuestionAsync(id);
        return Json(new { feedback });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> GenerateQuestion(int chapterId)
    {
        var chapter = await db.Chapters.FindAsync(chapterId);
        if (chapter == null) return NotFound();

        var question = await gemini.GenerateQuestionAsync(chapter.SlidePath ?? "", chapterId);
        if (question == null)
            TempData["Error"] = "Frage konnte nicht generiert werden. Bitte API-Schlüssel prüfen.";
        else
            TempData["Success"] = "KI-Frage erfolgreich generiert.";

        return RedirectToAction(nameof(Index), new { chapterId });
    }
}
