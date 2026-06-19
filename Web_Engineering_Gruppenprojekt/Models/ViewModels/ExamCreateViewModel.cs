using System.ComponentModel.DataAnnotations;

namespace Web_Engineering_Gruppenprojekt.Models.ViewModels;

public class ExamCreateViewModel
{
    public int CourseId { get; set; }
    public string CourseTitle { get; set; } = "";

    [Required, Range(1, 100)]
    public int NumberOfQuestions { get; set; } = 10;

    [Required, DataType(DataType.Date)]
    public DateTime Date { get; set; } = DateTime.Today;
}
