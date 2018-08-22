using System;
using System.Net;
using System.Threading.Tasks;

namespace Monitor
{
  /// <summary>
  /// Represents monitor contract.
  /// </summary>
  public interface IWebPagePing
  {
    /// <summary>
    /// Get status of web page.
    /// </summary>
    /// <param name="page">Web page which need to check.</param>
    /// <returns>Web page status.</returns>
    HttpStatusCode GetStatus(Uri webPageLink);
  }
}
