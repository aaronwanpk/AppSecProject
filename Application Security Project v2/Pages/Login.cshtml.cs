using Application_Security_Project_v2.Models;
using Application_Security_Project_v2.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Application_Security_Project_v2.Pages
{
	[ValidateAntiForgeryToken]
	public class LoginModel : PageModel
	{
		[BindProperty]
		public Login LoginInfo { get; set; }
		[BindProperty]
		public string TokenCaptcha { get; set; }
		public Audit AuditInfo { get; set; }
        public const string SessionKeyName = "_Name";
        public const string SessionKeyEmail = "_Email";

        private readonly CustomerService _customerService;

		private readonly SignInManager<IdentityUser> signInManager;
		private readonly CaptchaService _captchaService;
		private readonly AuditService _auditService;
		public LoginModel(SignInManager<IdentityUser> signInManager, CustomerService customerService, CaptchaService captchaService, AuditService auditService)
		{
			this.signInManager = signInManager;
			_customerService = customerService;
			_captchaService = captchaService;
			_auditService = auditService;

		}
		public void OnGet()
		{
		}

		public async Task<IActionResult> OnPostAsync()
		{
			if (ModelState.IsValid)
			{
				//Captcha Verification
				var captchaValue = await _captchaService.VerifyToken(TokenCaptcha);
				if (captchaValue == false)
				{
					return Redirect("/FailedCaptcha");
				}
				var encodedemail = HttpUtility.HtmlEncode(LoginInfo.Email);
				LoginInfo.Email = encodedemail;
				Customer? customer = _customerService.GetCustomerByEmail(LoginInfo.Email);
				if (customer == null)
				{
					ModelState.AddModelError("", "Username or Password incorrect");
					return Page();
				}
				//Start of Pass Hashing
				string pwd = LoginInfo.PasswordHash.Trim();
				//Generate random "salt"
				SHA512Managed hashing = new SHA512Managed();
				string pwdWithSalt = pwd + customer.Salt;
				byte[] hashbrown = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
				LoginInfo.PasswordHash = Convert.ToBase64String(hashbrown);

				//End of Pass Hashing

				var identityResult = await signInManager.PasswordSignInAsync(LoginInfo.Email, LoginInfo.PasswordHash,
				LoginInfo.RememberMe, true);
                if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
                {
                    HttpContext.Session.SetString(SessionKeyName, customer.FName);
                    HttpContext.Session.SetString(SessionKeyEmail, LoginInfo.Email);
                }

                var name = HttpContext.Session.GetString(SessionKeyName);
                var email = HttpContext.Session.GetInt32(SessionKeyEmail);

                AuditInfo.email = LoginInfo.Email;
				AuditInfo.description = "Logged In";
				_auditService.AddAuditEntry(AuditInfo);

                if (identityResult.Succeeded)
				{
					return RedirectToPage("Index");
				}
				else if (identityResult.IsLockedOut)
				{
					ModelState.AddModelError("", "Account has been locked out");
					return Page();
				}
				ModelState.AddModelError("", "Username or Password incorrect");
				return Page();
			}
			return Page();
		}
	}
}
