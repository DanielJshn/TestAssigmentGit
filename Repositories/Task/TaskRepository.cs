using Microsoft.EntityFrameworkCore;

namespace testProd.task
{
    public class TaskRepository : ITaskRepository
    {
        private readonly DataContext _dataContext;

        public TaskRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task AddAsync(TaskModel task)
        {
            await _dataContext.Tasks.AddAsync(task);
        }

        public async Task<TaskModel> GetByIdAsync(Guid id)
        {
            return await _dataContext.Tasks.FindAsync(id);
        }

        public async Task<IEnumerable<TaskModel>> GetTasksByUserIdAsync(Guid userId, TaskFilterDto filterDto)
        {

            return await _dataContext.Tasks
                .Where(t => t.UserId == userId)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _dataContext.SaveChangesAsync();
        }
        public async Task<IQueryable<TaskModel>> GetTasksByUserId(Guid userId)
        {
            return _dataContext.Tasks.Where(t => t.UserId == userId);
        }
        public async Task<TaskModel> GetTaskByIdAsync(Guid id)
        {
            return await _dataContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<IEnumerable<TaskModel>> GetTasksAsync(Guid userId, int? status, DateTime? dueDate, int? priority)
        {
            var query = _dataContext.Tasks.Where(t => t.UserId == userId);

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == (TaskStatus)status.Value);
            }
            if (dueDate.HasValue)
            {
                query = query.Where(t => t.DueDate == dueDate.Value);
            }
            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == (TaskPriority)priority.Value);
            }

            return await query.ToListAsync();
        }

        public async Task UpdateAsync(TaskModel task)
        {
            _dataContext.Tasks.Update(task);

            await _dataContext.SaveChangesAsync();
        }
        public void Delete(TaskModel task)
        {
            _dataContext.Tasks.Remove(task);
        }


    }
}