using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models;

public class MCQuestion
{
    public int Id { get; set; }

    [Required]
    public string QuestionText { get; set; } = "";

    public int ChapterId { get; set; }
    public Chapter Chapter { get; set; } = null!;

    public ICollection<MCAnswerOption> Options { get; set; } = [];
    public ICollection<ExamQuestion> ExamQuestions { get; set; } = [];
}
