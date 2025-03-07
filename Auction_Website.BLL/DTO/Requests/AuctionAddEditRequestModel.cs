using Auction_Website.BLL.DTO.ViewModels;
using System.ComponentModel.DataAnnotations;
using TimeZoneConverter;

namespace Auction_Website.BLL.DTO.Requests
{
    public class AuctionAddEditRequestModel
    {
        [Required]
        public int AuctionId { get; set; }

        [Required(ErrorMessage = "Product name is required.")]
        [MinLength(4, ErrorMessage = "Product name must be greater than 3 characters long.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Starting bid is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Starting bid must be higher than 0.")]
        [DataType(DataType.Currency)]
        public decimal StartingPrice { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [MinLength(11, ErrorMessage = "Description must be greater than 10 characters long.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        [DataType(DataType.DateTime)]
        [FutureDate(ErrorMessage = "End date must be in the future.")]
        public DateTime EndTime { get; set; }
        public string? CreatedByUserId { get; set; }
        public string? SellerName { get; set; }
        public decimal CurrentHighestBid { get; set; }
        public bool? IsClosed { get; set; }
        public TimeSpan TimeRemaining
        {
            get
            {
                var albaniaTimeZone = TZConvert.GetTimeZoneInfo("Central European Standard Time");
                var localNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, albaniaTimeZone);
                return EndTime - localNow;
            }
        }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is DateTime dateTime)
            {
                if (dateTime <= DateTime.UtcNow)
                {
                    return new ValidationResult(ErrorMessage ?? "The date must be in the future.");
                }
            }
            return ValidationResult.Success;
        }
    }
}