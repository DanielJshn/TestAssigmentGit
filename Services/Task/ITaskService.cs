using System.Security.Claims;
using testProd.auth;

namespace testProd.task
{
    public interface ITaskService
    {
        Task<TaskResponseDto> CreateTaskAsync(TaskModelDto taskDto, Guid userId);
        Task<User> GetUserByTokenAsync(ClaimsPrincipal userClaims);
        Task<TaskResponseDto> GetSingleTaskAsync(Guid id, Guid userId);
        Task<IEnumerable<TaskResponseDto>> GetTasksAsync(Guid userId, int? status, DateTime? dueDate, int? priority);
        Task<TaskResponseDto> UpdateTaskAsync(Guid id, TaskModelDto taskDto, Guid userId);
        Task DeleteTaskAsync(Guid id, Guid userId);
    }

}