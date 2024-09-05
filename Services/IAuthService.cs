using System.Threading.Tasks;

namespace testProd.auth
{
    public interface IAuthService
    {
        Task CheckUserAsync(UserAuthDto userForRegistration);
        Task CheckEmailAsync(UserAuthDto userForLogin);
        Task<string> GenerateTokenAsync(UserAuthDto userForRegistration);
        Task CheckPasswordAsync(UserAuthDto userForLogin);
        Task ValidateRegistrationDataAsync(UserAuthDto userForRegistration);
        Task CheckNameAsync(UserAuthDto userForLogin);
        Task CheckUserNameAsync(UserAuthDto userForLogin);
        Task CheckEmailOrNameAsync(UserAuthDto userForLogin);
        Task<string> GenerateTokenForLogin(UserAuthDto userAuthDto);
    }
}
