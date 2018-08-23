using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using WebMonitorApi.Models;

namespace WebMonitorApi.Repository
{
  /// <summary>
  /// Database context for working with web pages.
  /// </summary>
  public class WebPagesContext : DbContext
  {
    #region Inner class

    /// <summary>
    /// Model configuration for web page.
    /// </summary>
    private class WebPagesModelConfiguration : IEntityTypeConfiguration<WebPage>
    {
      public void Configure(EntityTypeBuilder<WebPage> builder)
      {
        builder.Property(e => e.Link)
                .HasConversion(v => v.ToString(), v => new Uri(v));
      }
    }

    #endregion

    /// <summary>
    /// Web pages.
    /// </summary>
    public DbSet<WebPage> Pages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfiguration(new WebPagesModelConfiguration());
      base.OnModelCreating(modelBuilder);
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="options">Database options</param>
    public WebPagesContext(DbContextOptions<WebPagesContext> options)
      : base(options)
    { }
  }
}

