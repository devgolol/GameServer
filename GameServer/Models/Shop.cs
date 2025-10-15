using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace GameServer.Models
{
    public class Shop
    {
        [Key]
        public int ShopItemId { get; set; }

        [Required]
        [StringLength(100)]
        public string ItemName { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public string ItemType { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Price { get; set; }

        [Required]
        public int InGameCurrency { get; set; }

        [Required]
        public string CurrencyType { get; set; } = "USD";
        public int Quantity { get; set; } = 1;
        public bool IsLimited { get; set; } = false;

        public int LimitQuantity { get; set; } = 0;

        public bool IsActive { get; set; } = true;

        public DateTime? SaleStartDate { get; set; }
        public DateTime? SaleEndDate { get; set; }

        [StringLength(500)]
        public string? ImageUrl { get; set; }
        public int SortOrder { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public bool IsFeatured { get; set; } = false;
        public int DiscountPercent { get; set; } = 0;
        public bool IsPopular { get; set; } = false;
        [StringLength(50)]
        public string? Category { get; set; }
    }
}
