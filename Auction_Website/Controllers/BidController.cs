using Auction_Website.BLL.DTO.Requests;
using Auction_Website.BLL.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Auction_Website.UI.Controllers
{
    [Authorize]
    public class BidController : Controller
    {
        private readonly IBidService _bidService;
        private readonly ILoggerService _loggerService;

        public BidController(IBidService bidService, ILoggerService loggerService)
        {
            _bidService = bidService;
            _loggerService = loggerService;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBid(BidAddRequestModel model)
        {
            try
            {
                var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(userId))
                {
                    TempData["error"] = "You must be logged in to place a bid.";
                    return RedirectToAction("Index", "Auction");
                }

                var result = await _bidService.PlaceBidAsync(model, userId);

                if (!result)
                {
                    TempData["error"] = "Bid placement failed. Check balance or bid amount.";
                    return RedirectToAction("Details", "Auction", new { id = model.AuctionId });
                }

                TempData["success"] = "Bid placed successfully!";
                return RedirectToAction("Details", "Auction", new { id = model.AuctionId });
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex);
                TempData["error"] = "An error occurred while placing your bid. Please try again later.";
                return RedirectToAction("Details", "Auction", new { id = model.AuctionId });
            }
        }
    }
}