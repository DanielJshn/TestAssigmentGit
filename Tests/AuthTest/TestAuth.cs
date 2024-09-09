using Moq;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using testProd.auth;
using NUnit.Framework.Legacy;

namespace testProd.Tests
{
    [TestFixture]
    public class AuthControllerTests
    {
        private AuthController _controller = null!;
        private Mock<IAuthService> _mockAuthService = null!;

        [SetUp]
        public void Setup()
        {
            _mockAuthService = new Mock<IAuthService>();
            _controller = new AuthController(_mockAuthService.Object);
        }

        [Test]
        public async Task Register_ShouldReturnToken_WhenRegistrationIsSuccessful()
        {
            // Arrange
            var userForRegistration = new UserAuthDto
            {
                Name = "test_user",
                Email = "test@example.com",
                Password = "password"
            };
            var token = "test_token";

            _mockAuthService.Setup(s => s.ValidateRegistrationDataAsync(userForRegistration))
                            .Returns(Task.CompletedTask);
            _mockAuthService.Setup(s => s.CheckNameAsync(userForRegistration))
                            .Returns(Task.CompletedTask);
            _mockAuthService.Setup(s => s.CheckUserAsync(userForRegistration))
                            .Returns(Task.CompletedTask);
            _mockAuthService.Setup(s => s.GenerateTokenAsync(userForRegistration))
                            .ReturnsAsync(token);

            // Act
            var result = await _controller.Register(userForRegistration) as OkObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected OkObjectResult but got null.");
            ClassicAssert.AreEqual(200, result?.StatusCode, "Expected status code 200.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsTrue(response?.Success, "Expected success response.");
            ClassicAssert.AreEqual(token, ((dynamic?)response?.Data)?.Token, "Expected token does not match.");
        }

        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var userForRegistration = new UserAuthDto
            {
                Name = "test_user",
                Email = "test@example.com",
                Password = "password"
            };

            _mockAuthService.Setup(s => s.ValidateRegistrationDataAsync(userForRegistration))
                            .ThrowsAsync(new Exception("Validation failed"));

            // Act
            var result = await _controller.Register(userForRegistration) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("Validation failed", response?.Message, "Expected error message does not match.");
        }

