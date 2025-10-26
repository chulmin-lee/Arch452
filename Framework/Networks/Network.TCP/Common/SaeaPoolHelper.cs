using Common;
using System;
using System.Collections.Generic;
using System.Net.Sockets;

namespace Framework.Network.TCP
{
  public static class SaeaPoolHelper
  {
    static SaeaPool _instance;
    public static void Initialize(int buffer_size, int count)
    {
      if (_instance != null)
      {
        LOG.ec($"SaeaPoolHelper already initialized");
        return;
      }
      _instance = new SaeaPool(buffer_size, count);
    }

    public static SocketAsyncEventArgs GetSaea()
    {
      if (_instance == null) throw new FrameworkException("pool not created");
      return _instance.GetSaea();
    }
    public static void ReturnSaea(SocketAsyncEventArgs e)
    {
      if (_instance == null) throw new FrameworkException("pool not created");
      _instance.ReturnSaea(e);
    }

    class SaeaPool : IDisposable
    {
      byte[] Array;
      Queue<SocketAsyncEventArgs> _pool = new Queue<SocketAsyncEventArgs>();
      object _lock = new object();
      int BlockSize;
      public SaeaPool(int buffer_size, int count)
      {
        this.BlockSize = buffer_size;
        this.Array = new byte[this.BlockSize * count];

        for (int index = 0; index < count; index++)
        {
          int offset = index * buffer_size;

          var saea = new SocketAsyncEventArgs();
          saea.SetBuffer(this.Array, offset, this.BlockSize);
          _pool.Enqueue(saea);
        }
      }

      public SocketAsyncEventArgs GetSaea()
      {
        lock (_lock)
        {
          if (_pool.Count == 0)
          {
            LOG.ec("pool empty");

            var saea = new SocketAsyncEventArgs();
            saea.SetBuffer(new byte[this.BlockSize], 0, this.BlockSize);
            return saea;
          }
          else
          {
            var saea = _pool.Dequeue();
            LOG.tc($"count : {_pool.Count}");
            return saea;
          }
        }
      }
      public void ReturnSaea(SocketAsyncEventArgs e)
      {
        lock (_lock)
        {
          e.UserToken = null;
          _pool.Enqueue(e);
          LOG.tc($"count : {_pool.Count}");
        }
      }

      public void Close()
      {
        this.Dispose();
      }

      public void Dispose()
      {
        Dispose(true);
        GC.SuppressFinalize(this);
      }

      bool _disposed;
      protected virtual void Dispose(bool disposing)
      {
        if (!_disposed && disposing)
        {
          foreach (var saea in _pool)
          {
            saea.SetBuffer(null, 0, 0);
            saea.Dispose();
          }
          _pool.Clear();
        }
        _disposed = true;
      }
    }
  }
}