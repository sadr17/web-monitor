using Microsoft.AspNetCore.TestHost;
using Monitor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using WebMonitorApi.Models;
using Xunit;

namespace WebMonitor.Tests
{
  public class WebPagesMonitorTests
  {
    private static IEnumerable<WebPage> GetPages()
    {
      return new WebPage[]
      {
        new WebPage()
        {
          Id = 1,
          DisplayName = "Вконтакте",
          Link = new Uri("http://vk.com")
        },
        new WebPage()
        {
          Id = 2,
          DisplayName = "Test",
          Link = new Uri("http://vk123dd.com")
        }
      };
    }

    [Fact]
    public void MonitorPagesTest()
    {
      using (var monitor = new WebPagesMonitor())
      {
        var originalPages = GetPages();
        monitor.Start(originalPages, TimeSpan.FromSeconds(5));
        var isRaised = false;
        monitor.Updated += (pages) =>
        {
          isRaised = true;
          var vk = pages.First();
          Assert.Equal(vk.Status, HttpStatusCode.OK.ToString());

          var test = pages.Last();
          Assert.True(test.Status == null);
        };
        Thread.Sleep(6000);
        Assert.True(isRaised);
      }
    }
  }
}
