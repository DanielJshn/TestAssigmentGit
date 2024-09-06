namespace testProd.task
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskModel task);
        Task<TaskModel> GetByIdAsync(Guid id);
        Task<IEnumerable<TaskModel>> GetTasksByUserIdAsync(Guid userId, TaskFilterDto filterDto);
        Task SaveChangesAsync();
    }
}