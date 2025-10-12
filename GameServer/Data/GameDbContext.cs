using GameServer.Models;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;

namespace GameServer.Data
{
    public class GameDbContext : DbContext
    {
        public GameDbContext(DbContextOptions<GameDbContext> options) : base(options)
        {

        }

        // 데이터베이스 테이블들
        public DbSet<Player> Players { get; set; }
        public DbSet<GameSession> GameSessions { get; set; }
        public DbSet<GameLog> GameLogs { get; set; }
        public DbSet<UserReward> UserRewards { get; set; }


        //테이블명을 바꾸기
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //테이블 설정
            modelBuilder.Entity<Player>().ToTable("players");
            modelBuilder.Entity<GameSession>().ToTable("game_sessions");
            modelBuilder.Entity<GameLog>().ToTable("game_logs");
            modelBuilder.Entity<UserReward>().ToTable("user_reward");
            
        }
    }
}
