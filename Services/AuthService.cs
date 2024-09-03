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

        private void CheckUserExistence(string email, string errorMessage)
        {
            var existingUser = _authRepository.GetUserByEmail(email);
            if (existingUser == null)
            {
                throw new Exception(errorMessage);
            }
        }

        public void CheckUser(UserAuthDto userForRegistration)
        {
            CheckUserExistence(userForRegistration.Email, "User with this email already exists!");
        }

        public void CheckEmail(UserAuthDto userForLogin)
        {
            CheckUserExistence(userForLogin.Email, "Email is incorrect!");
        }

        public string ReturnToken(UserAuthDto userForRegistration)
        {
            string passwordHash = _authHelp.GetPasswordHash(userForRegistration.Password);
            string token = _authHelp.CreateToken(userForRegistration.Email);

            var tokenEntity = new User
            {
                Username = userForRegistration.Name,
                Email = userForRegistration.Email,
                PasswordHash = passwordHash,
            };

            _authRepository.AddUser(tokenEntity);

            return token;
        }

        public void CheckPassword(UserAuthDto userForLogin)
        {
            var user = _authRepository.GetUserByEmail(userForLogin.Email);
            if (user == null)
            {
                throw new Exception("Incorrect Email");
            }

            string inputPasswordHash = _authHelp.GetPasswordHash(userForLogin.Password);

            if (!inputPasswordHash.SequenceEqual(user.PasswordHash))
            {
                throw new Exception("Incorrect Password");
            }
        }
    }
}