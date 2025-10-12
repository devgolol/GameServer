using System.ComponentModel.DataAnnotations;

namespace GameServer.Models
{
    public class UserReward
    {
        [Key]
        public int RewardId { get; set; }
        public int PlayerId { get; set; }

        [Required]
        [StringLength(50)]
        public string RewardType { get; set; }

        [Required]
        [StringLength(100)]
        public string RewardName { get; set; }
        public int Quantity { get; set; } = 1;
        public DateTime AcquiredAt { get; set; } = DateTime.UtcNow;
        public bool IsCollected { get; set; } = false;
        public string Description { get; set; }
        public Player Player { get; set; }
    }
}
