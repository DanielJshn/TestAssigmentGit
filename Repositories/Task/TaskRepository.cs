using Microsoft.EntityFrameworkCore;
using testProd.auth;

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
            await _dataContext.SaveChangesAsync();
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

        public async Task<IQueryable<TaskModel>> GetTasksByUserId(Guid userId)
        {
            return _dataContext.Tasks.Where(t => t.UserId == userId);
        }

        public async Task<TaskModel> GetTaskByIdAsync(Guid id)
        {
            return await _dataContext.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<PaginatedList<TaskModel>> GetTasksAsync
        (
            Guid userId,
            int pageIndex,
            int pageSize,
            int? status,
            DateTime? dueDate,
            int? priority
        )
        {
            var query = _dataContext.Tasks.Where(t => t.UserId == userId);

            if (status.HasValue)
            {
                query = query.Where(t => t.Status == (TaskStatus) status.Value);
            }
            if (dueDate.HasValue)
            {
                query = query.Where(t => t.DueDate == dueDate.Value);
            }
            if (priority.HasValue)
            {
                query = query.Where(t => t.Priority == (TaskPriority) priority.Value);
            }

            var count = await query.CountAsync();
            var totalPages = (int) Math.Ceiling(count / (double) pageSize);

            var tasks = await query
                .OrderBy(b => b.Id)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize).ToListAsync();

            return new PaginatedList<TaskModel>(tasks, pageIndex, totalPages);
        }

        public async Task UpdateAsync(TaskModel task)
        {
            _dataContext.Tasks.Update(task);

            await _dataContext.SaveChangesAsync();
        }

        public async Task Delete(TaskModel task)
        {
            _dataContext.Tasks.Remove(task);
            await _dataContext.SaveChangesAsync();
        }

    }
}