        [Test]
        public async Task Login_ShouldReturnToken_WhenLoginIsSuccessful()
        {
            // Arrange
            var userForLogin = new UserAuthDto
            {
                Name = "test_user",
                Password = "password"
            };
            var token = "test_token";

            _mockAuthService.Setup(s => s.CheckEmailOrNameAsync(userForLogin))
                            .Returns(Task.CompletedTask);
            _mockAuthService.Setup(s => s.CheckPasswordAsync(userForLogin))
                            .Returns(Task.CompletedTask);
            _mockAuthService.Setup(s => s.GenerateTokenForLogin(userForLogin))
                            .ReturnsAsync(token);

            // Act
            var result = await _controller.Login(userForLogin) as OkObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected OkObjectResult but got null.");
            ClassicAssert.AreEqual(200, result?.StatusCode, "Expected status code 200.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsTrue(response?.Success, "Expected success response.");
            ClassicAssert.AreEqual(token, ((dynamic?)response?.Data)?.Token, "Expected token does not match.");
        }

        [Test]
        public async Task Login_ShouldReturnBadRequest_WhenExceptionIsThrown()
        {
            // Arrange
            var userForLogin = new UserAuthDto
            {
                Name = "test_user",
                Password = "password"
            };

            _mockAuthService.Setup(s => s.CheckEmailOrNameAsync(userForLogin))
                            .ThrowsAsync(new Exception("Invalid credentials"));

            // Act
            var result = await _controller.Login(userForLogin) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("Invalid credentials", response?.Message, "Expected error message does not match.");
        }

        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenUserAlreadyExists()
        {
            // Arrange
            var userForRegistration = new UserAuthDto
            {
                Name = "existing_user",
                Email = "existing@example.com",
                Password = "password"
            };

            _mockAuthService.Setup(s => s.CheckUserAsync(userForRegistration))
                            .ThrowsAsync(new Exception("User already exists"));

            // Act
            var result = await _controller.Register(userForRegistration) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("User already exists", response?.Message, "Expected error message does not match.");
        }

        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenInvalidEmailFormat()
        {
            // Arrange
            var userForRegistration = new UserAuthDto
            {
                Name = "test_user",
                Email = "invalid_email",
                Password = "password"
            };

            _mockAuthService.Setup(s => s.ValidateRegistrationDataAsync(userForRegistration))
                            .ThrowsAsync(new Exception("Invalid email format"));

            // Act
            var result = await _controller.Register(userForRegistration) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("Invalid email format", response?.Message, "Expected error message does not match.");
        }

        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenPasswordTooWeak()
        {
            // Arrange
            var userForRegistration = new UserAuthDto
            {
                Name = "test_user",
                Email = "test@example.com",
                Password = "123"
            };

            _mockAuthService.Setup(s => s.ValidateRegistrationDataAsync(userForRegistration))
                            .ThrowsAsync(new Exception("Password is too weak"));

            // Act
            var result = await _controller.Register(userForRegistration) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("Password is too weak", response?.Message, "Expected error message does not match.");
        }

        [Test]
        public async Task Register_ShouldReturnBadRequest_WhenMissingRequiredFields()
        {
            // Arrange
            var userForRegistration = new UserAuthDto
            {
                Name = "test_user",
                // Email is missing
                Password = "password"
            };

            _mockAuthService.Setup(s => s.ValidateRegistrationDataAsync(userForRegistration))
                            .ThrowsAsync(new Exception("Missing required fields"));

            // Act
            var result = await _controller.Register(userForRegistration) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("Missing required fields", response?.Message, "Expected error message does not match.");
        }

        [Test]
        public async Task Login_ShouldReturnBadRequest_WhenUserDoesNotExist()
        {
            // Arrange
            var userForLogin = new UserAuthDto
            {
                Name = "non_existing_user",
                Password = "password"
            };

            _mockAuthService.Setup(s => s.CheckEmailOrNameAsync(userForLogin))
                            .ThrowsAsync(new Exception("User does not exist"));

            // Act
            var result = await _controller.Login(userForLogin) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("User does not exist", response?.Message, "Expected error message does not match.");
        }

        [Test]
        public async Task Login_ShouldReturnBadRequest_WhenPasswordIsIncorrect()
        {
            // Arrange
            var userForLogin = new UserAuthDto
            {
                Name = "test_user",
                Password = "wrong_password"
            };

            _mockAuthService.Setup(s => s.CheckPasswordAsync(userForLogin))
                            .ThrowsAsync(new Exception("Incorrect password"));

            // Act
            var result = await _controller.Login(userForLogin) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("Incorrect password", response?.Message, "Expected error message does not match.");
        }

        [Test]
        public async Task Login_ShouldReturnBadRequest_WhenUserNameIsEmpty()
        {
            // Arrange
            var userForLogin = new UserAuthDto
            {
                Name = "",
                Password = "password"
            };

            _mockAuthService.Setup(s => s.CheckEmailOrNameAsync(userForLogin))
                            .ThrowsAsync(new Exception("User name is required"));

            // Act
            var result = await _controller.Login(userForLogin) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("User name is required", response?.Message, "Expected error message does not match.");
        }

        [Test]
        public async Task Login_ShouldReturnBadRequest_WhenPasswordIsEmpty()
        {
            // Arrange
            var userForLogin = new UserAuthDto
            {
                Name = "test_user",
                Password = ""
            };

            _mockAuthService.Setup(s => s.CheckPasswordAsync(userForLogin))
                            .ThrowsAsync(new Exception("Password is required"));

            // Act
            var result = await _controller.Login(userForLogin) as BadRequestObjectResult;

            // Assert
            ClassicAssert.IsNotNull(result, "Expected BadRequestObjectResult but got null.");
            ClassicAssert.AreEqual(400, result?.StatusCode, "Expected status code 400.");
            var response = result?.Value as ApiResponse;
            ClassicAssert.IsNotNull(response, "Expected ApiResponse but got null.");
            ClassicAssert.IsFalse(response?.Success, "Expected failure response.");
            ClassicAssert.AreEqual("Password is required", response?.Message, "Expected error message does not match.");
        }
    }
}
