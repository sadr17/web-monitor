using Microsoft.AspNetCore.Builder;

namespace WebMonitorApi.Broadcast
{
  /// <summary>
  /// Extensions for adding broadcast middleware.
  /// </summary>
  public static class BrodcastMiddlewareExtensions
  {
    /// <summary>
    /// Added broadcast middleware fro updating status of web pages.
    /// </summary>
    /// <param name="app">Application builder.</param>
    /// <returns></returns>
    public static IApplicationBuilder UseBroadcast(this IApplicationBuilder app)
    {
      return app.UseMiddleware<Broadcast>();
    }
  }
}
