using System.Threading.Tasks;

namespace Common
{
  public interface IProducerConsumer<T>
  {
    Task WriteAsync(T job);
    void Write(T job);
    void Clear();
    void Close();
  }
}