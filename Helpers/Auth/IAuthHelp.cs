namespace testProd.auth
{
    public interface IAuthHelp
    {
        public string GetPasswordHash(string password);
        public string GenerateNewToken(string userEmail);
    }
}