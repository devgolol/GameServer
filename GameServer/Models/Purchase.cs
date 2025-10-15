using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models
{
    public class Purchase
    {
        [Key]
        public int PurchaseId { get; set; }

        [Required]
        public int PlayerId { get; set; }
        [Required]
        public int ShopItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string TransactionId { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Platform { get; set; } = string.Empty;

        [Required]
        [Column(TypeName ="decimal(10,2")]
        public decimal PaidAmount { get; set; }

        [Required]
        [StringLength(10)]
        public string Currency { get; set; } = string.Empty;

        [Required]
        public int Quantity { get; set; } = 1;

        [Required]
        public string Status { get; set; } = "Pending";
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedDate { get; set; }

        [StringLength(500)]
        public string? ErrorMessage { get; set; }

        [StringLength(1000)]
        public string? ReceiptData { get; set; }

        public bool IsVerified { get; set; }

        [StringLength(100)]
        public string? RefundReason { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player? Player { get; set; }

        [ForeignKey("ShopItemId")]
        public virtual Shop? ShopItem { get; set; }

        public bool IsFirstPurchase { get; set; } = false;
        public int BonusQuantity { get; set; } = 0;
    }
}
