using System.ComponentModel.DataAnnotations;

namespace TaskManager.Models;

public class TaskRequestBody
{
    [Required(ErrorMessage = "Please provide a task to add")]
    public string Name { get; set; } = string.Empty;
}