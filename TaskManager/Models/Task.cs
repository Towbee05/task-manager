namespace TaskManager.Models;

public class Task
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public bool Completed { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}