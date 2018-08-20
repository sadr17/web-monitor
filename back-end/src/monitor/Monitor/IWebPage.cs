using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Monitor
{
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
