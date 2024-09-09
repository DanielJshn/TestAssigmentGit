using Moq;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;
using testProd.auth;
using testProd.task;
using System.Security.Claims;
using testProd;
using NUnit.Framework.Legacy;

[TestFixture]
public class TaskControllerTests
{
    private Mock<ITaskService>? _mockTaskService;
    private TaskController? _controller;

    [SetUp]
    public void Setup()
    {
        _mockTaskService = new Mock<ITaskService>();
        _controller = new TaskController(_mockTaskService.Object);
    }

    [Test]
    public async Task PostTask_ShouldReturnOkResult_WhenTaskIsCreated()
    {
        // Arrange
        var taskDto = new TaskModelDto { Title = "Test Task", Description = "Task Description" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "your_username",
            Email = "your_email@example.com",
            PasswordHash = "hashed_password"
        };

        // Setup mock to return a user
        _mockTaskService?.Setup(s => s.GetUserByTokenAsync(It.IsAny<ClaimsPrincipal>()))
                         .ReturnsAsync(user);

        // Setup mock for CreateTaskAsync to return TaskResponseDto
        var taskResponse = new TaskResponseDto
        {
            Id = Guid.NewGuid(),
            Title = taskDto.Title,
            Description = taskDto.Description,
            // Add any other necessary properties for TaskResponseDto
        };

        _mockTaskService?.Setup(s => s.CreateTaskAsync(It.IsAny<TaskModelDto>(), It.IsAny<Guid>()))
                         .ReturnsAsync(taskResponse);

        // Act
        var result = await _controller!.PostTask(taskDto) as OkObjectResult;
        // Assert
        ClassicAssert.IsNotNull(result, "Expected OkObjectResult but got null.");
        ClassicAssert.AreEqual(200, result?.StatusCode, "Expected status code 200.");
        ClassicAssert.IsTrue(((ApiResponse)result!.Value!).Success, "Expected success response.");
    }

    [Test]
    public async Task PostTask_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        var taskDto = new TaskModelDto { Title = "Test Task", Description = "Task Description" };

        _mockTaskService?.Setup(s => s.GetUserByTokenAsync(It.IsAny<ClaimsPrincipal>()))
                         .ThrowsAsync(new Exception("Error getting user"));

        // Act
        var result = await _controller!.PostTask(taskDto) as BadRequestObjectResult;

