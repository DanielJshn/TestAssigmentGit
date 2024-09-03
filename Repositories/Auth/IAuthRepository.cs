namespace testProd.auth
{
    public interface IAuthRepository
    {
        User GetUserByEmail(string email);
        void AddUser(User user);
    }
}