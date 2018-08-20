using Monitor;
using System;
using System.Net;

namespace WebMonitorApi.Models
{
  /// <summary>
  /// Represents web page link in the Internet.
  /// </summary>
  public class WebPage : IEntity, IWebPage
  {
    /// <summary>
    /// Identificator.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Display name of web page.
    /// </summary>
    public string DisplayName { get; set; }

    /// <summary>
    /// Link to web page.
    /// </summary>
    public Uri Link { get; set; }

    /// <summary>
    /// Status of web page.
    /// </summary>
    public string Status { get; set; }
  }
}
