using System;
using Nancy;

namespace FudgyCron.Web
{
    public class BaseModule : NancyModule
    {
        public BaseModule() : base("/")
        {
            var response = $"NancyFX Hello @ {DateTime.UtcNow} ... DATABSE_URL from ENV: {System.Environment.GetEnvironmentVariable("DATABASE_URL")}";
            Get("/", args => response);
        }
    }
}
