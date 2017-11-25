using System;
using DotNetEnv;
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace FudgyCron.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            Env.Load();
            Console.WriteLine($"DATABSE_URL from ENV: {Environment.GetEnvironmentVariable("DATABASE_URL")}");
            app.UseOwin(x => x.UseNancy());
        }
    }
}
