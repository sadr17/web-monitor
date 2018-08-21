using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
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
    public async void WebPagesPostAndGetAll()
    {
      var page = new WebPage()
      {
        DisplayName = TestName,
        Link = new Uri(TestUriString)
      };
      var content = new StringContent(JsonConvert.SerializeObject(page), Encoding.UTF8, "application/json");

      HttpResponseMessage postResponse = await testClient.PostAsync(WebPagesRequestUri, content);
      postResponse.EnsureSuccessStatusCode();

      var getResponse = await testClient.GetAsync(WebPagesRequestUri);
      getResponse.EnsureSuccessStatusCode();
      string raw = await getResponse.Content.ReadAsStringAsync();
      List<WebPage> pages = JsonConvert.DeserializeObject<List<WebPage>>(raw);
      Assert.NotEmpty(pages);
      Assert.Equal(TestName, pages[0].DisplayName);
      Assert.Equal(TestUriString, pages[0].Link.ToString());
      Assert.Equal(1, pages[0].Id);
    }

    [Fact]
    public async void WebPagesDeleteAndGet()
    {
      var page = GetDefaultPage();
      var content = new StringContent(JsonConvert.SerializeObject(page), Encoding.UTF8, "application/json");

      HttpResponseMessage postResponse = await testClient.PostAsync(WebPagesRequestUri, content);
      postResponse.EnsureSuccessStatusCode();

      HttpResponseMessage deleteResponse = await testClient.DeleteAsync($"{WebPagesRequestUri}/{1}");
      deleteResponse.EnsureSuccessStatusCode();

      var getResponse = await testClient.GetAsync($"{WebPagesRequestUri}/{1}");
      getResponse.EnsureSuccessStatusCode();
      string raw = await getResponse.Content.ReadAsStringAsync();
      page = JsonConvert.DeserializeObject<WebPage>(raw);
      Assert.Null(page);
    }

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
      testServer = new TestServer(new WebHostBuilder()
        .UseStartup<Startup>());
      testClient = testServer.CreateClient();
    }
  }
}
