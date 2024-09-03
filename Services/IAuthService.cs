namespace testProd.auth
{
    public interface IAuthService
    {
        void CheckUser(UserAuthDto userForRegistration);
        string ReturnToken(UserAuthDto userForRegistration);
        void CheckEmail(UserAuthDto userForLogin);
        void CheckPassword(UserAuthDto userForLogin);
    }
}