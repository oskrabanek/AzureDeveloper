using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Redis.Models;
using Redis.Models.Home;
using StackExchange.Redis;

namespace Redis.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<IActionResult> Index()
        {
            await HttpContext.Session.LoadAsync();

            var sessionstartTime = HttpContext.Session.GetString("storedSessionStartTime");

            if (sessionstartTime == null)
            {
                sessionstartTime = DateTime.UtcNow.ToLongTimeString();
                HttpContext.Session.SetString("storedSessionStartTime", sessionstartTime);
                await HttpContext.Session.CommitAsync();
            }

            var model = new IndexModel
            {
                SessionStartTime = sessionstartTime,
                CacheItems = GetAllRedisCacheItems()
            };

            return View(model);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IEnumerable<(RedisKey Key, RedisValue? Value)> GetAllRedisCacheItems()
        {
            using (ConnectionMultiplexer connection = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("RedisCache")))
            {
                IDatabase db = connection.GetDatabase();

                var endPoint = connection.GetEndPoints().First();
                var server = connection.GetServer(endPoint);
                var keys = server.Keys().ToList();
                return keys.Select(k =>
                    {
                        try
                        {
                            return (Key: k, Value: db.StringGet(k));
                        }
                        catch (RedisServerException)
                        {
                            return (Key: k, Value: (RedisValue?)null);
                        }
                    }).ToList();
            }
        }
    }
}
