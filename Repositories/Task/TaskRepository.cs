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
    }
}