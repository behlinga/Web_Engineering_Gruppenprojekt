using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models.ViewModels;

public class QuestionEditViewModel
{
    public int Id { get; set; }

    [Required]
    public string QuestionText { get; set; } = "";

    public int ChapterId { get; set; }

    [Required]
    public string Option1 { get; set; } = "";
    [Required]
    public string Option2 { get; set; } = "";
    [Required]
    public string Option3 { get; set; } = "";
    [Required]
    public string Option4 { get; set; } = "";

    [Range(1, 4)]
    public int CorrectOption { get; set; } = 1;
}
