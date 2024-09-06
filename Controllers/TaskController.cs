using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testProd.auth;

namespace testProd.task
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly DataContext _dataContext;
        private readonly ITaskService _taskService;
        public TaskController(DataContext dataContext, ITaskService taskService)
        {
            _dataContext = dataContext;
            _taskService = taskService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> PostTask([FromBody] TaskModelDto taskDto)
        {
            User user;
            try
            {
                user = await _taskService.GetUserByTokenAsync(User);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }

            var response = await _taskService.CreateTaskAsync(taskDto, user.Id);
            return Ok(response);
        }
    }
}