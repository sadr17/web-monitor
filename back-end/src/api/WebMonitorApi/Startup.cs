using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monitor;
using WebMonitorApi.Hubs;
using WebMonitorApi.Models;
using WebMonitorApi.Repository;
using Microsoft.AspNetCore.HttpOverrides;

namespace WebMonitorApi
{
  /// <summary>
  /// Web API startup class.
  /// </summary>
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();
      services.AddSignalR()
        .AddHubOptions<UpdateStatusHub>((hub) =>
      {
        hub.EnableDetailedErrors = true;
      });

      services.AddScoped<IRepository<WebPage>, WebPages>();
      services.AddSingleton<WebPagesMonitor>();

      var corsBuilder = new CorsPolicyBuilder();
      corsBuilder.AllowAnyHeader();
      corsBuilder.AllowAnyMethod();
      corsBuilder.AllowAnyOrigin(); 
      corsBuilder.AllowCredentials();

      services.AddCors(options =>
      {
        options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
      });
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseMvc();
      app.UseCors("SiteCorsPolicy");

      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      });

      app.UseSignalR(routes =>
      {
        routes.MapHub<UpdateStatusHub>("/updateHub");
      });
    }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
  }
}
