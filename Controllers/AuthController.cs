using System.Security.Principal;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace testProd.auth
{
    [ApiController]
    [Route("users")]
    public class users : ControllerBase
    {
        private readonly AuthHelp _authHelp;
        private readonly IAuthService _authService;

        public users(AuthHelp authHelp, IAuthService authService)
        {
            _authHelp = authHelp;
            _authService = authService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(UserAuthDto userForRegistration)
        {
            string token;
            try
            {
                // await _authService.ValidateRegistrationDataAsync(userForRegistration);
                await _authService.CheckNameAsync(userForRegistration);
                await _authService.CheckUserAsync(userForRegistration);
                token = await _authService.GenerateTokenAsync(userForRegistration);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new { Token = token });
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(UserAuthDto userForLogin)
        {
            string newToken;
            try
            {
                await _authService.CheckEmailOrNameAsync(userForLogin);
                await _authService.CheckPasswordAsync(userForLogin);
                newToken =await _authService.GenerateTokenForLogin(userForLogin);

            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(new { Token = newToken });
        }


    }
}