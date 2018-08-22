using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Monitor;
using WebMonitorApi.Broadcast;
using WebMonitorApi.Hubs;
using WebMonitorApi.Models;
using WebMonitorApi.Repository;

namespace WebMonitorApi {

  /// <summary>
  /// Web API startup class.
  /// </summary>
  public class Startup
  {
    public IConfiguration Configuration { get; }

    public string ConnectionString => Configuration.GetSection("Data:WebPagesContext:ConnectionString")?.Value;

    public void ConfigureServices(IServiceCollection services)
    {
      services.AddMvc();

      services.AddSignalR()
        .AddHubOptions<UpdateStatusHub>((hub) =>
        {
          hub.EnableDetailedErrors = true;
        });

      var corsBuilder = new CorsPolicyBuilder();
      corsBuilder.AllowAnyHeader();
      corsBuilder.AllowAnyMethod();
      corsBuilder.AllowAnyOrigin();
      corsBuilder.AllowCredentials();

      services.AddCors(options =>
      {
        options.AddPolicy("SiteCorsPolicy", corsBuilder.Build());
      });

      services.AddEntityFrameworkNpgsql()
        .AddDbContext<WebPagesContext>(options =>
        {
          options.UseNpgsql(ConnectionString);
        });

      services.AddScoped<IRepository<WebPage>, WebPages>();
      services.AddSingleton<WebPagesMonitor>();
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }

      app.UseCors("SiteCorsPolicy");

      app.UseForwardedHeaders(new ForwardedHeadersOptions
      {
        ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
      });

      app.UseSignalR(routes =>
      {
        routes.MapHub<UpdateStatusHub>("/updateHub");
      });

      app.UseBroadcast();

      app.UseMvc();

      using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
      {
        var ctx = serviceScope.ServiceProvider.GetRequiredService<WebPagesContext>();
        ctx.Database.Migrate();
      }
    }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }
  }
}
