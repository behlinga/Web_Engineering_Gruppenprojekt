using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Engineering_Gruppenprojekt.Data;
using Web_Engineering_Gruppenprojekt.Models;
using Web_Engineering_Gruppenprojekt.Models.ViewModels;

namespace Web_Engineering_Gruppenprojekt.Controllers;

public class ExamController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index(int courseId)
    {
        var course = await db.Courses.FindAsync(courseId);
        if (course == null) return NotFound();

        var exams = await db.Exams
            .Where(e => e.CourseId == courseId)
            .OrderByDescending(e => e.Date)
            .Select(e => new { Exam = e, QuestionCount = e.ExamQuestions.Count })
            .ToListAsync();

        ViewBag.Course = course;
        ViewBag.QuestionCounts = exams.ToDictionary(x => x.Exam.Id, x => x.QuestionCount);
        return View(exams.Select(x => x.Exam).ToList());
    }

    public async Task<IActionResult> Create(int courseId)
    {
        var course = await db.Courses.FindAsync(courseId);
        if (course == null) return NotFound();

        var totalQuestions = await db.Questions
            .Where(q => q.Chapter.CourseId == courseId)
            .CountAsync();

        var vm = new ExamCreateViewModel
        {
            CourseId = courseId,
            CourseTitle = course.Title,
            NumberOfQuestions = Math.Min(10, totalQuestions),
            Date = DateTime.Today
        };

        ViewBag.TotalQuestions = totalQuestions;
        return View(vm);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamCreateViewModel vm)
    {
        var totalQuestions = await db.Questions
            .Where(q => q.Chapter.CourseId == vm.CourseId)
            .CountAsync();

        if (vm.NumberOfQuestions > totalQuestions)
            ModelState.AddModelError(nameof(vm.NumberOfQuestions),
                $"Nur {totalQuestions} Fragen verfügbar.");

        if (!ModelState.IsValid)
        {
            vm.CourseTitle = (await db.Courses.FindAsync(vm.CourseId))?.Title ?? "";
            ViewBag.TotalQuestions = totalQuestions;
            return View(vm);
        }

        var allQuestions = await db.Questions
            .Where(q => q.Chapter.CourseId == vm.CourseId)
            .Select(q => q.Id)
            .ToListAsync();

        var selected = allQuestions
            .OrderBy(_ => Guid.NewGuid())
            .Take(vm.NumberOfQuestions)
            .ToList();

        var exam = new Exam
        {
            Date = vm.Date,
            CourseId = vm.CourseId,
            ExamQuestions = selected.Select(qId => new ExamQuestion { QuestionId = qId }).ToList()
        };

        db.Exams.Add(exam);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { courseId = vm.CourseId });
    }

    public async Task<IActionResult> Print(int id)
    {
        var exam = await db.Exams
            .Include(e => e.Course)
            .Include(e => e.ExamQuestions)
                .ThenInclude(eq => eq.Question)
                    .ThenInclude(q => q.Options)
            .FirstOrDefaultAsync(e => e.Id == id);

        if (exam == null) return NotFound();
        return View(exam);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var exam = await db.Exams.FindAsync(id);
        if (exam != null)
        {
            var courseId = exam.CourseId;
            db.Exams.Remove(exam);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { courseId });
        }
        return NotFound();
    }
}
