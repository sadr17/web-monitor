using Monitor;
using System;
using System.Collections.Generic;
using System.Timers;

namespace Monitor
{
  /// <summary>
  /// Monitor of web pages.
  /// </summary>
  public class WebPagesMonitor : IDisposable
  {
    private IWebPagePing ping;

    private Timer timer;

    private IEnumerable<IWebPage> pages;

    public event Action<IEnumerable<IWebPage>> Updated;

    private void TimerHandler(object sender, ElapsedEventArgs e)
    {
      foreach (var page in this.pages)
        page.Status = this.ping.GetStatus(page.Link)?.ToString();
      this.Updated?.Invoke(this.pages);
    }

    /// <summary>
    /// Start web page monitor.
    /// </summary>
    /// <param name="pages">Pages to monitor.</param>
    /// <param name="interval">Interval.</param>
    public void Start(IEnumerable<IWebPage> pages, TimeSpan interval)
    {
      this.pages = new List<IWebPage>(pages);
      this.timer.Stop();
      this.timer.Interval = interval.TotalMilliseconds;
      this.timer.Start();
    }

    public void Dispose()
    {
      this.timer.Elapsed -= TimerHandler;
      this.timer.Stop();
      this.timer.Dispose();
    }

    /// <summary>
    /// Constructor.
    /// </summary>
    public WebPagesMonitor()
    {
      this.ping = new WebPagePing();
      this.timer = new Timer()
      {
        AutoReset = true,
        Enabled = false,
      };
      this.timer.Elapsed += TimerHandler;
    }
  }
}
