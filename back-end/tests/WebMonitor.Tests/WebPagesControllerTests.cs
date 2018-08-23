using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monitor;
using System.Collections.Generic;
using System.Linq;
using WebMonitorApi.Controllers;
using WebMonitorApi.Models;
using WebMonitorApi.Repository;

namespace WebMonitor.Tests
{
  [TestClass]
  public class WebPagesControllerTests
  {
    private const string TestUriString = "http://vk.com/";
    private const string TestName = "ВКонтакте";
    private const string NewTestName = "VKontakte";

    [TestMethod]
    public void CreateWebPage()
    {
      var controller = CreateController();
      var pages = controller.GetAll();
      var original = new List<WebPage>(pages);

      var p = CreateDefaultPage();
      var result = controller.Post(p);
      var newWebPagesRaw = controller.GetAll();
      List<WebPage> newPages = new List<WebPage>(newWebPagesRaw);

      Assert.AreEqual(newPages.Count, original.Count + 1);

      var addedPage = newPages
        .FirstOrDefault(page => page.DisplayName == TestName && page.Link.ToString().Equals(TestUriString));
      Assert.IsNotNull(addedPage);
    }

    [TestMethod]
    public void DeleteWebPage()
    {
      var controller = CreateController();
      WebPage p = CreateDefaultPage();

      var result = controller.Post(p);
      var page = ((ObjectResult)result).Value as WebPage;
      Assert.IsNotNull(page);

      controller.Delete(page.Id);

      var notExistsPage = controller.Get(page.Id);
      Assert.IsNull(notExistsPage);
    }

    [TestMethod]
    public void UpdatePage()
    {
      var controller = CreateController();
      var p = CreateDefaultPage();

      var result = controller.Post(p);
      p = ((ObjectResult)result).Value as WebPage;
      Assert.IsNotNull(p);

      p.DisplayName = NewTestName;

      result = controller.Post(p);
      var updatedPage = ((ObjectResult)result).Value as WebPage;
      updatedPage = controller.Get(updatedPage.Id);

      Assert.AreEqual(updatedPage.DisplayName, NewTestName);

      var pages = controller.GetAll();
      Assert.AreEqual(pages.Count(), 1);
    }

    private static WebPage CreateDefaultPage()
    {
      return new WebPage()
      {
        DisplayName = TestName,
        Link = new System.Uri(TestUriString)
      };
    }

    private static WebPagesController CreateController()
    {
      return new WebPagesController(new TestWebPages());
    }
  }
}
