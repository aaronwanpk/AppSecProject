using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application_Security_Project_v2.Services;
using Application_Security_Project_v2.Models;
using Azure;
using Microsoft.AspNetCore.Authorization;

namespace Application_Security_Project_v2.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        public const string SessionKeyName = "_Name";
        public const string SessionKeyEmail = "_Email";

        private readonly ILogger<IndexModel> _logger;
        private readonly CustomerService _customerService;
        private readonly EncryptionService _encryptionService;

        public IndexModel(ILogger<IndexModel> logger, CustomerService customerService, EncryptionService encryptionService)
        {
            _logger = logger;
            _customerService = customerService;
            _encryptionService = encryptionService;
        }
        public List<Customer> CustomerList { get; set; } = new();

        public void OnGet()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
            {
                HttpContext.Session.SetString(SessionKeyName, "User");
                HttpContext.Session.SetString(SessionKeyEmail, "Blank");
            }

            var name = HttpContext.Session.GetString(SessionKeyName);
            var email = HttpContext.Session.GetInt32(SessionKeyEmail);

            _logger.LogInformation("Session Name: {Name}", name);
            _logger.LogInformation("Session Age: {Age}", email);
            CustomerList = _customerService.GetAll();
            foreach(var item in CustomerList)
            {
                var decryptCreditCard = _encryptionService.Decrypt(item.CreditCard);
                item.CreditCard = decryptCreditCard;
            }
        }
    }
}