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

        private bool CheckUserExists(string email)
        {
            var existingUser = _authRepository.GetUserByEmail(email);
            Console.WriteLine(existingUser + "res");
            return existingUser != null;
        }

        public void CheckUser(UserAuthDto userForRegistration)
        {
            bool userExists = CheckUserExists(userForRegistration.Email);
            if (userExists)
            {
                throw new Exception("User with this email already exists!");
            }
        }

        public void CheckEmail(UserAuthDto userForLogin)
        {
            bool userExists = CheckUserExists(userForLogin.Email);
            if (!userExists)
            {
                throw new Exception("User with this email already exists!");
            }
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