using Microsoft.AspNetCore.Mvc;

namespace Auction_Website.UI.Controllers
{
    public class ErrorController : Controller
    {
        public IActionResult InvalidUrl(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }
    }
}
