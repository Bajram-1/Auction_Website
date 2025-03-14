using Auction_Website.Common;
using Auction_Website.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Auction_Website.UI.Controllers
{
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> UserList()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["error"] = "User not found.";
                return RedirectToAction("UserList");
            }

            if (user.Email == "admin@auction.com")
            {
                TempData["error"] = "Cannot deactivate the main admin account.";
                return RedirectToAction("UserList");
            }

            user.IsActive = !user.IsActive;
            await _userManager.UpdateAsync(user);

            TempData["success"] = $"User {user.UserName} has been {(user.IsActive ? "activated" : "deactivated")}.";
            return RedirectToAction("UserList");
        }
    }
}