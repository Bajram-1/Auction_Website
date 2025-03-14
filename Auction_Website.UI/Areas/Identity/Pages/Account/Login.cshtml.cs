using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Auction_Website.BLL.IServices;
using Auction_Website.DAL.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace Auction_Website.UI.Areas.Identity.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _logger;

        public LoginModel(SignInManager<ApplicationUser> signInManager, UserManager<ApplicationUser> userManager, ILoggerService logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(19, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 19 characters.")]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl) && !Url.IsLocalUrl(returnUrl))
            {
                TempData["error"] = "URL e pavlefshme. Ju lutemi provoni përsëri.";
                return RedirectToPage("/Identity/Account/Error");
            }

            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/Auction/Index");

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                ApplicationUser user = await _userManager.FindByNameAsync(Input.Username);

                if (user == null && Input.Username.Contains("@"))
                {
                    user = await _userManager.FindByEmailAsync(Input.Username);
                }

                if (user != null)
                {
                    if (!user.IsActive)
                    {
                        ModelState.AddModelError("", "Your account has been deactivated. Please contact support.");
                        return Page();
                    }

                    var result = await _signInManager.PasswordSignInAsync(user.UserName, Input.Password, Input.RememberMe, lockoutOnFailure: false);

                    if (result.IsNotAllowed)
                    {
                        TempData["error"] = "You have not verified your email account. Please verify it!";
                    }
                    else if (result.Succeeded)
                    {
                        _logger.LogInfo("User logged in.");
                        TempData["success"] = "Login successful! Welcome back.";
                        return LocalRedirect(returnUrl);
                    }
                    else if (result.RequiresTwoFactor)
                    {
                        return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, Input.RememberMe });
                    }
                    else if (result.IsLockedOut)
                    {
                        _logger.LogWarning("User account locked out.");
                        TempData["error"] = "Your account has been locked out due to multiple failed login attempts.";
                        return RedirectToPage("./Lockout");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                        TempData["error"] = "Invalid email or password.";
                        return Page();
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    TempData["error"] = "Invalid email or password.";
                    return Page();
                }
            }

            return Page();
        }
    }
}