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
    public class RegisterModel : PageModel
    {

        private readonly CustomerService _customerService;
        private readonly IWebHostEnvironment _environment;
        private UserManager<IdentityUser> userManager { get; }
        private SignInManager<IdentityUser> signInManager { get; }
        private readonly CaptchaService _captchaService;
        private readonly EncryptionService _encryptionService;
        public const string SessionKeyName = "_Name";
        public const string SessionKeyEmail = "_Email";

        public RegisterModel(UserManager<IdentityUser> userManager, CustomerService customerService, IWebHostEnvironment environment, SignInManager<IdentityUser> signInManager, CaptchaService captchaService, EncryptionService encryptionService)
        {
            this.userManager = userManager;
            _customerService = customerService;
            _captchaService = captchaService;
            _environment = environment;
            this.signInManager = signInManager;
            _encryptionService = encryptionService;
        }

        [BindProperty]
        public Customer MyCustomer { get; set; } = new();
        [BindProperty]
        public IFormFile? Upload { get; set; }
        [BindProperty]
        public string TokenCaptcha { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {

            try
            {
                if (ModelState.IsValid)
                {
                    //Captcha Verification
                    var captchaValue = await _captchaService.VerifyToken(TokenCaptcha);
                    if (captchaValue == false)
                    {
                        return Redirect("/FailedCaptcha");
                    }

                    //Encoding Email input
                    var encodedemail = HttpUtility.HtmlEncode(MyCustomer.Email);
                    MyCustomer.Email = encodedemail;
                    Customer? customer = _customerService.GetCustomerByEmail(MyCustomer.Email);

                    //Encode AboutMe input
                    var encodedAboutMe = HttpUtility.HtmlEncode(MyCustomer.AboutMe);
                    MyCustomer.AboutMe = encodedAboutMe;


                    //Check if email exists
                    if (customer != null)
                    {
                        ModelState.AddModelError("MyCustomer.Email", "Email already exists.");
                        return Page();
                    }

                    //Save uploaded image
                    if (Upload != null)
                    {
                        if (Upload.Length > 2 * 1024 * 1024)
                        {
                            ModelState.AddModelError("Upload", "File size cannot exceed 2MB.");
                            return Page();
                        }

                        var uploadsFolder = "uploads";
                        var imageFile = Guid.NewGuid() + Path.GetExtension(Upload.FileName);
                        var imagePath = Path.Combine(_environment.ContentRootPath, "wwwroot", uploadsFolder, imageFile);
                        using var fileStream = new FileStream(imagePath, FileMode.Create);
                        await Upload.CopyToAsync(fileStream);
                        MyCustomer.ImageURL = string.Format("/{0}/{1}", uploadsFolder, imageFile);
                    }

                    //Start of Pass Hashing
                    string pwd = MyCustomer.PasswordHash.Trim();
                    //Generate random "salt"
                    RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
                    byte[] saltByte = new byte[8];
                    //Fills array of bytes with a cryptographically strong sequence of random values.
                    rng.GetBytes(saltByte);
                    MyCustomer.Salt = Convert.ToBase64String(saltByte);
                    SHA512Managed hashing = new SHA512Managed();
                    string pwdWithSalt = pwd + MyCustomer.Salt;
                    byte[] hashWithSalt = hashing.ComputeHash(Encoding.UTF8.GetBytes(pwdWithSalt));
                    MyCustomer.PasswordHash = Convert.ToBase64String(hashWithSalt);

                    //End of Pass Hashing

                    //Start of CC Encrypt
                    MyCustomer.CreditCard = _encryptionService.Encrypt(MyCustomer.CreditCard);
                    //End of CC Encrypt

                    var user = new IdentityUser()
                    {
                        UserName = MyCustomer.Email,
                        Email = MyCustomer.Email
                    };

                    var result = await userManager.CreateAsync(user, MyCustomer.PasswordHash);
                    _customerService.AddCustomer(MyCustomer);
                    TempData["FlashMessage.Type"] = "success";
                    TempData["FlashMessage.Text"] = string.Format("Customer {0} is added", MyCustomer.FName);
                    if (result.Succeeded)
                    {
                        await signInManager.SignInAsync(user, false);
                        if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionKeyName)))
                        {
                            HttpContext.Session.SetString(SessionKeyName, MyCustomer.FName);
                            HttpContext.Session.SetString(SessionKeyEmail, MyCustomer.Email);
                        }

                        var name = HttpContext.Session.GetString(SessionKeyName);
                        var email = HttpContext.Session.GetInt32(SessionKeyEmail);
                        return RedirectToPage("/Index");
                    }
                }
                return RedirectToPage("/Login");
            }
            catch
            {
                return Redirect("/Error");
            }
        }
    }
}
