using Nancy;
using Nancy.Bootstrapper;
using Nancy.Configuration;
using Nancy.Diagnostics;
using Nancy.Json;
using Nancy.TinyIoc;

public class Bootstrapper : DefaultNancyBootstrapper
{
    protected override void ApplicationStartup(TinyIoCContainer container, IPipelines pipelines)
    {
    }

    public override void Configure(INancyEnvironment environment)
    {
        environment.Json(retainCasing: true);
        environment.Diagnostics(true, "password");
        environment.Tracing(enabled: true, displayErrorTraces: true);
        base.Configure(environment);
    }
}
