using System;

namespace Framework.DataSource
{
  public enum DTO
  {
    NONE,
    PRIMARY,
    UNIQUE,
    COMPUTED,
  }

  [AttributeUsage(AttributeTargets.Property)]
  public class OriginAttribute : Attribute
  {
    public DTO Constraint { get; private set; }
    public bool IsUnique { get; private set; }
    public bool IsPrimary { get; private set; }
    public bool IsComputed { get; private set; }
    public bool IsNormal { get; private set; } = true;
    public string Mapping { get; set; } = string.Empty;
    public OriginAttribute() { }
    public OriginAttribute(DTO o)
    {
      Constraint = o;

      this.IsNormal = false;
      switch (o)
      {
        case DTO.UNIQUE: IsUnique = true; break;
        case DTO.PRIMARY: IsPrimary = true; break;
        case DTO.COMPUTED: IsComputed = true; break;
        default:
          this.IsNormal = true;
          break;
      }
    }
  }
}