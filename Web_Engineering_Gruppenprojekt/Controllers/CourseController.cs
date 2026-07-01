using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Engineering_Gruppenprojekt.Data;
using Web_Engineering_Gruppenprojekt.Models;

namespace Web_Engineering_Gruppenprojekt.Controllers;

public class CourseController(AppDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var courses = await db.Courses
            .OrderBy(c => c.Title)
            .Select(c => new
            {
                Course = c,
                ChapterCount = c.Chapters.Count,
                ExamCount = c.Exams.Count
            })
            .ToListAsync();

        ViewBag.ChapterCounts = courses.ToDictionary(x => x.Course.Id, x => x.ChapterCount);
        ViewBag.ExamCounts = courses.ToDictionary(x => x.Course.Id, x => x.ExamCount);
        return View(courses.Select(x => x.Course).ToList());
    }

    public IActionResult Create() => View();

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Course course)
    {
        if (!ModelState.IsValid) return View(course);
        db.Courses.Add(course);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var course = await db.Courses.FindAsync(id);
        if (course == null) return NotFound();
        return View(course);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Course course)
    {
        if (id != course.Id) return BadRequest();
        if (!ModelState.IsValid) return View(course);

        db.Update(course);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var course = await db.Courses
            .Include(c => c.Chapters)
            .Include(c => c.Exams)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (course == null) return NotFound();
        ViewBag.QuestionCount = await db.Questions.CountAsync(q => q.Chapter.CourseId == id);
        return View(course);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var course = await db.Courses.FindAsync(id);
        if (course != null)
        {
            var examQuestions = await db.ExamQuestions
                .Where(eq => eq.Question.Chapter.CourseId == id)
                .ToListAsync();
            db.ExamQuestions.RemoveRange(examQuestions);

            db.Courses.Remove(course);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
