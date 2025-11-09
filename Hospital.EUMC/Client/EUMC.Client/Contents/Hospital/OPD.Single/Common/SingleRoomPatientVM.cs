using UIControls;

namespace EUMC.Client
{
  internal class SingleRoomPatientVM : ViewModelBase
  {
    public int Index { get; set; } // index에 따라 폰트크기, 배경색이 달라질수있다
    public string Title { get; set; } = string.Empty;
    string _patientName = string.Empty;
    public string PatientName { get => _patientName; set => Set(ref _patientName, value); }
    public SingleRoomPatientVM(int index)
    {
      this.Index = index;
    }
    public SingleRoomPatientVM(int index, string init)
    {
      this.Index = index;
      this.Title = init;
    }
  }
}