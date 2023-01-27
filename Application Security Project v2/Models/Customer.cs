using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application_Security_Project_v2.Models
{
    public class Customer
    {
        [Required(ErrorMessage = "Enter your First Name")]
        public string FName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter you Last Name")]
        public string LName { get; set; } = string.Empty;

        [Key]
        [Required(ErrorMessage = "Enter your email")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Enter your credit card number")]
        [MaxLength(16)]
        [Display(Name = "Credit Card Number")]
        public string CreditCard { get; set; } = string.Empty;

        [Required]
        [MinLength(12, ErrorMessage = "Password must be at least 12 characters long")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[$@$!%*?&])[A-Za-z\d$@$!%*?&]{12,}$",
         ErrorMessage = "Passwords must be at least 12 characters long and contain at least an uppercase letter, lower case letter, digit and a symbol")]
        [Display(Name = "Password")]
        public string PasswordHash { get; set; } = string.Empty;
        public string Salt { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Required(ErrorMessage = "Enter your Date of Birth")]
        [Column(TypeName = "date")]
        [Display(Name = "Birthday")]
        public DateTime DateBirth { get; set; } = new DateTime(DateTime.Now.Year - 18, 1, 1);

        [Display(Name = "About Me")]
        public string AboutMe { get; set; } = string.Empty;

        public string? ImageURL { get; set; }

    }
}
