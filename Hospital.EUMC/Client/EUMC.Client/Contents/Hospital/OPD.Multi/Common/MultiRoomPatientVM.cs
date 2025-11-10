using UIControls;

namespace EUMC.Client
{
  internal class MultiRoomPatientVM : ViewModelBase
  {
    public int Index { get; set; }
    public bool IsPanel { get; set; }
    string _patientName = string.Empty;
    public string PatientName { get => _patientName; set => Set(ref _patientName, value); }
    public MultiRoomPatientVM(int index)
    {
      this.Index = index;
    }
    public MultiRoomPatientVM(int index, string init)
    {
      this.Index = index;
      this.IsPanel = true;
      this.PatientName = init;
    }
    public void Clear()
    {
      this.PatientName = string.Empty;
    }
  }
}