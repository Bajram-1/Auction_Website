using Auction_Website.BLL.IServices;
using Auction_Website.Common;
using Auction_Website.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using X.PagedList.Extensions;

namespace Auction_Website.UI.Controllers
{
    [Authorize(Roles = StaticDetails.Role_Admin)]
    public class AdminController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILoggerService _logger;

        public AdminController(UserManager<ApplicationUser> userManager, ILoggerService loggerService)
        {
            _userManager = userManager;
            _logger = loggerService;
        }

        [HttpGet]
        public IActionResult UserList(int? page)
        {
            try
            {
                int pageSize = 10;
                int pageNumber = page ?? 1;

                var users = _userManager.Users
                    .OrderBy(u => u.UserName)
                    .ToPagedList(pageNumber, pageSize);

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                TempData["error"] = "An error occurred while retrieving users.";
                return View("UserList");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUserStatus(string userId)
        {
            try
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

                _logger.LogInfo($"User {user.UserName} has been {(user.IsActive ? "activated" : "deactivated")}.");
                TempData["success"] = $"User {user.UserName} has been {(user.IsActive ? "activated" : "deactivated")}.";

                return RedirectToAction("UserList");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex);
                TempData["error"] = "Something went wrong. Please try again later.";
                return RedirectToAction("UserList");
            }
        }
    }
}