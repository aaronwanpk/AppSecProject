using Application_Security_Project_v2.Models;
using Application_Security_Project_v2.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Application_Security_Project_v2.Pages
{

    public class DeleteModel : PageModel
    {
        private readonly CustomerService _customerService;
        private readonly SignInManager<IdentityUser> signInManager;
        private UserManager<IdentityUser> userManager { get; }
        public List<Customer> CustomerList { get; set; } = new();
        public DeleteModel(CustomerService customerService, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
        {
            _customerService = customerService;
            this.userManager = userManager;
            this.signInManager = signInManager;
        }
        public IActionResult OnGet()
        {
            CustomerList = _customerService.GetAll();
            foreach(var entry in CustomerList)
            {
                var user = new IdentityUser()
                {
                    UserName = entry.Email,
                    Email = entry.Email
                };
                var userFound = userManager.DeleteAsync(user);
                System.Threading.Thread.Sleep(4000);
            }

            System.Threading.Thread.Sleep(4000);
            signInManager.SignOutAsync();
            return Redirect("/Login");

        }
    }
}
