using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace GameServer.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDistributedCache _distributedCache;
        private readonly ILogger<RedisCacheService> _logger;

        //캐시 키 프리픽스
        private const string LEADERBOARD_PREFIX = "leaderboard:";
        private const string PLAYER_SESSION_PREFIX = "player_session:";
        private const string SHOP_PREFIX = "shop:";
        private const string FRIEND_PREFIX = "friend:";

        public RedisCacheService(IDistributedCache distributedCache, ILogger<RedisCacheService> logger)
        {
            _distributedCache = distributedCache;
            _logger = logger;
        }

        public async Task<T?> GetAsync<T>(string key) where T : class
        {
            try
            {
                var cacheValue = await _distributedCache.GetStringAsync(key);
                if (string.IsNullOrEmpty(cacheValue))
                    return null;

                return JsonSerializer.Deserialize<T>(cacheValue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting cache for key: {key}", key);
                return null;
            }
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null) where T : class
        {
            try
            {
                var options = new DistributedCacheEntryOptions();
                if (expiry.HasValue)
                    options.SetAbsoluteExpiration(expiry.Value);
                else
                    options.SetAbsoluteExpiration(TimeSpan.FromHours(1));

                var serializedValue = JsonSerializer.Serialize(value);
                await _distributedCache.SetStringAsync(key, serializedValue, options);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error setting cache for key: {key}", key);
            }
        }

        public async Task<bool> RemoveAsync(string key)
        {
            try
            {
                await _distributedCache.RemoveAsync(key);
                _logger.LogInformation($"Cache removed for key: {key}", key);
                return true;
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, $"Error removing cache for key: {key}", key);
                return false;
            }
        }

        public async Task<bool> ExistsAsync(string key)
        {
            try
            {
                var value = await _distributedCache.GetStringAsync(key);
                return !string.IsNullOrEmpty(value);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error checking cache existence for key: {key}", key);
                return false;
            }
        }

        //리더보드 캐시
        public async Task<List<T>> GetLeaderboardAsync<T>(string leaderboardType, int count = 10) where T : class
        {
            var key = $"{LEADERBOARD_PREFIX}{leaderboardType}:top{count}";
            var result = await GetAsync<List<T>>(key);
            return result ?? new List<T>();
        }

        public async Task SetLeaderboardAsync<T>(string leaderboardType, List<T> data, TimeSpan? expiry = null) where T : class
        {
            var key = $"{LEADERBOARD_PREFIX}{leaderboardType}:top{data.Count}";
            await SetAsync(key, data, expiry ?? TimeSpan.FromMinutes(10));
        }

        //플레이어 세션 캐시
        public async Task<T?> GetPlayerSessionAsync<T>(int playerId) where T : class
        {
            var key = $"{PLAYER_SESSION_PREFIX}{playerId}";
            return await GetAsync<T>(key);
        }

        public async Task SetPlayerSessionAsync<T>(int playerId, T sessionData, TimeSpan? expiry = null) where T : class
        {
            var key = $"{PLAYER_SESSION_PREFIX}{playerId}";
            await SetAsync(key, sessionData, expiry ?? TimeSpan.FromMinutes(30));
        }

        //캐시무효화
        public async Task InvalidatePlayerCacheAsync(int playerId)
        {
            var sessionKey = $"{PLAYER_SESSION_PREFIX}{playerId}";
            var friendKey = $"{FRIEND_PREFIX}{playerId}";

            await RemoveAsync(sessionKey);
            await RemoveAsync(friendKey);

            _logger.LogInformation("Player cache invalidated for player: {PlayerId}", playerId);
        }

        public async Task InvalidateLeaderboardCacheAsync()
        {
            var leaderboardTypes = new[] { "level", "gold", "score" };
            var counts = new[] { 10, 20, 50 };

            foreach (var type in leaderboardTypes)
            {
                foreach(var count in counts)
                {
                    var key = $"{LEADERBOARD_PREFIX}{type}:top{count}";
                    await RemoveAsync(key);
                }
            }

            _logger.LogInformation("Leaderboard cache invalidated");
        }

    }
}
