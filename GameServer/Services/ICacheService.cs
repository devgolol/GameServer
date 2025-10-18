namespace GameServer.Services
{
    public interface ICacheService
    {
        //기본 캐시 operations
        Task<T?> GetAsync<T>(string key) where T : class;
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class;
        Task<bool> RemoveAsync(string key);
        Task<bool> ExistsAsync(string key);

        //게임 특화 기능들
        Task<List<T>> GetLeaderboardAsync<T>(string leaderboardType, int count = 10) where T : class;
        Task SetLeaderboardAsync<T>(string leaderboardType, List<T> data, TimeSpan? expiry = null) where T : class;

        Task<T?> GetPlayerSessionAsync<T>(int playerId) where T : class;
        Task SetPlayerSessionAsync<T>(int playerId, T sessionData, TimeSpan? expiry = null) where T : class;

        Task InvalidatePlayerCacheAsync(int playerId);
        Task InvalidateLeaderboardCacheAsync();
    }
}
