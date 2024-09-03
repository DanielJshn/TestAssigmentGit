namespace testProd.auth
{
    public interface IAuthRepository
    {
        public User GetUserByEmail(string email);
        void AddUser(User user);
        public void CheckEmail(UserAuthDto userForLogin);
        public void CheckPassword(UserAuthDto userForLogin);
    }
}