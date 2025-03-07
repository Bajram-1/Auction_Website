using Auction_Website.BLL.DTO.Requests;
using Auction_Website.BLL.DTO.ViewModels;
using Auction_Website.BLL.IServices;
using Auction_Website.BLL.Services;
using Auction_Website.DAL;
using Auction_Website.DAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeZoneConverter;

namespace Auction_Website.UI.Controllers
{
    [Authorize]
    public class AuctionController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IAuctionService _auctionService;
        private readonly ILoggerService _loggerService;
        public AuctionController(UserManager<ApplicationUser> userManager, IAuctionService auctionService, ILoggerService loggerService)
        {
            _userManager = userManager;
            _auctionService = auctionService;
            _loggerService = loggerService;
        }

        [HttpGet]
        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                await _auctionService.CloseExpiredAuctions();
                var auctions = await _auctionService.GetActiveAuctionsAsync();

                var pagedAuctions = auctions
                    .OrderBy(a => a.EndTime)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                var user = await _userManager.GetUserAsync(User);
                ViewBag.WalletBalance = user?.WalletBalance ?? 0;
                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = (int)Math.Ceiling(auctions.Count() / (double)pageSize);

                return View(pagedAuctions);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex);
                ViewBag.ErrorMessage = "An error occurred while loading auctions. Please try again later.";
                return View(new List<AuctionAddEditRequestModel>());
            }
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AuctionAddEditRequestModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["error"] = "Please correct the errors and try again.";
                return View(model);
            }

            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                var albaniaTimeZone = TZConvert.GetTimeZoneInfo("Central European Standard Time");
                var albaniaTimeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, albaniaTimeZone);

                var auction = new Auction
                {
                    Title = model.Title,
                    StartingPrice = model.StartingPrice,
                    Description = model.Description,
                    StartTime = albaniaTimeNow,
                    EndTime = model.EndTime,
                    CreatedByUserId = user.Id,
                    IsClosed = false
                };

                await _auctionService.CreateAuctionAsync(auction);

                TempData["success"] = "Auction created successfully!";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex);
                TempData["error"] = "An error occurred while creating the auction. Please try again later.";
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var auction = await _auctionService.GetAuctionByIdAsync(id);
                if (auction == null)
                {
                    TempData["error"] = "Auction not found.";
                    return RedirectToAction("Index");
                }

                var viewModel = new AuctionDetailsViewModel
                {
                    AuctionId = auction.AuctionId,
                    Title = auction.Title,
                    Description = auction.Description,
                    StartingPrice = auction.StartingPrice,
                    EndTime = auction.EndTime,
                    IsClosed = auction.IsClosed,
                    SellerName = auction.CreatedByUser != null
                                    ? $"{auction.CreatedByUser.FirstName} {auction.CreatedByUser.LastName}"
                                    : "Unknown",
                    CreatedByUserId = auction.CreatedByUserId,
                    CurrentHighestBid = auction.Bids.Any() ? auction.Bids.Max(b => b.Amount) : auction.StartingPrice,
                    Bids = auction.Bids.Select(b => new BidViewModel
                    {
                        Amount = b.Amount,
                        BidderName = b.User != null
                                        ? $"{b.User.FirstName} {b.User.LastName}"
                                        : "Anonymous",
                        TimePlaced = b.TimePlaced
                    }).ToList()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex);
                TempData["error"] = "An error occurred while loading the auction details. Please try again later.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var userId = _userManager.GetUserId(User);
                var success = await _auctionService.DeleteAuctionAsync(id, userId);

                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex);
                return Json(new { success = false, error = "An error occurred while deleting the auction." });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceBid(int auctionId, decimal bidAmount)
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null)
                {
                    TempData["error"] = "User not found.";
                    return RedirectToAction("Index");
                }

                bool isBidSuccessful = await _auctionService.PlaceBidAsync(auctionId, user.Id, bidAmount);

                if (!isBidSuccessful)
                {
                    TempData["error"] = "Bid failed. Ensure the bid is higher than the current highest bid and within your wallet balance.";
                }
                else
                {
                    TempData["success"] = "Bid placed successfully!";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex);
                TempData["error"] = "An error occurred while placing your bid. Please try again later.";
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Close(int id)
        {
            try
            {
                var success = await _auctionService.CloseAuctionAsync(id);

                if (!success)
                {
                    TempData["error"] = "Failed to close the auction.";
                }
                else
                {
                    TempData["success"] = "Auction closed and funds transferred successfully.";
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex);
                TempData["error"] = "An error occurred while closing the auction. Please try again later.";
                return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetWalletBalance()
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    return Json("0.00");
                }

                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
                return Json(user?.WalletBalance.ToString("0.00") ?? "0.00");
            }
            catch (Exception ex)
            {
                _loggerService.LogError(ex);
                return Json("0.00");
            }
        }
    }
}