namespace testProd.auth
{
    public interface IAuthService
    {
        public void CheckUser(UserAuthDto userForRegistration);
        string ReturnToken(UserAuthDto userForRegistration);
    }
}