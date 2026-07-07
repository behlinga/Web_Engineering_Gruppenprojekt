using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models.ViewModels;

public class QuestionEditViewModel
{
    public int Id { get; set; }

    [Display(Name = "Fragetext")]
    [Required(ErrorMessage = "Bitte einen Fragetext eingeben.")]
    public string QuestionText { get; set; } = "";

    public int ChapterId { get; set; }

    [Display(Name = "Option 1")]
    [Required(ErrorMessage = "Bitte Option 1 ausfüllen.")]
    public string Option1 { get; set; } = "";
    [Display(Name = "Option 2")]
    [Required(ErrorMessage = "Bitte Option 2 ausfüllen.")]
    public string Option2 { get; set; } = "";
    [Display(Name = "Option 3")]
    [Required(ErrorMessage = "Bitte Option 3 ausfüllen.")]
    public string Option3 { get; set; } = "";
    [Display(Name = "Option 4")]
    [Required(ErrorMessage = "Bitte Option 4 ausfüllen.")]
    public string Option4 { get; set; } = "";

    [Display(Name = "Richtige Antwort")]
    [Range(1, 4, ErrorMessage = "Bitte eine gültige Option (1-4) auswählen.")]
    public int CorrectOption { get; set; } = 1;
}
