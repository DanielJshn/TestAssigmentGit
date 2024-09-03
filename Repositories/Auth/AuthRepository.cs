namespace testProd.auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dataContext;

        public AuthRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
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
    }
}