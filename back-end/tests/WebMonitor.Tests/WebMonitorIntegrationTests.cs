using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebMonitorApi;
using WebMonitorApi.Models;
using Xunit;

namespace WebMonitor.Tests
{
  public class WebMonitorIntegrationTests
  {
    private readonly TestServer testServer;
    private readonly HttpClient testClient;

    const string TestName = "Zombie";
    const string TestUriString = "http://vk.com/";
    private const string WebPagesRequestUri = "/api/webpages";

    [Fact]
    public async void Positive_AddNewPage()
    {
      var page = this.GetDefaultPage();
      page = await this.Save(page);
      var pages = await this.GetAll();
      var existedPage = pages.FirstOrDefault(p => p.Id == page.Id);

      Assert.NotNull(existedPage);
      Assert.Equal(page.DisplayName, existedPage.DisplayName);
      Assert.Equal(page.Link.ToString(), existedPage.Link.ToString());
      Assert.Equal(page.Id, existedPage.Id);

      await this.Delete(existedPage);
    }

    [Fact]
    public async void Positive_AddAndUpdatePage()
    {
      var page = this.GetDefaultPage();
      page = await this.Save(page);
      page.DisplayName = "Renamed";
      page.Link = new Uri("http://not.existed.site.com");
      page = await this.Save(page);
      var existedPage = await this.Get(page);

      Assert.NotNull(existedPage);
      Assert.Equal(page.DisplayName, existedPage.DisplayName);
      Assert.Equal(page.Link.ToString(), existedPage.Link.ToString());
      Assert.Equal(page.Id, existedPage.Id);

      await this.Delete(existedPage);
    }

    [Fact]
    public async void Positive_DeleteWebPage()
    {
      var page = GetDefaultPage();
      page = await this.Save(page);
      await this.Delete(page);

      page = await this.Get(page);
      Assert.Null(page);
    }

    [Fact]
    public async void Negative_DeleteNotExistedWebPage()
    {
      var page = new WebPage()
      {
        Id = -1,
      };
      page = await this.Get(page);
      Assert.Null(page);
      await this.Delete(new WebPage()
      {
        Id = -1,
      });    
    }

    #region Additional methods

    private async Task<WebPage> Save(WebPage page)
    {
      var content = new StringContent(JsonConvert.SerializeObject(page), Encoding.UTF8, "application/json");
      HttpResponseMessage postResponse = await testClient.PostAsync(WebPagesRequestUri, content);
      postResponse.EnsureSuccessStatusCode();
      string raw = await postResponse.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<WebPage>(raw);
    }

    private async Task<List<WebPage>> GetAll()
    {
      var getResponse = await testClient.GetAsync(WebPagesRequestUri);
      getResponse.EnsureSuccessStatusCode();
      var raw = await getResponse.Content.ReadAsStringAsync();
      return JsonConvert.DeserializeObject<List<WebPage>>(raw);
    }

    private async Task<WebPage> Get(WebPage page)
    {
      var pages = await this.GetAll();
      return pages.FirstOrDefault(p => p.Id == page.Id);
    }

    private async Task Delete(WebPage page)
    {
      HttpResponseMessage deleteResponse = await testClient.DeleteAsync($"{WebPagesRequestUri}/{page.Id}");
      deleteResponse.EnsureSuccessStatusCode();
    }

    #endregion

    private WebPage GetDefaultPage()
    {
      return new WebPage()
      {
        DisplayName = TestName,
        Link = new Uri(TestUriString)
      };
    }

    public WebMonitorIntegrationTests()
    {
      testServer = new TestServer(WebHost.CreateDefaultBuilder(null)
        .UseStartup<Startup>());
      testClient = testServer.CreateClient();
    }
  }
}

