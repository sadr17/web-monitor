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
      var entry = context.Entry(entity);
      switch (entry.State)
      {
        case EntityState.Detached:
          context.Add(entity);
          break;
        case EntityState.Modified:
          context.Update(entity);
          break;
        case EntityState.Added:
          context.Add(entity);
          break;
        case EntityState.Unchanged:  
          break;

        default:
          throw new ArgumentOutOfRangeException();
      }
      this.context.SaveChanges();
    }

    public WebPages(WebPagesContext context)
    {
      this.context = context;
    }
  }
}
