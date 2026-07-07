using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models;

public enum CourseLevel { Bachelor, Master }

public class Course
{
    public int Id { get; set; }

    [Display(Name = "Titel")]
    [Required(ErrorMessage = "Bitte einen Titel eingeben.")]
    [MaxLength(200)]
    public string Title { get; set; } = "";

    [Display(Name = "Dozent/in")]
    [Required(ErrorMessage = "Bitte einen Dozentennamen eingeben.")]
    [MaxLength(200)]
    public string LecturerName { get; set; } = "";

    public CourseLevel Level { get; set; }

    public ICollection<Chapter> Chapters { get; set; } = [];
    public ICollection<Exam> Exams { get; set; } = [];
}
