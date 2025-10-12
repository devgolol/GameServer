using System.ComponentModel.DataAnnotations;

namespace GameServer.Models
{
    public class GameSession
    {
        [Key]
        public int SessionId { get; set; }
        public int PlayerId { get; set; }
        public string GameType { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Score { get; set; }
        public Player Player { get; set; }
    }
}
