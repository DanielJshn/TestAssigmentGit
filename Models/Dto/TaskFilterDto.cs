namespace testProd.task
{
    public class TaskFilterDto
    {
        public TaskStatus? Status { get; set; }
        public DateTime? DueDate { get; set; }
        public TaskPriority? Priority { get; set; }
    }

}