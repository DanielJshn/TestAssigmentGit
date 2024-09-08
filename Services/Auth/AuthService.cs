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
        private readonly ILog _logger;

        public AuthService(IAuthRepository authRepository, IAuthHelp authHelp, ILog logger)
        {
            _authRepository = authRepository;
            _authHelp = authHelp;
            _logger = logger;
        }

        private const int REQUIRED_USER_NAME_LENGTH = 3;
        private const int REQUIRED_USER_PASSWORD_LENGTH = 8;
        private async Task<bool> CheckEmailExistsAsync(string email)
        {
            _logger.LogInfo("Checking if email exists: {Email}", email);
            var existingUser = await _authRepository.GetUserByEmailAsync(email);
            return existingUser != null;
        }

        private async Task<bool> CheckNameExistsAsync(string username)
        {
            _logger.LogInfo("Checking if username exists: {Username}", username);
            var existingUser = await _authRepository.GetUserByNameAsync(username);
            return existingUser != null;
        }

        public async Task CheckNameAsync(UserAuthDto userForLogin)
        {
            _logger.LogInfo("Checking if name exists for user: {Name}", userForLogin.Name);
            bool userExists = await CheckNameExistsAsync(userForLogin.Name);

            if (userExists)
            {
                _logger.LogWarning("User with name {Name} already exists", userForLogin.Name);
                throw new Exception("User with this Name already exists!");
            }
        }

        public async Task CheckUserAsync(UserAuthDto userForRegistration)
        {
            _logger.LogInfo("Checking if email exists for user: {Email}", userForRegistration.Email);
            bool userExists = await CheckEmailExistsAsync(userForRegistration.Email);

            if (userExists)
            {
                _logger.LogWarning("User with email {Email} already exists", userForRegistration.Email);
                throw new Exception("User with this email already exists!");
            }
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            _logger.LogInfo("Fetching user by email: {Email}", email);
            var user = await _authRepository.GetUserByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", email);
                throw new Exception("Incorrect Email");
            }

            return user;
        }

        public async Task CheckEmailAsync(UserAuthDto userForLogin)
        {
            _logger.LogInfo("Checking if email exists for login: {Email}", userForLogin.Email);
            bool userExists = await CheckEmailExistsAsync(userForLogin.Email);

            if (!userExists)
            {
                _logger.LogWarning("Incorrect email: {Email}", userForLogin.Email);
                throw new Exception("Incorrect Email");
            }
        }

        public async Task CheckUserNameAsync(UserAuthDto userForLogin)
        {
            _logger.LogInfo("Checking if username exists for login: {Name}", userForLogin.Email);
            bool userExists = await CheckNameExistsAsync(userForLogin.Email);

            if (!userExists)
            {
                _logger.LogWarning("Incorrect username: {Name}", userForLogin.Email);
                throw new Exception("Incorrect Name");
            }
        }

        public async Task CheckEmailOrNameAsync(UserAuthDto userForLogin)
        {
            _logger.LogInfo("Checking if email or username exists for login.");
            bool emailCorrect = await CheckEmailExistsAsync(userForLogin.Email);
            bool userExists = await CheckNameExistsAsync(userForLogin.Name);

            if (!emailCorrect && !userExists)
            {
                _logger.LogWarning("Incorrect email or username for login: {Email}, {Name}", userForLogin.Email, userForLogin.Name);
                throw new Exception("Incorrect email or username.");
            }
        }

        public async Task<string> GenerateTokenAsync(UserAuthDto userForRegistration)
        {
            _logger.LogInfo("Generating token for user: {Email}", userForRegistration.Email);
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
            _logger.LogInfo("Token generated for user: {Email}", userForRegistration.Email);

            return token;
        }

        public async Task<string> GenerateNewTokenAsync(UserAuthDto userForRegistration)
        {
            _logger.LogInfo("Generating new token for user: {Email}", userForRegistration.Email);
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
            _logger.LogInfo("New token generated for user: {Email}", userForRegistration.Email);

            return token;
        }

        public async Task CheckPasswordAsync(UserAuthDto userForLogin)
        {
            _logger.LogInfo("Checking password for user: {Email}", userForLogin.Email);
            User? user = await _authRepository.GetUserByEmailAsync(userForLogin.Email) ??
                         await _authRepository.GetUserByNameAsync(userForLogin.Name);

            if (user == null)
            {
                _logger.LogWarning("User not found: {Email}, {Name}", userForLogin.Email, userForLogin.Name);
                throw new Exception("User not found");
            }

            string inputPasswordHash = _authHelp.GetPasswordHash(userForLogin.Password);

            if (!inputPasswordHash.Equals(user.PasswordHash))
            {
                _logger.LogWarning("Incorrect password for user: {Email}", userForLogin.Email);
                throw new Exception("Incorrect Password");
            }
        }

        public Task ValidateRegistrationDataAsync(UserAuthDto userForRegistration)
        {
            _logger.LogInfo("Validating registration data for user: {Email}", userForRegistration.Email);

            if (string.IsNullOrWhiteSpace(userForRegistration.Name) || userForRegistration.Name.Length < REQUIRED_USER_NAME_LENGTH)
            {
                _logger.LogWarning("Invalid username: {Name}", userForRegistration.Name);
                throw new InvalidOperationException("Name must be at least 3 characters long.");
            }

            if (userForRegistration.Password.Length < REQUIRED_USER_PASSWORD_LENGTH ||
                !userForRegistration.Password.Any(char.IsDigit) ||
                !userForRegistration.Password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                _logger.LogWarning("Invalid password for user: {Email}", userForRegistration.Email);
                throw new InvalidOperationException("Password must be at least 8 characters long, contain at least one digit and one special character.");
            }

            if (!IsEmailValid(userForRegistration.Email))
            {
                _logger.LogWarning("Invalid email format: {Email}", userForRegistration.Email);
                throw new InvalidOperationException("Email must contain '@' symbol.");
            }

            return Task.CompletedTask;
        }

        private bool IsEmailValid(string email)
        {
            _logger.LogInfo("Validating email format: {Email}", email);
            Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
            Match match = regex.Match(email);
            return match.Success;
        }

        public async Task<string> GenerateTokenForLogin(UserAuthDto userAuthDto)
        {
            _logger.LogInfo("Generating token for login: {Email}, {Name}", userAuthDto.Email, userAuthDto.Name);
            string email = userAuthDto.Email;

            if (string.IsNullOrEmpty(email))
            {
                _logger.LogInfo("Fetching email by username: {Name}", userAuthDto.Name);
                email = await _authRepository.GetEmailByUserNameAsync(userAuthDto.Name);
                if (string.IsNullOrEmpty(email))
                {
                    _logger.LogWarning("User not found: {Name}", userAuthDto.Name);
                    throw new Exception("User not found");
                }
            }

            string token = _authHelp.GenerateNewToken(email);
            _logger.LogInfo("Token generated for login: {Email}", email);
            return token;
        }

    }

}

