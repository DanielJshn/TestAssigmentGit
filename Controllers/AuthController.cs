using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace testProd.auth
{
    [ApiController]
    [Route("[controller]")]
    public class Authorization : ControllerBase
    {
        private readonly AuthHelp _authHelp;
        private readonly IAuthService _authService;

        public Authorization(AuthHelp authHelp, IAuthService authService)
        {
            _authHelp = authHelp;
            _authService = authService;

        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public async Task<IActionResult> Register(UserAuthDto userForRegistration)
        {
            string token;
            try
            {
                await _authService.ValidateRegistrationDataAsync(userForRegistration);
                await _authService.CheckUserAsync(userForRegistration);
                token = await _authService.ReturnTokenAsync(userForRegistration);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new { Token = token });
        }



        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserAuthDto userForLogin)
        {
            string newToken;
            try
            {
                {
                    _authService.CheckEmailAsync(userForLogin);
                    _authService.CheckPasswordAsync(userForLogin);
                    newToken = _authHelp.GenerateNewToken(userForLogin.Email);
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new { Token = newToken });
        }
    }
}