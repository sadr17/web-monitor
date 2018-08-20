using System;
using System.Net;
using System.Net.Http;

namespace Monitor
{
  /// <summary>
  /// Class for ping web page.
  /// </summary>
  public class WebPagePing : IWebPagePing
  {
    #region IWebPagePing

    public HttpStatusCode? GetStatus(Uri link)
    {
      try
      {
        var client = new HttpClient();
        var task = client.GetAsync(link);
        task.Wait();
        var result = task.Result;
        if (result.IsSuccessStatusCode)
          return HttpStatusCode.OK;
        return result.StatusCode;
      }
      catch (AggregateException)
      {
        return null;
      }
    }

    #endregion
  }
}
