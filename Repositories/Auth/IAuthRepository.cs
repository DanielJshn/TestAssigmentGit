namespace testProd.auth
{
    public interface IAuthRepository
    {
        public void CheckUser(UserForRegistrationDto userForRegistration);
        public string ReturnToken(UserForRegistrationDto userForRegistration);
    }
}