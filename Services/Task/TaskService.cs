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
            await _taskRepository.SaveChangesAsync();

            return _mapper.Map<TaskResponseDto>(taskModel);
        }


    }

}