namespace testProd.auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dataContext;
        private readonly IConfiguration _config;
        private readonly AuthHelp _authHelp;

        public AuthRepository(DataContext dataContext, IConfiguration config, AuthHelp authHelp)
        {
            _dataContext = dataContext;
            _config = config;
            _authHelp = authHelp;
        }

        public void CheckUser(UserAuthDto userForRegistration)
        {
            using (var dbContext = new DataContext(_config))
            {
                Console.WriteLine("sss");
                var existingUser = dbContext.Users.FirstOrDefault(u => u.Email == userForRegistration.Email);
                Console.WriteLine("sss2");
                if (existingUser != null)
                {
                    throw new Exception("User with this email already exists!");
                }
                Console.WriteLine("sss3");
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

            using (var dbContext = new DataContext(_config))
            {
                dbContext.Users.Add(tokenEntity);
                dbContext.SaveChanges();
            }
            Console.WriteLine("token" + token);
            return token;
        }

        public void CheckEmail(UserAuthDto userForLogin)
        {

            var token = _dataContext.Users.FirstOrDefault(t => t.Email == userForLogin.Email);

            if (token == null)
            {
                throw new Exception("Email is incorrect!");
            }
        }

        public void CheckPassword(UserAuthDto userForLogin)
        {
            var user = _dataContext.Users.FirstOrDefault(u => u.Email == userForLogin.Email);
            if (user == null)
            {
                throw new Exception("Incorrect Email");
            }
            Console.WriteLine(user );

            var userForConfirmation = _dataContext.Users
                .Where(t => t.Email == userForLogin.Email)
                .Select(t => new UserForLoginConfirmationDto
                {
                    PasswordHash = t.PasswordHash
                })
                .FirstOrDefault();

            string inputPasswordHash = _authHelp.GetPasswordHash(userForLogin.Password );
            if (!inputPasswordHash.SequenceEqual(userForConfirmation.PasswordHash))
            {
                throw new Exception("Incorrect Password");
            }


        }

    }
}