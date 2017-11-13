
mkdir FudgyCron
cd FudgyCron

wget https://raw.githubusercontent.com/github/gitignore/master/VisualStudio.gitignore -O .gitignore

dotnet new sln -n FudgyCron

dotnet new web -f netcoreapp2.0 -n FudgyCron.Web
rm -rf FudgyCron.Web/wwwroot  # remove its ItemGroup from .csproj manually
dotnet sln add FudgyCron.Web/FudgyCron.Web.csproj

dotnet new xunit -f netcoreapp2.0 -n FudgyCron.Tests
dotnet add ./FudgyCron.Tests package xunit --version 2.3.1  # update the version
dotnet add ./FudgyCron.Tests package FakeItEasy --version 4.2.0
dotnet add ./FudgyCron.Tests reference ./FudgyCron.Web/FudgyCron.Web.csproj

sed -i '/PackageReference Include="Microsoft.NET.Test.Sdk"/ d' FudgyCron.Tests/FudgyCron.Tests.csproj
sed -i 's/<PackageReference Include="xunit.runner.visualstudio" Version="2.2.0" \/>/<DotNetCliToolReference Include="dotnet-xunit" Version="2.3.1" \/>/' FudgyCron.Tests/FudgyCron.Tests.csproj

dotnet add ./FudgyCron.Web package DotNetEnv --version 1.1.0
dotnet add ./FudgyCron.Web package Npgsql --version 3.2.5
dotnet add ./FudgyCron.Web package Dapper --version 1.50.2
dotnet add ./FudgyCron.Web package StackExchange.Redis --version 1.2.6
dotnet add ./FudgyCron.Web package Confluent.Kafka --version 0.11.2
dotnet add ./FudgyCron.Web package Quartz --version 3.0.0-beta1
dotnet add ./FudgyCron.Web package Newtonsoft.Json --version 10.0.3
dotnet add ./FudgyCron.Web package CronExpressionDescriptor --version 2.0.2

dotnet remove ./FudgyCron.Web package Microsoft.AspNetCore.All
dotnet add ./FudgyCron.Web package Microsoft.AspNetCore.Hosting --version 2.0.0
dotnet add ./FudgyCron.Web package Microsoft.AspNetCore.Owin --version 2.0.0
dotnet add ./FudgyCron.Web package Microsoft.AspNetCore.Server.Kestrel --version 2.0.0
dotnet add ./FudgyCron.Web package Microsoft.Extensions.CommandLineUtils --version 1.1.1
dotnet add ./FudgyCron.Web package Nancy -v 2.0.0-clienteastwood




cat <<EOF > FudgyCron.Web/Startup.cs
using Microsoft.AspNetCore.Builder;
using Nancy.Owin;

namespace FudgyCron.Web
{
    public class Startup
    {
        public void Configure(IApplicationBuilder app)
        {
            app.UseOwin(x => x.UseNancy());
        }
    }
}
EOF

cat <<EOF > FudgyCron.Web/Program.cs
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace FudgyCron.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls($"http://*:5000")
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .Build();

            host.Run();
        }
    }
}
EOF

cat <<EOF > FudgyCron.Web/BaseModule.cs
using System;
using Nancy;

namespace FudgyCron.Web
{
    public class BaseModule : NancyModule
    {
        public BaseModule() : base("/")
        {
            Get("/", args => "NancyFX Hello @ " + DateTime.UtcNow);
        }
    }
}
EOF

mkdir FudgyCron.Web/Content
cat <<EOF > FudgyCron.Web/Content/index.html
<html>
  <head>
    <title>NancyFX Demo</title>
  </head>
  <body>
    <p>Demoing NancyFX!</p>
  </body>
</html>
EOF

cat <<EOF > FudgyCron.Web/Bootstrapper.cs
using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Diagnostics;
using Nancy.TinyIoc;

public class Bootstrapper : DefaultNancyBootstrapper
{
    protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
    {
    }

    public override void Configure(INancyEnvironment environment)
    {
        environment.Diagnostics(true, "password");
        environment.Tracing(enabled: true, displayErrorTraces: true);
        base.Configure(environment);
    }
}
EOF



rm -rf FudgyCron.Web/bin FudgyCron.Web/obj FudgyCron.Tests/bin FudgyCron.Tests/obj
dotnet clean
dotnet restore

cd FudgyCron.Web && dotnet build && dotnet restore && cd ..
cd FudgyCron.Web && dotnet publish -c Release && cd ..
dotnet FudgyCron.Web/bin/Release/netcoreapp2.0/publish/FudgyCron.Web.dll

cp FudgyCron.Web/Content/* FudgyCron.Web/bin/Release/netcoreapp2.0/publish/Content/
