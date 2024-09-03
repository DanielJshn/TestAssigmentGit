namespace testProd.auth
{
    public interface IAuthRepository
    {
        public void CheckUser(UserAuthDto userForRegistration);
        public string ReturnToken(UserAuthDto userForRegistration);
    }
}