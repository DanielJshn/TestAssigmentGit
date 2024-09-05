using System.Threading.Tasks;

namespace testProd.auth
{
    public interface IAuthRepository
    {
        Task<User> GetUserByEmailAsync(string email);
        Task AddUserAsync(User user);
        Task<User> GetUserByNameAsync(string name);
        Task<string> GetEmailByUserNameAsync(string name);
    }
}
