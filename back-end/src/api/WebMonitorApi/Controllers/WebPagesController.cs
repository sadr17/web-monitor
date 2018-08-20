using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR.Client;
using Monitor;
using System;
using System.Collections.Generic;
using WebMonitorApi.Models;
using WebMonitorApi.Repository;

namespace WebMonitorApi.Controllers
{
  [EnableCors("SiteCorsPolicy")]
  [Route("api/[controller]")]
  public class WebPagesController : Controller
  {
    private static IRepository<WebPage> repository;

    private WebPagesMonitor monitor;

    // GET api/webpages
    [HttpGet]
    public IEnumerable<WebPage> GetAll()
    {
      return repository.GetAll();
    }

    // GET api/webpages/5
    [HttpGet("{id}")]
    public WebPage Get(int id)
    {
      return repository.Get(id);
    }

    // POST api/webpages
    [HttpPost]
    public IActionResult Post([FromBody]WebPage value)
    {
      if (value == null)
        return this.BadRequest();

      repository.Save(value);
      this.RestartMonitor();
      return this.Created($"/webpages/{value.Id}", value);
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
      repository.Delete(id);
      this.RestartMonitor();
    }

    private void RestartMonitor()
    {
      this.monitor.Start(repository.GetAll(), TimeSpan.FromSeconds(15));
    }

    private static async void MonitorUpdatedHandler(IEnumerable<IWebPage> pages)
    {
      var connection = new HubConnectionBuilder()
        .WithUrl("http://localhost:54131/updateHub")
        .Build();
      await connection.StartAsync();
      await connection.InvokeAsync("Update", pages);
      await connection.StopAsync();
    }

    public WebPagesController(IRepository<WebPage> rep, WebPagesMonitor monitor)
    {
      if (repository == null)
        repository = rep;
      this.monitor = monitor;
      this.monitor.Start(repository.GetAll(), TimeSpan.FromSeconds(15));
      this.monitor.Updated -= MonitorUpdatedHandler;
      this.monitor.Updated += MonitorUpdatedHandler;
    }
  }
}
