using GoogleFormsClone.Services;
using Microsoft.AspNetCore.Mvc;

namespace GoogleFormsClone.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RedisTestController : ControllerBase
    {
        private readonly RedisService _redis;

        public RedisTestController(RedisService redis)
        {
            _redis = redis;
        }

        // ✅ Patikrina, ar Redis veikia
        [HttpGet("ping")]
        public IActionResult Ping()
        {
            return Ok("Redis test endpoint veikia! 🚀");
        }

        // ✅ Įrašo testinę reikšmę į Redis
        [HttpGet("set")]
        public async Task<IActionResult> Set()
        {
            await _redis.SetAsync("test-key", new { Message = "Sveikas iš Redis Cloud!" }, TimeSpan.FromMinutes(5));
            return Ok("Reikšmė įrašyta į Redis Cloud ✅");
        }

        // ✅ Nuskaito testinę reikšmę iš Redis
        [HttpGet("get")]
        public async Task<IActionResult> Get()
        {
            var result = await _redis.GetAsync<object>("test-key");
            if (result == null)
                return NotFound("Nieko nerasta Redis Cloud ❌");

            return Ok(result);
        }
    }
}
