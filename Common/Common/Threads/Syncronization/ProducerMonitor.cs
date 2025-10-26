using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
  public class ProducerMonitor<T> : IProducerConsumer<T>
  {
    object _lock = new object();
    CancellationTokenSource _cts;
    Queue<T> queue = new Queue<T>();
    Task _task;
    long _running = 0;
    bool IsRunning => Interlocked.Read(ref _running) == 1;

    public ProducerMonitor(Action<T> action)
    {
      _cts = new CancellationTokenSource();
      _task = ReadTask(action);
      _running = 1;
    }
    public ProducerMonitor(Func<T, Task> action)
    {
      _cts = new CancellationTokenSource();
      _task = ReadTask(action);
      _running = 1;
    }
    public void Write(T obj)
    {
      if (this.IsRunning)
      {
        lock (_lock)
        {
          queue.Enqueue(obj);
          Monitor.Pulse(_lock); // Notify the waiting thread
        }
      }
    }
    public Task WriteAsync(T job)
    {
      throw new NotImplementedException();
    }
    Task ReadTask(Action<T> action)
    {
      return Task.Run(() =>
      {
        try
        {
          while (!_cts.Token.IsCancellationRequested)
          {
            lock (_lock)
            {
              if (queue.Count == 0)
              {
                Monitor.Wait(_lock);
              }
              else
              {
                var obj = queue.Dequeue();
                action(obj);
              }
            }
          }
          Trace.WriteLine("Monitor ProcessTask exit");
        }
        catch (Exception ex)
        {
          Trace.WriteLine("Monitor ProcessTask exception: " + ex.Message);
        }
      }, _cts.Token);
    }
    Task ReadTask(Func<T, Task> action)
    {
      return Task.Run(() =>
      {
        try
        {
          while (!_cts.Token.IsCancellationRequested)
          {
            lock (_lock)
            {
              if (queue.Count == 0)
              {
                Monitor.Wait(_lock);
              }
              else
              {
                var obj = queue.Dequeue();
                action(obj);
              }
            }
          }
          Trace.WriteLine("Monitor ProcessTask exit");
        }
        catch (Exception ex)
        {
          Trace.WriteLine("Monitor ProcessTask exception: " + ex.Message);
        }
      }, _cts.Token);
    }
    public void Clear()
    {
      lock (_lock)
      {
        queue.Clear();
      }
    }
    public void Close()
    {
      if (Interlocked.Exchange(ref _running, 0) == 1)
      {
        lock (_lock)
        {
          _cts.Cancel();
          Monitor.Pulse(_lock);
          //_task.Wait();
        }
      }
    }
  }
}