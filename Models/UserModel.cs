using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace testProd.auth
{
    public class User
    {
       public Guid Id { get; set; } = Guid.NewGuid();

        public string Username { get; set; }="";

        public string Email { get; set; }="";

        public string PasswordHash { get; set; }="";

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }


}