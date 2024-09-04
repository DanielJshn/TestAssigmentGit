using System.Threading.Tasks;

namespace testProd.auth
{
    public interface IAuthService
    {
        Task CheckUserAsync(UserAuthDto userForRegistration);

        Task CheckEmailAsync(UserAuthDto userForLogin);
        
        Task<string> ReturnTokenAsync(UserAuthDto userForRegistration);
        Task CheckPasswordAsync(UserAuthDto userForLogin);
        Task ValidateRegistrationDataAsync(UserAuthDto userForRegistration);
    }
}
