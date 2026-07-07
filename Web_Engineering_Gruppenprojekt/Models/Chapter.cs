using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models;

public class Chapter
{
    public int Id { get; set; }

    [Display(Name = "Titel")]
    [Required(ErrorMessage = "Bitte einen Titel eingeben.")]
    [MaxLength(200)]
    public string Title { get; set; } = "";

    [Display(Name = "Kapitelnummer")]
    [Required(ErrorMessage = "Bitte eine Kapitelnummer eingeben.")]
    [Range(0, int.MaxValue, ErrorMessage = "Die Kapitelnummer darf nicht negativ sein.")]
    public int ChapterNumber { get; set; }

    public string? SlideFileName { get; set; }
    public string? SlidePath { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<MCQuestion> Questions { get; set; } = [];
}
