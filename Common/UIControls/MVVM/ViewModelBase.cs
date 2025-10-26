using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace UIControls
{
  public class ViewModelBase : INotifyPropertyChanged, IDisposable
  {
    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
      //this.VerifyPropertyName(propertyName);
      PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    //[Conditional("DEBUG")]
    //[DebuggerStepThrough]
    //public void VerifyPropertyName(string propertyName)
    //{
    //  if (TypeDescriptor.GetProperties(this)[propertyName] == null)
    //  {
    //    string msg = "Invalid property name: " + propertyName;
    //    Debug.Fail(msg);
    //  }
    //}

    protected virtual bool Set<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
    {
      if (string.IsNullOrEmpty(propertyName))
        return false;

      if (Equals(storage, value))
        return false;

      storage = value;
      OnPropertyChanged(propertyName);
      return true;
    }
    public virtual ViewModelBase CloneCopy()
    {
      return (ViewModelBase)MemberwiseClone(); // 호출된 객체가 복사된다.
    }
    //--------------------------------------------
    // IDispose
    //--------------------------------------------
    public void Dispose()
    {
      Dispose(true);
      GC.SuppressFinalize(this);
    }
    //bool _disposed;
    protected virtual void Dispose(bool disposing)
    {
      //if (!_disposed && disposing)
      //{
      //  //
      //}
      //_disposed = true;
    }
    // 자식 클래스에서 다음과 같이 override 한다.
    // 주의 : 마지막에 base.Dispose() 호출해야 한다.
    //bool disposed = false;
    //protected override void Dispose(bool disposing)
    //{
    //  if (!this.disposed)
    //  {
    //    if (disposing)
    //    {
    //      // 관리 리소스 해지
    //    }
    //    // 비관리 리소스 해지.
    //    this.disposed = true;
    //  }
    //  base.Dispose(disposing);
    //}
  }
}