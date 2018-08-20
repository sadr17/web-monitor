using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebMonitorApi.Models;

namespace WebMonitorApi.Hubs
{
  /// <summary>
  /// Hub for updating status of web pages.
  /// </summary>
  public class UpdateStatusHub : Hub
  {
    /// <summary>
    /// Update status of pages.
    /// </summary>
    /// <param name="pages"></param>
    /// <returns></returns>
    public async Task Update(IEnumerable<WebPage> pages)
    {
      await this.Clients
        .AllExcept(this.Context.ConnectionId)
        .SendAsync("UpdateStatus", pages);
    }
  }
}
