using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models;

public class MCQuestion
{
    public int Id { get; set; }

    [Display(Name = "Fragetext")]
    [Required(ErrorMessage = "Bitte einen Fragetext eingeben.")]
    public string QuestionText { get; set; } = "";

    public int ChapterId { get; set; }
    public Chapter Chapter { get; set; } = null!;

    public ICollection<MCAnswerOption> Options { get; set; } = [];
    public ICollection<ExamQuestion> ExamQuestions { get; set; } = [];
}
