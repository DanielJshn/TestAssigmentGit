using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace testProd.auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly IAuthHelp _authHelp;

        private const int REQUIRED_USER_NAME_LENGTH = 3;
        private const int REQUIRED_USER_PASSWORD_LENGTH = 8;

        public AuthService(IAuthRepository authRepository, IAuthHelp authHelp)
        {
            _authRepository = authRepository;
            _authHelp = authHelp;
        }

        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(email);

            return existingUser != null;
        }
        private async Task<bool> CheckNameExistsAsync(string username)
        {
            var existingUser = await _authRepository.GetUserByNameAsync(username);

            return existingUser != null;
        }
        public async Task CheckNameAsync(UserAuthDto userForLogin)
        {
            bool userExists = await CheckNameExistsAsync(userForLogin.Name);

            if (userExists)
            {
                throw new Exception("User with this Name already exists!");
            }
        }


        public async Task CheckUserAsync(UserAuthDto userForRegistration)
        {
            bool userExists = await CheckEmailExistsAsync(userForRegistration.Email);
            if (userExists)
            {
                throw new Exception("User with this email already exists!");
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            var user = await _authRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                throw new Exception("Incorrect Email");
            }
            return user;
        }

        public async Task CheckEmailAsync(UserAuthDto userForLogin)
        {
            bool userExists = await CheckEmailExistsAsync(userForLogin.Email);
            if (!userExists) // todo check this, was userExists == null before
            {
                throw new Exception("Incorrect Email");
            }
        }

        public async Task CheckUserNameAsync(UserAuthDto userForLogin)
        {
            bool userExists = await CheckNameExistsAsync(userForLogin.Email);
            if (!userExists) // todo check this, was userExists == null before
            {
                throw new Exception("Incorrect Name");
            }
        }

        public async Task CheckEmailOrNameAsync(UserAuthDto userForLogin)
        {
            // Проверяем по email или имени
            bool emailCorrect = await CheckEmailExistsAsync(userForLogin.Email);
            bool userExists = await CheckNameExistsAsync(userForLogin.Name);

            if (!emailCorrect && !userExists) 
            {
                throw new Exception("Incorrect email or username.");
            }
        }



        public async Task<string> GenerateTokenAsync(UserAuthDto userForRegistration)
        {
            string passwordHash = _authHelp.GetPasswordHash(userForRegistration.Password);
            string token = _authHelp.GenerateNewToken(userForRegistration.Email);

            var tokenEntity = new User
            {
                Id = Guid.NewGuid(),
                Username = userForRegistration.Name,
                Email = userForRegistration.Email,
                PasswordHash = passwordHash,
            };

            await _authRepository.AddUserAsync(tokenEntity);

            return token;
        }
        public async Task<string> GenerateNewTokenAsync(UserAuthDto userForRegistration)
        {
            string passwordHash = _authHelp.GetPasswordHash(userForRegistration.Password);
            string token = _authHelp.GenerateNewToken(userForRegistration.Email);

            var tokenEntity = new User
            {
                Id = Guid.NewGuid(),
                Username = userForRegistration.Name,
                Email = userForRegistration.Email,
                PasswordHash = passwordHash,
            };

            await _authRepository.AddUserAsync(tokenEntity);

            return token;
        }

        public async Task CheckPasswordAsync(UserAuthDto userForLogin)
        {
            User? user = await _authRepository.GetUserByEmailAsync(userForLogin.Email);

            if (user == null)
            {
                user = await _authRepository.GetUserByNameAsync(userForLogin.Name);
            }

            if (user == null)
            {
                throw new Exception("User not found");
            }
            string inputPasswordHash = _authHelp.GetPasswordHash(userForLogin.Password);

            if (!inputPasswordHash.Equals(user.PasswordHash))
            {
                throw new Exception("Incorrect Password");
            }
        }


        public Task ValidateRegistrationDataAsync(UserAuthDto userForRegistration)
        {
            if (string.IsNullOrWhiteSpace(userForRegistration.Name) || userForRegistration.Name.Length < REQUIRED_USER_NAME_LENGTH)
            {
                throw new InvalidOperationException("Name must be at least 3 characters long.");
            }

            if (userForRegistration.Password.Length < REQUIRED_USER_PASSWORD_LENGTH ||
                !userForRegistration.Password.Any(char.IsDigit) ||
                !userForRegistration.Password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                throw new InvalidOperationException("Password must be at least 8 characters long, contain at least one digit and one special character.");
            }

            if (!IsEmailValid(userForRegistration.Email))
            {
                throw new InvalidOperationException("Email must contain '@' symbol.");
            }

            return Task.CompletedTask;
        }

        private bool IsEmailValid(string email)
        {
            Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            Match match = regex.Match(email);
            return match.Success;
        }

        public async Task<string> GenerateTokenForLogin(UserAuthDto userAuthDto)
        {

            string email = userAuthDto.Email;

            if (string.IsNullOrEmpty(email))
            {
                email = await _authRepository.GetEmailByUserNameAsync(userAuthDto.Name);
                if (string.IsNullOrEmpty(email))
                {
                    throw new Exception("User not found");
                }
            }

            string token = _authHelp.GenerateNewToken(email);
            return token;
        }

        

    }
}
