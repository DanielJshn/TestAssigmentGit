namespace testProd.auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext? _dataContext;
        private readonly IConfiguration _config;
        private readonly AuthHelp _authHelp;

        public AuthRepository(DataContext dataContext, IConfiguration config, AuthHelp authHelp)
        {
            _dataContext = dataContext;
            _config = config;
            _authHelp = authHelp;
        }

        public User GetUserByEmail(string email)
        {
            return _dataContext.Users.FirstOrDefault(u => u.Email == email);
        }


        public void AddUser(User user)
        {
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();
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
            Console.WriteLine(user);

            var userForConfirmation = _dataContext.Users
                .Where(t => t.Email == userForLogin.Email)
                .Select(t => new UserForLoginConfirmationDto
                {
                    PasswordHash = t.PasswordHash
                })
                .FirstOrDefault();

            string inputPasswordHash = _authHelp.GetPasswordHash(userForLogin.Password);
            if (!inputPasswordHash.SequenceEqual(userForConfirmation.PasswordHash))
            {
                throw new Exception("Incorrect Password");
            }


        }

    }
}