namespace Web_Engineering_Gruppenprojekt.Models;

public class ExamQuestion
{
    public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;

    public int QuestionId { get; set; }
    public MCQuestion Question { get; set; } = null!;
}
