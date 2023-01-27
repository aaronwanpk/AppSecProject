using Application_Security_Project_v2.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Application_Security_Project_v2.Models;

namespace Application_Security_Project_v2.Pages
{
    public class LogoutModel : PageModel
    {
		private readonly SignInManager<IdentityUser> signInManager;
        private readonly AuditService _auditService;
        public Audit AuditInfo { get; set; }
        public LogoutModel(SignInManager<IdentityUser> signInManager, AuditService auditService)
        {
            this.signInManager = signInManager;
            _auditService = auditService;
        }
        public void OnGet()
        {
        }

		public async Task<IActionResult> OnPostLogoutAsync()
		{
			await signInManager.SignOutAsync();
            AuditInfo.email = HttpContext.Session.GetString("_Name");
			AuditInfo.description = "Logged Out";
            _auditService.AddAuditEntry(AuditInfo);
            return RedirectToPage("Login");
		}
		public async Task<IActionResult> OnPostDontLogoutAsync()
		{
			return RedirectToPage("Index");
		}
	}
}
