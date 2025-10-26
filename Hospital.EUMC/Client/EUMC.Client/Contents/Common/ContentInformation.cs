using Common;
using System;
using System.Windows.Threading;
using UIControls;

namespace EUMC.Client
{
  internal abstract class ContentInformation : ViewModelBase
  {
    public PageIndexer PAGE { get; set; } = new PageIndexer();
    protected int LoopCounter;
    protected int PageItemCount;
    protected object LOCK = new object();
    public CallMessageVM CallMessages { get; set; }
    int _interval;
    public ContentInformation(int interval = 0)
    {
      _interval = interval;
    }

    protected virtual int GetPageCount()
    {
      return 0;
    }
    protected int GetCurrentPage()
    {
      return this.GetPageCount().GetCurrentPage(LoopCounter);
    }

    #region Call Message
    protected void InitCallMessage(bool isLandscape, bool minPopup = false, int call_popup_duration = 10)
    {
      this.CallMessages = new CallMessageVM(isLandscape, minPopup, call_popup_duration);
    }
    protected void Bell()
    {
      if (this.CallMessages == null)
      {
        throw new InvalidOperationException("CallMessages is not initialized. Call InitCallMessage first.");
      }
      this.CallMessages.Bell();
    }
    protected void call_message(string e)
    {
      if (this.CallMessages == null)
      {
        throw new InvalidOperationException("CallMessages is not initialized. Call InitCallMessage first.");
      }
      this.CallMessages.Announce(e);
    }

    protected void call_message(string msg1, string msg2)
    {
      if (this.CallMessages == null)
      {
        throw new InvalidOperationException("CallMessages is not initialized. Call InitCallMessage first.");
      }
      this.CallMessages.OperationMessage(msg1, msg2);
    }
    protected void call_message(string name, string message, string speech)
    {
      if (this.CallMessages == null)
      {
        throw new InvalidOperationException("CallMessages is not initialized. Call InitCallMessage first.");
      }
      this.CallMessages.CallMessage(name, message, speech);
    }

    protected void CallClear()
    {
      this.CallMessages?.Close();
    }

    #endregion Call Message

    #region Timer

    DispatcherTimer _rotation_timer;
    protected void StartTimer(Action action)
    {
      if (_rotation_timer == null)
      {
        if (_interval <= 0)
        {
          throw new InvalidOperationException("Interval must be greater than zero to start the timer.");
        }

        _rotation_timer = new DispatcherTimer();
        _rotation_timer.Interval = TimeSpan.FromSeconds(_interval);
        _rotation_timer.Tick += (s, e) => action();
      }
      if (!_rotation_timer.IsEnabled)
        _rotation_timer.Start();
    }
    protected void StopTimer()
    {
      _rotation_timer?.Stop();
    }

    public void Close()
    {
      this.Dispose();
    }

    bool _disposed;
    protected override void Dispose(bool disposing)
    {
      if (!_disposed && disposing)
      {
        CloseImpl();
        StopTimer();
        this.CallMessages?.Close();
      }
      _disposed = true;
    }
    protected virtual void CloseImpl() { }

    #endregion Timer
  }
}