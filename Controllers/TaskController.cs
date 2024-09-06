using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using testProd.auth;

namespace testProd.task
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class taskController : ControllerBase
    {
        private readonly ITaskService _taskService;
        public taskController(ITaskService taskService)
        {
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
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            var response = await _taskService.CreateTaskAsync(taskDto, user.Id);
            return Ok(response);
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTasks([FromQuery] int? status, [FromQuery] DateTime? dueDate, [FromQuery] int? priority)
        {

            User user;
            try
            {
                user = await _taskService.GetUserByTokenAsync(User);
                var tasks = await _taskService.GetTasksAsync(user.Id, status, dueDate, priority);
                return Ok(tasks);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetTaskById(Guid id)
        {
            try
            {
                var user = await _taskService.GetUserByTokenAsync(User);
                var userId = user.Id;
                var taskResponse = await _taskService.GetSingleTaskAsync(id, userId);
                return Ok(taskResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        

    }
}