using UIControls;

namespace EUMC.Client
{
  internal class SinglePatientViewModel : ViewModelBase
  {
    public int Index { get; set; }
    //public bool IsPanel { get; set; }
    public string Title { get; set; } = string.Empty;
    string _patientName = string.Empty;
    public string PatientName { get => _patientName; set => Set(ref _patientName, value); }
    public SinglePatientViewModel(int index)
    {
      this.Index = index;
    }
    public SinglePatientViewModel(int index, string init)
    {
      this.Index = index;
      this.Title = init;
    }
    public void Clear()
    {
      this.PatientName = string.Empty;
    }
  }
}