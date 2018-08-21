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
      return this.Created($"/webpages/{value.Id}", value);
    }

    // DELETE api/values/5
    [HttpDelete("{id}")]
    public void Delete(int id)
    {
      repository.Delete(id);
    }

    public WebPagesController(IRepository<WebPage> rep)
    {
      if (repository == null)
        repository = rep;
    }
  }
}
