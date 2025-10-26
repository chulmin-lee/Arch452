/*
using System.Threading.Channels;

namespace Common
{
  public class ProducerChannel<T> : IProducerConsumer<T>
  {
    Channel<T> _channel;
    Task _task;

    public ProducerChannel(Action<T> action)
    {
      _channel = Channel.CreateUnbounded<T>();
      _task = ReadTask(action);
    }
    public ProducerChannel(Func<T, Task> action)
    {
      _channel = Channel.CreateUnbounded<T>();
      _task = ReadTask(action);
    }

    public async Task WriteAsync(T job)
    {
      if (await _channel.Writer.WaitToWriteAsync())
      {
        _channel.Writer.TryWrite(job);
      }
    }
    public void Write(T job)
    {
      _channel.Writer.TryWrite(job);
    }
    async Task ReadTask(Action<T> action)
    {
      await foreach (var job in _channel.Reader.ReadAllAsync())
      {
        action(job);
      }
    }
    async Task ReadTask(Func<T, Task> action)
    {
      await foreach (var job in _channel.Reader.ReadAllAsync())
      {
        await action(job);
      }
    }
    public void Clear()
    {
      while (_channel.Reader.TryRead(out _)) { } // Clear the channel by reading all items
    }
    public void Close()
    {
      _channel.Writer.TryComplete();
    }
  }
}
*/