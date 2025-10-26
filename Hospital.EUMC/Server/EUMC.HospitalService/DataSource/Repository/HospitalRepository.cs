/*
using Common;
using DataSource.Database;

namespace DS.HospitalService;
public class HospitalRepository : OracleRepository, IHospitalRepository
{
  const string XMED = "XMED";
  const string XEDP = "XEDP";
  const string XSUP = "XSUP";
  string HspCode;
  public HospitalRepository(string hspCode, string xmed, string xedp, string xsup)
  {
    this.HspCode = hspCode;
    AddConnection(XMED, xmed);
    AddConnection(XEDP, xedp);
    AddConnection(XSUP, xsup);

    //column_mapping();
  }

  public List<DRUG_DTO> DRUG_DATA()
  {
    throw new NotImplementedException();
  }

  public List<EMERGENCY_DTO> ER_ROOM_DATA()
  {
    throw new NotImplementedException();
  }
  public List<EXAM_DTO> EXAM_DATA()
  {
    throw new NotImplementedException();
  }

  public List<ICU_DTO> ICU_DATA()
  {
    throw new NotImplementedException();
  }

  public List<OFFICE_DTO> OFFICE_DATA()
  {
    throw new NotImplementedException();
  }

  public List<OPERATION_DTO> OPERATION_DATA()
  {
    throw new NotImplementedException();
  }
  public List<INSPECTION_DTO> INSPECTION_DATA()
  {
    throw new NotImplementedException();
  }
  public List<PHOTO_DTO> PHOTO_DATA()
  {
    throw new NotImplementedException();
  }
}*/