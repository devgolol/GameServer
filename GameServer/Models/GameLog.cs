using System.ComponentModel.DataAnnotations;

namespace GameServer.Models
{
    public class GameLog
    {
        [Key]
        public int LogId { get; set; }
        public int PlayerId { get; set; }

        [Required]
        [StringLength(100)]
        public string Action { get; set; }
        public string Details { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public string IpAddress { get; set; }

        public Player Player { get; set; }
    }
}
