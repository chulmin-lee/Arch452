using Common;
using ServiceCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using UIControls;

namespace EUMC.Client
{

  // ward code별로 환자 그룹을 나누기 위한 클래스
  internal class ICU_GROUP
  {
    public string IcuCode { get; set; } = string.Empty;
    public string IcuName { get; set; } = string.Empty;
    public string IcuTitle { get; set; } = string.Empty;
    public int ItemRows { get; set; }
    public int PageCount => this._all_page.Count;

    public ICU_GROUP(string code, string name, string title, int itemRows)
    {
      this.IcuCode = code;
      this.IcuName = name;
      this.IcuTitle = title;
      ItemRows = itemRows;
    }

    internal void Update(ICU_PT_INFO d)
    {
      if (d.WardCode != this.IcuCode)
      {
        LOG.ec($"{d.WardCode} not match {this.IcuCode}");
        return;
      }

      var code = d.WardCode;
      var name = d.WardName;

      this._all_page.Clear();

      var patients = d.Patients;
      var pageNo = patients.CalcPageCount(this.ItemRows);

      // itemRows 단위로 나누기
      for (int i = 0; i < pageNo; i++)
      {
        var list = patients.Skip(i * this.ItemRows).Take(this.ItemRows).ToList();
        this._all_page.Add(new ICU_CONTENT(this.ItemRows, name, list));
      }
    }

    List<ICU_CONTENT> _all_page = new List<ICU_CONTENT>();
    public ICU_CONTENT this[int i]
    {
      get { return _all_page[i]; }
      set { _all_page[i] = value; }
    }
  }

  public class ICU_CONTENT
  {
    public int ItemRows { get; set; }
    public string ContentTile { get; set; } = string.Empty;

    public List<ICU_PATIENT> Patients { get; set; } = new List<ICU_PATIENT>();

    public ICU_CONTENT(int rows, string title, List<ICU_INFO> patients)
    {
      this.ItemRows = rows;
      this.ContentTile = title;
      foreach (var p in patients)
      {
        this.Patients.Add(new ICU_PATIENT(p));
      }
    }
  }

  public class ICU_PATIENT : ViewModelBase
  {
    public string BedNo { get; set; } = string.Empty;
    public string PatientDetail { get; set; } = string.Empty;
    public string PatientSexAge { get; set; } = string.Empty;
    public string PatientName { get; set; } = string.Empty;
    public string DeptName { get; set; } = string.Empty;
    public string DoctorName { get; set; } = string.Empty;
    public string Gender { get; set; } = string.Empty;
    public string Age { get; set; }
    public string R { get; set; } = string.Empty;
    public string A { get; set; } = string.Empty;
    public string B { get; set; } = string.Empty;
    public string C { get; set; } = string.Empty;
    public string D { get; set; } = string.Empty;

    //public List<string> Infections { get; set; } = new List<string>();

    public ICU_PATIENT(ICU_INFO o)
    {
      this.BedNo = o.BedNo;
      this.DeptName = o.DeptName;
      this.DoctorName = o.DoctorName;
      this.Gender = o.Gender;
      this.Age = o.Age;

      var arr = o.Infections.Split(','); //o.Infections.Split(o.Delimeter, StringSplitOptions.RemoveEmptyEntries);
      var list = new List<string>();
      foreach (var s in arr)
      {
        if (s != "N")
        {
          list.Add(s);
        }
      }

      foreach (var p in list.Take(5))
      {
        switch (p)
        {
          case "R": this.R = p; break;
          case "A": this.A = p; break;
          case "B": this.B = p; break;
          case "C": this.C = p; break;
          case "D": this.D = p; break;
        }
      }

      //this.Infections.AddRange(list.Take(5).ToList());
      this.PatientName = o.PatientName;
      this.PatientDetail = $"{o.PatientName} ({o.Gender}/{o.Age})";
      this.PatientSexAge = $"({o.Gender}/{o.Age})";
    }
  }
}