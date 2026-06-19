using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models;

public class Chapter
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    public int ChapterNumber { get; set; }

    public string? SlideFileName { get; set; }
    public string? SlidePath { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<MCQuestion> Questions { get; set; } = [];
}
