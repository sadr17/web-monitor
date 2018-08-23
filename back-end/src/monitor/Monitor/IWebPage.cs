using System;

namespace Monitor
{
  /// <summary>
  /// Representation of simple web page object.
  /// </summary>
  public interface IWebPage
  {
    /// <summary>
    /// Link of the web page.
    /// </summary>
    Uri Link { get; }

    /// <summary>
    /// Status of the web page.
    /// </summary>
    string Status { get; set; }
  }
}
