using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monitor;
using System;
using System.Net;

namespace WebMonitor.Tests
{
  [TestClass]
  public class WebPagePingTests
  {
    [TestMethod]
    public void PingExistedWebPage()
    {
      var monitor = new WebPagePing();
      var statusCode = monitor.GetStatus(new Uri("http://vk.com"));
      Assert.AreEqual(HttpStatusCode.OK, statusCode);
    }

    [TestMethod]
    public void PingNotFoundWebPage()
    {
      var monitor = new WebPagePing();
      var statusCode = monitor.GetStatus(new Uri("https://vk.com/edfgggsdfwe323"));
      Assert.AreEqual(HttpStatusCode.NotFound, statusCode);
    }

    [TestMethod]
    public void PingNotExistedWebPage()
    {
      var monitor = new WebPagePing();
      var statusCode = monitor.GetStatus(new Uri("https://vsdrr3.ru"));
      Assert.AreEqual(null, statusCode);
    }
  }
}
