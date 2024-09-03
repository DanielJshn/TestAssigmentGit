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

        public void CheckUser(UserAuthDto userForRegistration)
        {
            var existingUser = _authRepository.GetUserByEmail(userForRegistration.Email);
            if (existingUser != null)
            {
                throw new Exception("User with this email already exists!");
            }
        }
        public string ReturnToken(UserAuthDto userForRegistration)
        {
            string passwordHash = _authHelp.GetPasswordHash(userForRegistration.Password);
            Console.WriteLine("hash" + passwordHash);
            string token = _authHelp.CreateToken(userForRegistration.Email);

            var tokenEntity = new User
            {
                Username = userForRegistration.Name,
                Email = userForRegistration.Email,
                PasswordHash = passwordHash,
            };

            _authRepository.AddUser(tokenEntity);

            Console.WriteLine("token" + token);
            return token;
        }
    }
}