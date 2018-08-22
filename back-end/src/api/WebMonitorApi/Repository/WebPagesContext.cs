using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using WebMonitorApi.Models;

namespace WebMonitorApi.Repository
{
  public class WebPagesContext : DbContext
  {
    private class WebPagesModelConfiguration : IEntityTypeConfiguration<WebPage>
    {
      public void Configure(EntityTypeBuilder<WebPage> builder)
      {
        builder.Property(e => e.Link)
                .HasConversion(v => v.ToString(), v => new Uri(v));
      }
    }

    public DbSet<WebPage> Pages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      modelBuilder.ApplyConfiguration(new WebPagesModelConfiguration());
      base.OnModelCreating(modelBuilder);
    }

    public WebPagesContext(DbContextOptions<WebPagesContext> options)
      : base(options)
    { }
  }
}

