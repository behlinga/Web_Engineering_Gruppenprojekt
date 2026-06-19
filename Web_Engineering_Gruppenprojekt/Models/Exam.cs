using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models;

public class Exam
{
    public int Id { get; set; }

    [DataType(DataType.Date)]
    public DateTime Date { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;

    public ICollection<ExamQuestion> ExamQuestions { get; set; } = [];
}
