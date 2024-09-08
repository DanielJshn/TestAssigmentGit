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
    [Route("task")]
    public class TaskController : ControllerBase
    {

        private readonly ITaskService _taskService;
        public TaskController(ITaskService taskService)
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
                return BadRequest(new ApiResponse(success: false, message: ex.Message));
            }

            var response = await _taskService.CreateTaskAsync(taskDto, user.Id);
            return Ok(new ApiResponse(success: true, data: response));
        }


        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetTasks(
        [FromQuery] int? status,
        [FromQuery] DateTime? dueDate,
        [FromQuery] int? priority,
        [FromQuery] int pageIndex = 1,
        [FromQuery] int pageSize = 10)
        {

            User user;
            try
            {
                user = await _taskService.GetUserByTokenAsync(User);
                var tasks = await _taskService.GetTasksAsync(user.Id, pageIndex, pageSize, status, dueDate, priority);
                return Ok(new ApiResponse(success: true, data: tasks));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(success: false, message: ex.Message));
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
                return Ok(new ApiResponse(success: true, data: taskResponse));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(success: false, message: ex.Message));
            }
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTask(Guid id, [FromBody] TaskModelDto taskDto)
        {
            try
            {
                var user = await _taskService.GetUserByTokenAsync(User);
                var userId = user.Id;

                var updatedTask = await _taskService.UpdateTaskAsync(id, taskDto, userId);
                return Ok(new ApiResponse(success: true, data: updatedTask));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(success: false, message: ex.Message));
            }

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            try
            {
                var user = await _taskService.GetUserByTokenAsync(User);
                var userId = user.Id;
                await _taskService.DeleteTaskAsync(id, userId);
                return Ok(new ApiResponse(success: true));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse(success: false, message: ex.Message));
            }
        }
    }
}