using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace testProd.auth
{
    [ApiController]
    [Route("[controller]")]
    public class Authorization : ControllerBase
    {
        private readonly AuthHelp _authHelp;
        private readonly AuthRepository _authRepository;
        public Authorization(AuthHelp authHelp, AuthRepository authRepository)
        {
            _authHelp = authHelp;
            _authRepository = authRepository;
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserAuthDto userForRegistration)
        {
            string token;
            try
            {
                _authRepository.CheckUser(userForRegistration);
                token = _authRepository.ReturnToken(userForRegistration);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            return Ok(new { Token = token });
        }
    }
}