using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using WebMonitorApi.Models;

namespace WebMonitorApi.Repository
{
  public class WebPages : IRepository<WebPage>
  {
    private WebPagesContext context;

    public void Delete(int id)
    {
      var page = this.Get(id);
      if (page == null)
        return; 

      this.context.Pages.Remove(page);
      this.context.SaveChanges();
    }

    public WebPage Get(int id)
    {
      return this.context.Pages.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<WebPage> GetAll()
    {
      return this.context.Pages.ToList();
    }

    public void Save(WebPage entity)
    {
      var existedEntity = this.Get(entity.Id);
      if (existedEntity != null)
      {
        context.Entry(existedEntity).CurrentValues.SetValues(entity);
//        context.Pages.Attach(existedEntity);
//        context.Pages.Update(entity);
      }
      else
        context.Pages.Add(entity);
      this.context.SaveChanges();
    }

    public WebPages(WebPagesContext context)
    {
      this.context = context;
    }
  }
}

