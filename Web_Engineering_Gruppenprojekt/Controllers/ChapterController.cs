using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Web_Engineering_Gruppenprojekt.Data;
using Web_Engineering_Gruppenprojekt.Models;
using Web_Engineering_Gruppenprojekt.Services;

namespace Web_Engineering_Gruppenprojekt.Controllers;

public class ChapterController(AppDbContext db, IFileStorageService fileStorage) : Controller
{
    public async Task<IActionResult> Index(int courseId)
    {
        var course = await db.Courses.FindAsync(courseId);
        if (course == null) return NotFound();

        var chapters = await db.Chapters
            .Where(c => c.CourseId == courseId)
            .OrderBy(c => c.ChapterNumber)
            .Select(c => new { Chapter = c, QuestionCount = c.Questions.Count })
            .ToListAsync();

        ViewBag.Course = course;
        ViewBag.QuestionCounts = chapters.ToDictionary(x => x.Chapter.Id, x => x.QuestionCount);
        return View(chapters.Select(x => x.Chapter).ToList());
    }

    public async Task<IActionResult> Create(int courseId)
    {
        var course = await db.Courses.FindAsync(courseId);
        if (course == null) return NotFound();
        ViewBag.Course = course;
        return View(new Chapter { CourseId = courseId });
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Chapter chapter, IFormFile? slideFile)
    {
        ModelState.Remove(nameof(Chapter.Course));
        if (slideFile != null && slideFile.Length > 0)
        {
            var (fileName, path) = await fileStorage.UploadAsync(slideFile);
            chapter.SlideFileName = slideFile.FileName;
            chapter.SlidePath = path;
        }

        if (!ModelState.IsValid)
        {
            ViewBag.Course = await db.Courses.FindAsync(chapter.CourseId);
            return View(chapter);
        }

        db.Chapters.Add(chapter);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { courseId = chapter.CourseId });
    }

    public async Task<IActionResult> Edit(int id)
    {
        var chapter = await db.Chapters.Include(c => c.Course).FirstOrDefaultAsync(c => c.Id == id);
        if (chapter == null) return NotFound();
        ViewBag.Course = chapter.Course;
        return View(chapter);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Chapter chapter, IFormFile? slideFile, bool deleteFile = false)
    {
        ModelState.Remove(nameof(Chapter.Course));
        if (id != chapter.Id) return BadRequest();

        var existing = await db.Chapters.FindAsync(id);
        if (existing == null) return NotFound();

        if (deleteFile && !string.IsNullOrEmpty(existing.SlidePath))
        {
            await fileStorage.DeleteAsync(existing.SlidePath);
            existing.SlideFileName = null;
            existing.SlidePath = null;
        }
        else if (slideFile != null && slideFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(existing.SlidePath))
                await fileStorage.DeleteAsync(existing.SlidePath);

            var (fileName, path) = await fileStorage.UploadAsync(slideFile);
            existing.SlideFileName = slideFile.FileName;
            existing.SlidePath = path;
        }

        existing.Title = chapter.Title;
        existing.ChapterNumber = chapter.ChapterNumber;

        if (!ModelState.IsValid)
        {
            ViewBag.Course = await db.Courses.FindAsync(existing.CourseId);
            return View(existing);
        }

        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index), new { courseId = existing.CourseId });
    }

    public async Task<IActionResult> Delete(int id)
    {
        var chapter = await db.Chapters.Include(c => c.Course).FirstOrDefaultAsync(c => c.Id == id);
        if (chapter == null) return NotFound();
        ViewBag.QuestionCount = await db.Questions.CountAsync(q => q.ChapterId == id);
        return View(chapter);
    }

    [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var chapter = await db.Chapters
            .Include(c => c.Questions).ThenInclude(q => q.ExamQuestions)
            .FirstOrDefaultAsync(c => c.Id == id);
        if (chapter != null)
        {
            if (!string.IsNullOrEmpty(chapter.SlidePath))
                await fileStorage.DeleteAsync(chapter.SlidePath);

            db.ExamQuestions.RemoveRange(chapter.Questions.SelectMany(q => q.ExamQuestions));

            var courseId = chapter.CourseId;
            db.Chapters.Remove(chapter);
            await db.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { courseId });
        }
        return RedirectToAction(nameof(Index), new { courseId = 0 });
    }
}
