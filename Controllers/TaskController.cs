using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace testProd.task
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class TaskController : ControllerBase
    {
        private readonly DataContext _dataContext;
        public TaskController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpPost]
        public async Task<IActionResult> PostTask(TaskModelDto taskDto)
        {
            
           
            var email = User.FindFirstValue(ClaimTypes.Email);
            Console.WriteLine("email is"+ email);

            if (email == null)
            {
                throw new Exception("User email not found in token.");
            }

            
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized("User not found.");
            }

            var taskModel = new TaskModel
            {
                Id = Guid.NewGuid(),
                Title = taskDto.Title,
                Description = taskDto.Description,
                DueDate = taskDto.DueDate,
                Status = taskDto.Status,
                Priority = taskDto.Priority,
                UserId = user.Id 
            };

            await _dataContext.Tasks.AddAsync(taskModel);
            await _dataContext.SaveChangesAsync();
            return Ok(taskModel);
        }

    }
}