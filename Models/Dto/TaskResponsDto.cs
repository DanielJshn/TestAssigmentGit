namespace testProd.task
{
    public class TaskResponseDto
    {
       public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } // Строка для статуса
    public string Priority { get; set; } // Строка для приоритета
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    }
}