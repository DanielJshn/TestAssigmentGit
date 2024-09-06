namespace testProd.task
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskModel task);
        Task<TaskModel> GetByIdAsync(Guid id);
        Task<IEnumerable<TaskModel>> GetTasksByUserIdAsync(Guid userId, TaskFilterDto filterDto);
        Task SaveChangesAsync();
        Task UpdateAsync(TaskModel task);
        Task<TaskModel> GetTaskByIdAsync(Guid id);
        Task<IEnumerable<TaskModel>> GetTasksAsync(Guid userId, int? status, DateTime? dueDate, int? priority);
        void Delete(TaskModel task);
    }
}