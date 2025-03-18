using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using Auction_Website.DAL.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;

namespace Auction_Website.UI.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IUserEmailStore<ApplicationUser> _emailStore;
        private readonly IEmailSender _emailSender;
        private readonly ILogger<RegisterModel> _logger;

        public RegisterModel(
                    UserManager<ApplicationUser> userManager,
                    SignInManager<ApplicationUser> signInManager,
                    IUserEmailStore<ApplicationUser> emailStore,
                    IEmailSender emailSender,
                    ILogger<RegisterModel> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailStore = emailStore;
            _emailSender = emailSender;
            _logger = logger;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(19, MinimumLength = 4, ErrorMessage = "Username must be between 4 and 19 characters.")]
            [RegularExpression(@"^[A-Za-z0-9]+(?:[_\.][A-Za-z0-9]+)*$", ErrorMessage = "Username can only contain letters, numbers, underscores, and dots (not at the beginning or end).")]
            [Display(Name = "Username")]
            public string UserName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email Address")]
            [PersonalData]
            public string Email { get; set; }

            [Required]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "First name must be between 3 and 50 characters.")]
            [RegularExpression(@"^[A-Za-z]+(?:['-][A-Za-z]+)*$", ErrorMessage = "First name can only contain letters, apostrophes, or hyphens.")]
            [Display(Name = "First Name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(50, MinimumLength = 3, ErrorMessage = "Last name must be between 3 and 50 characters.")]
            [RegularExpression(@"^[A-Za-z]+(?:['-][A-Za-z]+)*(?: [A-Za-z]+(?:['-][A-Za-z]+)*)*$", ErrorMessage = "Last name can only contain letters, apostrophes, hyphens, and spaces between words.")]
            [Display(Name = "Last Name")]
            public string LastName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The password must be at least {2} characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm Password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(returnUrl) && !Url.IsLocalUrl(returnUrl))
            {
                TempData["error"] = "Invalid URL entered. Redirecting to Register.";
                return RedirectToPage("/Identity/Account/Register");
            }

            ReturnUrl = returnUrl;
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            if (ModelState.IsValid)
            {
                var existingUser = await _userManager.FindByNameAsync(Input.UserName);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Input.UserName", "This username is already taken.");
                    return Page();
                }

                var existingUserByEmail = await _userManager.Users.Where(u => u.Email == Input.Email).FirstOrDefaultAsync();
                if (existingUserByEmail != null)
                {
                    ModelState.AddModelError("Input.Email", "An account with this email already exists.");
                    return Page();
                }

                var user = new ApplicationUser
                {
                    UserName = Input.UserName,
                    Email = Input.Email,
                    FirstName = Input.FirstName,
                    LastName = Input.LastName,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User registered successfully: {Email}", user.Email);

                    var createdUser = await _userManager.FindByEmailAsync(user.Email);
                    if (createdUser == null)
                    {
                        _logger.LogError("User not found immediately after creation: {Email}", user.Email);
                        TempData["error"] = "An error occurred. Please try registering again.";
                        return Page();
                    }

                    await _userManager.AddClaimAsync(createdUser, new System.Security.Claims.Claim("FirstName", Input.FirstName));
                    await _userManager.AddClaimAsync(createdUser, new System.Security.Claims.Claim("LastName", Input.LastName));

                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(createdUser);
                    token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                    var confirmationLink = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = createdUser.Id, token },
                        protocol: Request.Scheme);

                    _logger.LogInformation("Sending email confirmation to {Email}. Confirmation link: {Link}", user.Email, confirmationLink);

                    await _emailSender.SendEmailAsync(Input.Email, "Confirm Your Email",
                        $"<p>Please confirm your account by <a href='{HtmlEncoder.Default.Encode(confirmationLink)}'>clicking here</a>.</p>");

                    TempData["success"] = "Registration successful! Please check your email to confirm your account.";
                    return RedirectToPage("./Login");
                }

                foreach (var error in result.Errors)
                {
                    TempData["error"] = error.Description;
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return Page();
        }
    }
}