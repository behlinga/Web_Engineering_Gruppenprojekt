using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models;

public enum CourseLevel { Bachelor, Master }

public class Course
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = "";

    [Required, MaxLength(200)]
    public string LecturerName { get; set; } = "";

    public CourseLevel Level { get; set; }

    public ICollection<Chapter> Chapters { get; set; } = [];
    public ICollection<Exam> Exams { get; set; } = [];
}
