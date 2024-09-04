using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.IdentityModel.Tokens;

namespace testProd.auth
{
    public class AuthHelp
    {
        private readonly IConfiguration _config;
        private readonly DataContext _entity;

        public AuthHelp(IConfiguration config, DataContext entity)
        {
            _config = config;
            _entity = entity;
        }

        public string GetPasswordHash(string password)
        {
           
            string? passwordKeyString = _config.GetSection("AppSettings:PasswordKey").Value;
            Console.WriteLine("token");
            if (string.IsNullOrEmpty(passwordKeyString))
            {
                throw new ArgumentException("PasswordKey is not configured.");
            }

            byte[] passwordKey = Encoding.ASCII.GetBytes(passwordKeyString);

          
            byte[] passwordHash = KeyDerivation.Pbkdf2(
                password: password,
                salt: passwordKey,  
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 1000000,
                numBytesRequested: 256 / 8
            );
            string passwordHashBase64 = Convert.ToBase64String(passwordHash);

            return passwordHashBase64;

        }

        public string CreateToken(string userEmail)
        {
         
            Claim[] claims = new Claim[]
            {
        new Claim(ClaimTypes.Email, userEmail)
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey").Value;

      
            if (string.IsNullOrEmpty(tokenKeyString))
            {
                throw new ArgumentException("TokenKey is not configured.");
            }

            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString));
            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha256Signature);

   
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddMonths(1)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken token = handler.CreateToken(descriptor);

            return handler.WriteToken(token);
        }


        public string GenerateNewToken(string userEmail)
        {

            Claim[] claims = new Claim[]
            {
              new Claim("Email", userEmail)
            };

            string? tokenKeyString = _config.GetSection("AppSettings:TokenKey")?.Value;
            SymmetricSecurityKey tokenKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenKeyString ?? ""));
            SigningCredentials credentials = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);
            SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor()

            {
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credentials,
                Expires = DateTime.Now.AddMonths(1)
            };

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            SecurityToken newtoken = handler.CreateToken(descriptor);

            return handler.WriteToken(newtoken);
        }




    }
}