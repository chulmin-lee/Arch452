using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Framework.DataSource.Oracle
{
  public static class OracleConnectionExtension
  {
    public static List<T> OracleProcedure<T>(this OracleConnection conn, string procedure, Dictionary<string, string> dic)
    {
      var param = new OracleDynamicParameters();
      foreach (KeyValuePair<string, string> item in dic)
      {
        param.Add(item.Key, OracleDbType.Varchar2, ParameterDirection.Input, item.Value);
      }
      param.Add("OUT_CURSOR", OracleDbType.RefCursor, ParameterDirection.Output);

      return conn.Query<T>(procedure, param: param, commandType: CommandType.StoredProcedure).ToList();
    }
  }
}
