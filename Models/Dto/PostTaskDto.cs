using testProd.auth;

public class TaskModelDto
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime? DueDate { get; set; }
    public string Status { get; set; } // Строка для статуса
    public string Priority { get; set; } // Строка для приоритета
    
}

