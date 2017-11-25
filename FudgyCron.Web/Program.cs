using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FudgyCron.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var port = System.Environment.GetEnvironmentVariable("PORT") ?? "5000";
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:{port}")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
