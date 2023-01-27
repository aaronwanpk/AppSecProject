using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Application_Security_Project_v2.Models
{
    [Keyless]
    public class Audit
    {

        [Required]
        public string email { get; set; }

        public DateTime actionTime { get; set; } = DateTime.Now;

        [Required]
        public string description { get; set; }

    }
}
