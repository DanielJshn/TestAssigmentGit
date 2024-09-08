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

        public TaskService(ITaskRepository taskRepository, IMapper mapper, DataContext dataContext)
        {
            _taskRepository = taskRepository;
            _mapper = mapper;
            _dataContext = dataContext;
        }

        public async Task<User> GetUserByTokenAsync(ClaimsPrincipal userClaims)
        {
            var email = userClaims.FindFirstValue(ClaimTypes.Email);
            if (email == null)
            {
                throw new UnauthorizedAccessException("User email not found in token.");
            }
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            return user;
        }

        public async Task<TaskResponseDto> CreateTaskAsync(TaskModelDto taskDto, Guid userId)
        {
            if (string.IsNullOrEmpty(taskDto.Title))
            {
                throw new ArgumentException("Title is required");
            }

            var taskModel = _mapper.Map<TaskModel>(taskDto);
            taskModel.Id = Guid.NewGuid();
            taskModel.UserId = userId;
            taskModel.CreatedAt = DateTime.UtcNow;
            taskModel.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.AddAsync(taskModel);

            return _mapper.Map<TaskResponseDto>(taskModel);
        }

        public async Task<IEnumerable<TaskResponseDto>> GetTasksAsync(Guid userId, int? status, DateTime? dueDate, int? priority)
        {

            var tasks = await _taskRepository.GetTasksAsync(userId, status, dueDate, priority);

            return _mapper.Map<IEnumerable<TaskResponseDto>>(tasks);
        }

        public async Task<TaskResponseDto> GetSingleTaskAsync(Guid id, Guid userId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }
            if (task.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to view this task.");
            }

            return _mapper.Map<TaskResponseDto>(task);
        }


        public async Task<TaskResponseDto> UpdateTaskAsync(Guid id, TaskModelDto taskDto, Guid userId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }
            if (task.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to update this task.");
            }

            task.Title = taskDto.Title;
            task.Description = taskDto.Description;
            task.DueDate = taskDto.DueDate;
            task.Status = Enum.Parse<TaskStatus>(taskDto.Status, true);
            task.Priority = Enum.Parse<TaskPriority>(taskDto.Priority, true);
            task.UpdatedAt = DateTime.UtcNow;

            await _taskRepository.UpdateAsync(task);

            return _mapper.Map<TaskResponseDto>(task);
        }


        public async Task DeleteTaskAsync(Guid id, Guid userId)
        {
            var task = await _taskRepository.GetTaskByIdAsync(id);

            if (task == null)
            {
                throw new KeyNotFoundException("Task not found.");
            }

            if (task.UserId != userId)
            {
                throw new UnauthorizedAccessException("You do not have permission to delete this task.");
            }

            _taskRepository.Delete(task);
        }

    }

}