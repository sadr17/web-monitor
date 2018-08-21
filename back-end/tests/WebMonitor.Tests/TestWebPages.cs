using System.Collections.Generic;
using System.Linq;
using WebMonitorApi.Models;
using WebMonitorApi.Repository;

namespace WebMonitor.Tests
{
  class TestWebPages : IRepository<WebPage>
  {
    private IList<WebPage> pages;

    private static int lastIndex = 1;

    public void Delete(int id)
    {
      var page = this.pages.FirstOrDefault(p => p.Id == id);
      this.pages.Remove(page);
    }

    public WebPage Get(int id)
    {
      return this.pages.FirstOrDefault(p => p.Id == id);
    }

    public IEnumerable<WebPage> GetAll()
    {
      return this.pages;
    }

    public void Save(WebPage entity)
    {
      if (entity.Id == 0)
      {
        entity.Id = lastIndex++;
      }
      else
      {
        var page = this.pages.FirstOrDefault(p => p.Id == entity.Id);
        this.pages.Remove(page);
      }
      this.pages.Add(entity);
    }

    public TestWebPages()
    {
      this.pages = new List<WebPage>();
    }
  }
}
