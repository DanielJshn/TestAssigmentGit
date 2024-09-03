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
        public IActionResult Register(UserAuthDto userForRegistration)
        {
            string token;
            try
            {
                _authService.CheckUser(userForRegistration);
                token = _authService.ReturnToken(userForRegistration);
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
            {
                _authService.CheckEmail(userForLogin);
                _authService.CheckPassword(userForLogin);
                newToken = _authHelp.GenerateNewToken(userForLogin.Email);
            }
            return Ok(new { Token = newToken });
        }
    }
}