using System.ComponentModel.DataAnnotations;

namespace Application_Security_Project_v2.Models
{
    public class Login
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string PasswordHash { get; set; }
        public bool RememberMe { get; set; }
    }
}