        // Assert
        ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
        ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
        ClassicAssert.AreEqual("Error getting user", ((ApiResponse)result!.Value!).Message, "Expected error message.");
    }

    [Test]
    public async Task GetTasks_ShouldReturnOkResult_WhenTasksAreRetrieved()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "your_username",
            Email = "your_email@example.com",
            PasswordHash = "hashed_password"
        };

        var tasks = new List<TaskResponseDto>
    {
        new TaskResponseDto { Title = "Task 1", Description = "Description 1" },
        new TaskResponseDto { Title = "Task 2", Description = "Description 2" }
    };

        var paginatedTasks = new PaginatedList<TaskResponseDto>(tasks, 1, 1); // Create a paginated list with one page

        _mockTaskService?.Setup(s => s.GetUserByTokenAsync(It.IsAny<ClaimsPrincipal>()))
                         .ReturnsAsync(user);

        _mockTaskService?.Setup(s => s.GetTasksAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), null, null, null))
                         .ReturnsAsync(paginatedTasks); // Return the PaginatedList<TaskResponseDto>

        // Act
        var result = await _controller!.GetTasks(null, null, null) as OkObjectResult;

        // Assert
        ClassicAssert.IsNotNull(result, "Expected OkObjectResult but got null.");
        ClassicAssert.AreEqual(200, result?.StatusCode, "Expected status code 200.");
        ClassicAssert.IsTrue(((ApiResponse)result!.Value!).Success, "Expected success response.");

        // Extract paginated tasks from ApiResponse
        var actualApiResponse = (ApiResponse)result!.Value!;
        var actualPaginatedTasks = actualApiResponse.Data as PaginatedList<TaskResponseDto>;

        ClassicAssert.IsNotNull(actualPaginatedTasks, "Expected non-null data in response.");

        // Compare paginated list properties
        ClassicAssert.AreEqual(paginatedTasks.PageIndex, actualPaginatedTasks?.PageIndex, "PageIndex does not match.");
        ClassicAssert.AreEqual(paginatedTasks.TotalPages, actualPaginatedTasks?.TotalPages, "TotalPages does not match.");

        // Compare the tasks list
        ClassicAssert.AreEqual(tasks.Count, actualPaginatedTasks?.Items.Count, "Task counts do not match.");
        for (int i = 0; i < tasks.Count; i++)
        {
            ClassicAssert.AreEqual(tasks[i].Title, actualPaginatedTasks?.Items[i].Title, $"Task title at index {i} does not match.");
            ClassicAssert.AreEqual(tasks[i].Description, actualPaginatedTasks?.Items[i].Description, $"Task description at index {i} does not match.");
        }
    }



    [Test]
    public async Task GetTasks_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        _mockTaskService?.Setup(s => s.GetUserByTokenAsync(It.IsAny<ClaimsPrincipal>()))
                         .ThrowsAsync(new Exception("Error getting tasks"));

        // Act
        var result = await _controller!.GetTasks(null, null, null) as BadRequestObjectResult;

        // Assert
        ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
        ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
        ClassicAssert.AreEqual("Error getting tasks", ((ApiResponse)result!.Value!).Message, "Expected error message.");
    }

    [Test]
    public async Task DeleteTask_ShouldReturnOkResult_WhenTaskIsDeleted()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "your_username",
            Email = "your_email@example.com",
            PasswordHash = "hashed_password"
        };

        _mockTaskService?.Setup(s => s.GetUserByTokenAsync(It.IsAny<ClaimsPrincipal>()))
                         .ReturnsAsync(user);
        _mockTaskService?.Setup(s => s.DeleteTaskAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                         .Returns(Task.CompletedTask);

        // Act
        var result = await _controller!.DeleteTask(Guid.NewGuid()) as OkObjectResult;

        // Assert
        ClassicAssert.IsNotNull(result, "Expected OkObjectResult but got null.");
        ClassicAssert.AreEqual(200, result?.StatusCode, "Expected status code 200.");
        ClassicAssert.IsTrue(((ApiResponse)result!.Value!).Success, "Expected success response.");
    }

    [Test]
    public async Task DeleteTask_ShouldReturnBadRequest_WhenExceptionIsThrown()
    {
        // Arrange
        _mockTaskService?.Setup(s => s.GetUserByTokenAsync(It.IsAny<ClaimsPrincipal>()))
                         .ThrowsAsync(new Exception("Error deleting task"));

        // Act
        var result = await _controller!.DeleteTask(Guid.NewGuid()) as BadRequestObjectResult;

        // Assert
        ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
        ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
        ClassicAssert.AreEqual("Error deleting task", ((ApiResponse)result!.Value!).Message, "Expected error message.");
    }

    [Test]
    public async Task GetTasks_ShouldReturnEmptyList_WhenUserHasNoTasks()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "your_username",
            Email = "your_email@example.com",
            PasswordHash = "hashed_password"
        };

        // Create an empty paginated task list
        var emptyTaskList = new PaginatedList<TaskResponseDto>(new List<TaskResponseDto>(), 1, 1);

        _mockTaskService?.Setup(s => s.GetUserByTokenAsync(It.IsAny<ClaimsPrincipal>()))
                         .ReturnsAsync(user);

        _mockTaskService?.Setup(s => s.GetTasksAsync(It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<int>(), null, null, null))
                         .ReturnsAsync(emptyTaskList);

        // Act
        var result = await _controller!.GetTasks(null, null, null) as OkObjectResult;

        // Assert
        ClassicAssert.IsNotNull(result, "Expected OkObjectResult but got null.");
        ClassicAssert.AreEqual(200, result?.StatusCode, "Expected status code 200.");
        ClassicAssert.IsTrue(((ApiResponse)result!.Value!).Success, "Expected success response.");

        // Verify that the task list is empty
        var actualApiResponse = (ApiResponse)result!.Value!;
        var actualTaskList = actualApiResponse.Data as PaginatedList<TaskResponseDto>;
        ClassicAssert.IsNotNull(actualTaskList, "Expected non-null task list in response.");
        ClassicAssert.AreEqual(0, actualTaskList!.Items.Count, "Expected an empty task list.");
    }
}
