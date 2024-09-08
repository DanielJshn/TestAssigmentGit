using System.Data;
using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using testProd.auth;

namespace testProd.task
{
    public class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IMapper _mapper;
        private readonly DataContext _dataContext;
        private readonly ILog _logger;

        public TaskService(ITaskRepository taskRepository, IMapper mapper, DataContext dataContext, ILog logger)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _dataContext = dataContext;
            _logger = logger;
        }

        public async Task<User> GetUserByTokenAsync(ClaimsPrincipal userClaims)
        {
            _logger.LogInfo("Retrieving user by token.");

            var email = userClaims.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                _logger.LogWarning("User email not found in token.");
                throw new UnauthorizedAccessException("User email not found in token.");
            }

            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                _logger.LogWarning("User with email {Email} not found.", email);
                throw new UnauthorizedAccessException("User not found.");
            }

            _logger.LogInfo("User with email {Email} retrieved successfully.", email);
            return user;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(TaskModelDto taskDto, Guid userId)
        {
            _logger.LogInfo("Creating new task for user {UserId}.", userId);

            if (string.IsNullOrEmpty(taskDto.Title))
            {
                _logger.LogWarning("Task creation failed: Title is required.");
                throw new ArgumentException("Title is required");
            }

            var taskModel = _mapper.Map<TaskModel>(taskDto);
            taskModel.Id = Guid.NewGuid();
            taskModel.UserId = userId;
            taskModel.CreatedAt = DateTime.UtcNow;
            taskModel.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.AddAsync(taskModel);

            _logger.LogInfo("Task {TaskId} created successfully for user {UserId}.", taskModel.Id, userId);
            return _mapper.Map<TaskResponseDto>(taskModel);
        }

        public async Task<PaginatedList<TaskResponseDto>> GetTasksAsync
        (
            Guid userId,
            int pageIndex,
            int pageSize,
            int? status,
            DateTime? dueDate,
            int? priority
        )
        {
            _logger.LogInfo("Retrieving tasks for user {UserId}.", userId);

            var tasks = await _taskRepository.GetTasksAsync(userId, pageIndex, pageSize, status, dueDate, priority);

            _logger.LogInfo("Tasks for user {UserId} retrieved successfully.", userId);
            return _mapper.Map<PaginatedList<TaskResponseDto>>(tasks);
        }

        public async Task<TaskResponseDto> GetSingleTaskAsync(Guid id, Guid userId)
        {
            _logger.LogInfo("Retrieving task {TaskId} for user {UserId}.", id, userId);

            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found for user {UserId}.", id, userId);
                throw new KeyNotFoundException("Task not found.");
            }
            if (task.UserId != userId)
            {
                _logger.LogWarning("Unauthorized access to task {TaskId} by user {UserId}.", id, userId);
                throw new UnauthorizedAccessException("You do not have permission to view this task.");
            }

            _logger.LogInfo("Task {TaskId} retrieved successfully for user {UserId}.", id, userId);
            return _mapper.Map<TaskResponseDto>(task);
        }


        public async Task<TaskResponseDto> UpdateTaskAsync(Guid id, TaskModelDto taskDto, Guid userId)
        {
            _logger.LogInfo("Updating task {TaskId} for user {UserId}.", id, userId);

            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found for user {UserId}.", id, userId);
                throw new KeyNotFoundException("Task not found.");
            }
            if (task.UserId != userId)
            {
                _logger.LogWarning("Unauthorized update attempt on task {TaskId} by user {UserId}.", id, userId);
                throw new UnauthorizedAccessException("You do not have permission to update this task.");
            }

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.DueDate = taskDto.DueDate;
            task.Status = Enum.Parse<TaskStatus>(taskDto.Status, true);
            task.Priority = Enum.Parse<TaskPriority>(taskDto.Priority, true);
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            _logger.LogInfo("Task {TaskId} updated successfully for user {UserId}.", id, userId);
            return _mapper.Map<TaskResponseDto>(task);
        }


        public async Task DeleteTaskAsync(Guid id, Guid userId)
        {
            _logger.LogInfo("Deleting task {TaskId} for user {UserId}.", id, userId);

            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null)
            {
                _logger.LogWarning("Task {TaskId} not found for user {UserId}.", id, userId);
                throw new KeyNotFoundException("Task not found.");
            }

            if (task.UserId != userId)
            {
                _logger.LogWarning("Unauthorized delete attempt on task {TaskId} by user {UserId}.", id, userId);
                throw new UnauthorizedAccessException("You do not have permission to delete this task.");
            }

            _taskRepository.Delete(task);

            _logger.LogInfo("Task {TaskId} deleted successfully for user {UserId}.", id, userId);
        }

    }

}