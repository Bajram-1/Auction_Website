using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;

namespace Auction_Website.UI.Areas.Identity.Pages.Account
{
    public class ErrorModel : PageModel
    {
        public IActionResult OnGet()
        {
            if (TempData["error"] == null)
            {
                TempData["error"] = "An unexpected error occurred.";
            }
            return Page();
        }
    }
}