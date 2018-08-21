using Microsoft.EntityFrameworkCore;
using WebMonitorApi.Models;

namespace WebMonitorApi.Repository
{
  public class WebPagesContext : DbContext
  {
    public DbSet<WebPage> Pages { get; set; }

    public WebPagesContext()
      : base()
    { }

    public WebPagesContext(DbContextOptions<WebPagesContext> options)
      : base(options)
    { }
  }
}
