using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Common
{
  /// <summary>
  /// NET.Framework 4.5 이상에서 BlockingCollection을 사용하여 Producer/Consumer 구현
  /// </summary>
  public class ProducerBlocking<T> : IProducerConsumer<T>
  {
    object _lock = new object();
    BlockingCollection<T> _bc = new BlockingCollection<T>();
    Task _task;
    long _running = 0;
    bool IsRunning => Interlocked.Read(ref _running) == 1;

    public ProducerBlocking(Action<T> action)
    {
      _task = ReadTask(action);
      _running = 1;
    }
    public ProducerBlocking(Func<T, Task> action)
    {
      _task = ReadTask(action);
      _running = 1;
    }
    public void Write(T obj)
    {
      if (this.IsRunning)
      {
        lock (_lock)
        {
          if (!_bc.IsAddingCompleted)
          {
            _bc.Add(obj);
            Monitor.Pulse(_lock); // Notify the waiting thread
          }
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
          // IsAddingCompleted=true 이지만 큐에는 데이타가 있을 수 있으므로, 모든 데이타를 처리하기 위해
          while (!_bc.IsCompleted)
          {
            lock (_lock)
            {
              if (_bc.Count == 0)
              {
                Monitor.Wait(_lock);
              }
              else
              {
                var obj = _bc.Take();
                action(obj);
              }
            }
          }
          Trace.WriteLine("Blocking ProcessTask exit");
        }
        catch (Exception ex)
        {
          Trace.WriteLine("Blocking ProcessTask exception: " + ex.Message);
        }
      });
    }
    Task ReadTask(Func<T, Task> action)
    {
      return Task.Run(() =>
      {
        try
        {
          // IsAddingCompleted=true 이지만 큐에는 데이타가 있을 수 있으므로, 모든 데이타를 처리하기 위해
          while (!_bc.IsCompleted)
          {
            lock (_lock)
            {
              if (_bc.Count == 0)
              {
                Monitor.Wait(_lock);
              }
              else
              {
                var obj = _bc.Take();
                action(obj);
              }
            }
          }
          Trace.WriteLine("Blocking ProcessTask exit");
        }
        catch (Exception ex)
        {
          Trace.WriteLine("Blocking ProcessTask exception: " + ex.Message);
        }
      });
    }
    public void Clear()
    {
      lock (_lock)
      {
        while (_bc.TryTake(out _)) { } // Clear the collection by taking all items
      }
    }
    public void Close()
    {
      if (Interlocked.Exchange(ref _running, 0) == 1)
      {
        lock (_lock)
        {
          _bc.CompleteAdding();
          Monitor.Pulse(_lock);

          while (!_bc.IsCompleted)
          {
            // queue가 비워지기를 기다린다.
            Thread.Sleep(100);
          }
          _task.Wait(1000);
        }
      }
    }
  }
}