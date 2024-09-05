using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using testProd.auth;

namespace testProd.auth
{
    public class AuthRepository : IAuthRepository
    {
        private readonly DataContext _dataContext;

        public AuthRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
        public async Task<User> GetUserByNameAsync(string name)
        {
            return await _dataContext.Users.FirstOrDefaultAsync(u => u.Username == name);
        }

        public async Task AddUserAsync(User user)
        {
            await _dataContext.Users.AddAsync(user);
            await _dataContext.SaveChangesAsync();
        }
        public async Task<string> GetEmailByUserNameAsync(string name)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Username == name);

            return user.Email;

        }
    }
}
