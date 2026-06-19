using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models;

public class MCAnswerOption
{
    public int Id { get; set; }

    [Required]
    public string AnswerText { get; set; } = "";

    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
    public MCQuestion Question { get; set; } = null!;
}
