using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Auction_Website.DAL.Entities
{
    public class Transfer
    {
        [Key]
        public int TransferId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string FromUserId { get; set; }

        [Required]
        public string ToUserId { get; set; }

        [Required]
        public DateTime TransferDate { get; set; } = DateTime.UtcNow;
        [StringLength(200)]
        public string Reason { get; set; }

        [ForeignKey("FromUserId")]
        public virtual ApplicationUser FromUser { get; set; }

        [ForeignKey("ToUserId")]
        public virtual ApplicationUser ToUser { get; set; }
    }
}