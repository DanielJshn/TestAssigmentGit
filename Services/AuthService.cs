using System;
using System.Linq;
using System.Threading.Tasks;

namespace testProd.auth
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _authRepository;
        private readonly AuthHelp _authHelp;

        public AuthService(IAuthRepository authRepository, AuthHelp authHelp)
        {
            _authRepository = authRepository;
            _authHelp = authHelp;
        }

        private async Task<bool> CheckUserExistsAsync(string email)
        {
            var existingUser = await _authRepository.GetUserByEmailAsync(email);
            Console.WriteLine(existingUser + "res");
            return existingUser != null;
        }

        public async Task CheckUserAsync(UserAuthDto userForRegistration)
        {
            bool userExists = await CheckUserExistsAsync(userForRegistration.Email);
            if (userExists)
            {
                throw new Exception("User with this email already exists!");
            }
        }

        public async Task CheckEmailAsync(UserAuthDto userForLogin)
        {
            bool userExists = await CheckUserExistsAsync(userForLogin.Email);
            if (!userExists)
            {
                throw new Exception("Incorrect Email");
            }
        }

        public async Task<string> ReturnTokenAsync(UserAuthDto userForRegistration)
        {
            string passwordHash = _authHelp.GetPasswordHash(userForRegistration.Password);
            string token = _authHelp.CreateToken(userForRegistration.Email);

            var tokenEntity = new User
            {
                Username = userForRegistration.Name,
                Email = userForRegistration.Email,
                PasswordHash = passwordHash,
            };

            await _authRepository.AddUserAsync(tokenEntity);

            return token;
        }

        public async Task CheckPasswordAsync(UserAuthDto userForLogin)
        {
            var user = await _authRepository.GetUserByEmailAsync(userForLogin.Email);
            if (user == null)
            {
                throw new Exception("Incorrect Email");
            }

            string inputPasswordHash = _authHelp.GetPasswordHash(userForLogin.Password);

            
            if (!inputPasswordHash.Equals(user.PasswordHash))
            {
                throw new Exception("Incorrect Password");
            }
        }

        public Task ValidateRegistrationDataAsync(UserAuthDto userForRegistration)
        {
            if (string.IsNullOrWhiteSpace(userForRegistration.Name) || userForRegistration.Name.Length < 3)
            {
                throw new InvalidOperationException("Name must be at least 3 characters long.");
            }

            if (userForRegistration.Password.Length < 8 ||
                !userForRegistration.Password.Any(char.IsDigit) ||
                !userForRegistration.Password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                throw new InvalidOperationException("Password must be at least 8 characters long, contain at least one digit and one special character.");
            }

            if (!userForRegistration.Email.Contains("@"))
            {
                throw new InvalidOperationException("Email must contain '@' symbol.");
            }

            return Task.CompletedTask; 
        }
    }
}
