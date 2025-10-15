using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GameServer.Models
{
    public class Friend
    {
        [Key]
        public int FriendshipId { get; set; }
        
        [Required]        
        public int PlayerId { get; set; }

        [Required]
        public int FriendPlayerId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";

        [StringLength(50)]
        public string? Nickname { get; set; }

        public bool IsFavorite { get; set; } = false;
        public bool CanSendGift { get; set; } = true;

        public DateTime? LastGiftSentDate { get; set; }

        public bool ShowOnlineStatus { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? AcceptedAt { get; set; }

        [ForeignKey("PlayerId")]
        public virtual Player? Player { get; set; }

        [ForeignKey("FriendPlayerId")]
        public virtual Player? FriendPlayer { get; set; }

    }
}
