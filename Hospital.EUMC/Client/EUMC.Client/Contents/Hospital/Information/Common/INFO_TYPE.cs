using System;

namespace EUMC.Client
{
  [AttributeUsage(AttributeTargets.Field)]
  public class InformationAttribute : Attribute
  {
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public InformationAttribute() { }
    public InformationAttribute(string title, string description)
    {
      Title = title;
      Description = description;
    }
  }

  public enum INFO_TYPE
  {
    NONE = 0,
    [Information("접수 및 예약 조회", "당일 진료 접수 및 예약 확인")]
    RECEPT,
    [Information("층별 안내", "각 층의 진료실 위치 찾기")]
    FLOOR_MAP,
    [Information("편의 시설 안내", "병원 내 편의시설 위치 안내")]
    AMENITIES,
    [Information("주차 안내", "당일 진료 접수 및 예약 확인")]
    FIND_CAR,
    [Information("교통 안내", "병원 이용 교통안내")]
    TRAFFIC_INFO,
    [Information("Home", "Home")]
    HOME,
  }
}
// xaml에서 enum : Value="{x:Static local:AppProfileItemType.Custom}"