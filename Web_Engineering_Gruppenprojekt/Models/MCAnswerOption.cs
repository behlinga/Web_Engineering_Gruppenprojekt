using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models;

public class MCAnswerOption
{
    public int Id { get; set; }

    [Display(Name = "Antworttext")]
    [Required(ErrorMessage = "Bitte einen Antworttext eingeben.")]
    public string AnswerText { get; set; } = "";

    public bool IsCorrect { get; set; }

    public int QuestionId { get; set; }
    public MCQuestion Question { get; set; } = null!;
}
