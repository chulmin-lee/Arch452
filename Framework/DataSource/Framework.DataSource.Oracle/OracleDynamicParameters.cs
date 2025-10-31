using Oracle.ManagedDataAccess.Client;
using System.Data;
using Dapper;
using System.Collections.Generic;

namespace Framework.DataSource.Oracle
{
  public class OracleDynamicParameters : SqlMapper.IDynamicParameters
  {
    private readonly DynamicParameters dynamicParameters = new DynamicParameters();
    private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();

    public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction, object value = null, int size = 0)
    {
      OracleParameter oracleParameter;
      if (size > 0)
      {
        oracleParameter = new OracleParameter(name, oracleDbType, size, value, direction);
      }
      else
      {
        oracleParameter = new OracleParameter(name, oracleDbType, value, direction);
      }

      oracleParameters.Add(oracleParameter);
    }
    public void Add(string name, OracleDbType oracleDbType, ParameterDirection direction)
    {
      var oracleParameter = new OracleParameter(name, oracleDbType, direction);
      oracleParameters.Add(oracleParameter);
    }

    public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
    {
      ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);

      var oracleCommand = command as OracleCommand;

      if (oracleCommand != null)
      {
        oracleCommand.Parameters.AddRange(oracleParameters.ToArray());
      }
    }
  }
}
