namespace testProd.task
{
    public interface ITaskRepository
    {
        Task AddAsync(TaskModel task);
        Task<TaskModel> GetByIdAsync(Guid id);
        Task<IEnumerable<TaskModel>> GetTasksByUserIdAsync(Guid userId, TaskFilterDto filterDto);
        Task UpdateAsync(TaskModel task);
        Task<TaskModel> GetTaskByIdAsync(Guid id);
        Task<PaginatedList<TaskModel>> GetTasksAsync
        (
            Guid userId,
            int pageIndex,
            int pageSize,
            int? status,
            DateTime? dueDate,
            int? priority
        );
        void Delete(TaskModel task);
    }
}