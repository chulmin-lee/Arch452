namespace EUMC.HospitalService
{
  public static class DataRepository
  {
    public static IEumcRepository Create(string hspCode = "01")
    {
      return new HospitalRepositorySim();
    }
    public static IEumcRepository Create(string hspCode, string xmed, string xedp, string xsup)
    {
      //return new HospitalRepository(hspCode, xmed, xedp, xsup);
      return null;
    }
  }
}