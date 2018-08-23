using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.SignalR.Client;
using Monitor;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WebMonitorApi.Hubs;
using WebMonitorApi.Models;

namespace WebMonitorApi.Broadcast
{
  /// <summary>
  /// Brodcast middleware for updating page statuses.
  /// </summary>
  public class Broadcast
  {
    #region Constants

    private const string ApiPath = "/api/webpages";
    private const string UpdateMethodName = "UpdateStatus";
    public readonly TimeSpan DefaultInterval = TimeSpan.FromSeconds(5);

    #endregion

    #region Fields

    private readonly RequestDelegate next;

    private ConcurrentDictionary<int, WebPage> pages;

    private WebPagesMonitor monitor;

    IHubContext<UpdateStatusHub> hub;

    #endregion

    #region Methods

    public async Task InvokeAsync(HttpContext context)
    {
      var request = context.Request;
      if (!request.Path.StartsWithSegments(ApiPath))
      {
        await next(context);
        return;
      }

      await this.TryHandleGetRequest(context);
      await this.TryHandlePostRequest(context);
      await this.TryHandleDeleteRequest(context);

      this.RestartMonitor();
    }

    private async Task TryHandleGetRequest(HttpContext context)
    {
      var request = context.Request;
      if (request.Method != HttpMethods.Get)
        return;

      var body = await this.ReadResponseBody(context);
      var webPages = JsonConvert.DeserializeObject<List<WebPage>>(body);
      if (webPages != null)
        this.AddOrUpdate(webPages);
    }

    private async Task TryHandlePostRequest(HttpContext context)
    {
      var request = context.Request;
      if (request.Method != HttpMethods.Post)
        return;

      var body = await this.ReadResponseBody(context);
      var webPage = JsonConvert.DeserializeObject<WebPage>(body);
      this.AddOrUpdate(webPage);
    }

    private async Task TryHandleDeleteRequest(HttpContext context)
    {
      var request = context.Request;
      if (request.Method != HttpMethods.Delete)
        return;

      await next(context);

      var idValue = request.Path.Value
        .Split("/")
        .Last();

      var id = int.Parse(idValue);
      this.Remove(id);
    }

    private async Task<string> ReadResponseBody(HttpContext context)
    {
      var response = context.Response;
      string body = null;
      Stream originalBody = response.Body;
      try
      {
        using (var memStream = new MemoryStream())
        {
          response.Body = memStream;
          await this.next(context);

          memStream.Position = 0;
          body = new StreamReader(memStream).ReadToEnd();

          memStream.Position = 0;
          await memStream.CopyToAsync(originalBody);
        }
      }
      finally
      {
        response.Body = originalBody;
      }
      return body;
    }

    private void AddOrUpdate(IEnumerable<WebPage> webPages)
    {
      foreach (var page in webPages)
        this.AddOrUpdate(page);
    }

    private void AddOrUpdate(WebPage webPage)
    {
      this.pages.AddOrUpdate(webPage.Id, webPage, (id, page) => webPage);
    }

    private void Remove(int webPageId)
    {
      this.pages.TryRemove(webPageId, out WebPage page);
    }

    private void RestartMonitor()
    {
      this.monitor.Start(this.pages.Values, DefaultInterval);
    }

    private async void BroadcastHandler(IEnumerable<IWebPage> pages)
    {
      await this.hub.Clients.All.SendAsync(UpdateMethodName, pages);
    }

    #endregion

    #region Constructors

    public Broadcast(RequestDelegate next, WebPagesMonitor monitor, IHubContext<UpdateStatusHub> hub)
    {
      this.next = next;
      this.hub = hub;
      this.pages = new ConcurrentDictionary<int, WebPage>();
      this.monitor = monitor;
      this.monitor.Updated += BroadcastHandler;
    }

    #endregion
  }
}

