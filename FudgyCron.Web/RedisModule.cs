using System;
using System.Collections.Generic;
using Nancy;

namespace FudgyCron.Web
{
    public class RedisModule : NancyModule
    {
        public RedisModule() : base("/redis")
        {
            // this is very bad REST, I know
            Get("/{key}/{value}", args =>
            {
                var key = args.key.ToString();
                var value = args.value.ToString();

                // https://github.com/redis/redis-rb/blob/master/lib/redis/client.rb#L408
                // https://msdn.microsoft.com/en-us/library/system.uri(v=vs.110).aspx
                // https://stackexchange.github.io/StackExchange.Redis/Configuration
                var redisUrlString = Environment.GetEnvironmentVariable("REDIS_URL");
                Console.WriteLine($"REDIS_URL from ENV: {redisUrlString}");
                var redisUri = new Uri(redisUrlString);
                var userInfo = redisUri.UserInfo.Split(':');
                Console.WriteLine($"parsed redisUri: {redisUri} + {string.Join(", ", userInfo)}");
                var redisConnString = $"{redisUri.Host}:{redisUri.Port},password={userInfo[1]}";
                Console.WriteLine($"constructed redisConnString: {redisConnString}");

                var redisConn = StackExchange.Redis.ConnectionMultiplexer.Connect(redisConnString);
                var redisDb = redisConn.GetDatabase();
                redisDb.StringSet(key, value);
                var dict = new Dictionary<string, string>
                {
                    {args.key, redisDb.StringGet(key)},
                    {"ticks", DateTime.UtcNow.Ticks.ToString()},
                };
                
                // https://github.com/NancyFx/Nancy/wiki/Content-Negotiation
                var response = Response.AsJson(dict);
                response.ContentType = "application/json";
                return response;
            });
        }
    }
}
