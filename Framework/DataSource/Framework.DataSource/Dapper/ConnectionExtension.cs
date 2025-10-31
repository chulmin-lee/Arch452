using Dapper;
using System.Collections.Generic;
using System.Data;

namespace Framework.DataSource
{
  public static class ConnectionExtension
  {
    public static IEnumerable<T> Query<T>(this IDbConnection conn, string query)
    {
      return conn.Query<T>(query, null, commandType: CommandType.Text);
    }
  }
}
