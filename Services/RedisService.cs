using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

namespace GoogleFormsClone.Services
{
    public class RedisService
    {
        private readonly IDatabase _db;
        private readonly ConnectionMultiplexer _redis;

        public RedisService(IConfiguration configuration)
        {
            var host = configuration["ConnectionStrings:RedisHost"];
            var port = int.Parse(configuration["ConnectionStrings:RedisPort"] ?? "6379");
            var user = configuration["ConnectionStrings:RedisUser"];
            var password = configuration["ConnectionStrings:RedisPassword"];

            var options = new ConfigurationOptions
            {
                EndPoints = { { host!, port } },
                User = user,
                Password = password,
                Ssl = true
            };

            _redis = ConnectionMultiplexer.Connect(options);
            _db = _redis.GetDatabase();
        }

        public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
        {
            var json = JsonSerializer.Serialize(value);
            await _db.StringSetAsync(key, json, expiry);
        }

        public async Task<T?> GetAsync<T>(string key)
        {
            var json = await _db.StringGetAsync(key);
            if (json.IsNullOrEmpty) return default;
            return JsonSerializer.Deserialize<T>(json!);
        }
    }
}
