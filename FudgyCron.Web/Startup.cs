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
            app.UseOwin(x => x.UseNancy());
        }
    }
}
