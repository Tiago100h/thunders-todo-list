namespace ThundersTodoList.Domain.Entities;

public class Todo
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public bool IsCompleted { get; set; }
}