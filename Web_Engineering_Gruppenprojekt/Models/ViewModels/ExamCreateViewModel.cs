using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models.ViewModels;

public class ExamCreateViewModel
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = "";

    [Display(Name = "Anzahl der Fragen")]
    [Required(ErrorMessage = "Bitte die Anzahl der Fragen angeben.")]
    [Range(1, 100, ErrorMessage = "Die Anzahl der Fragen muss zwischen {1} und {2} liegen.")]
    public int NumberOfQuestions { get; set; } = 10;

    [Display(Name = "Prüfungsdatum")]
    [Required(ErrorMessage = "Bitte ein Prüfungsdatum wählen.")]
    [DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;
}
