using Newtonsoft.Json;
using System;
using System.Linq;

namespace Framework.DataSource
{
  /// <summary>
  /// Raw 데이타의 변경 여부를 판단하기 위한 로직 제공
  /// </summary>
  public class OriginDataModel
  {
    [JsonIgnore]
    [Origin(DTO.PRIMARY)]
    public int HashCode
    {
      get => (_hashCode == 0) ? GetHashCode() : _hashCode;
      set => _hashCode = value;
    }
    // Key Attribute들로 hash code를 만들어서 아이템의 추가/삭제를 알아낼때 사용
    // 객체 필드 변환 여부와는 무관함에 주의
    int _hashCode;
    public override int GetHashCode()
    {
      if (_hashCode == 0)
      {
        unchecked // Overflow is fine, just wrap
        {
          Type type = typeof(OriginAttribute);
          int hash = (int) 2166136261;
          bool key_exist = false;
          foreach (var pi in this.GetType().GetProperties())
          {
            var attri = pi.GetCustomAttributes(type, false).FirstOrDefault() as OriginAttribute;
            if (attri != null && attri.IsUnique)
            {
              hash = (hash * 16777619) ^ (pi.GetValue(this)?.GetHashCode() ?? 0);
              key_exist = true;
            }
          }
          if (!key_exist)
          {
            //LOG.e($"{this.GetType().Name} : no key attribute");
          }
          this._hashCode = hash;
        }
        //_hashCode = (dept_cd, hsp_tp_cd, pt_no).GetHashCode();
      }
      return _hashCode;
    }

    public object Clone()
    {
      return this.MemberwiseClone();
    }
  }
}