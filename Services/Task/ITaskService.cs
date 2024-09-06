using System.Security.Claims;
using testProd.auth;

namespace testProd.task
{
    public interface ITaskService
    {
        Task<TaskResponseDto> CreateTaskAsync(TaskModelDto taskDto, Guid userId);
        Task<User> GetUserByTokenAsync(ClaimsPrincipal userClaims);
    }

}