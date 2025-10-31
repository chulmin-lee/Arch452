using Common;
using Dapper;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Framework.DataSource.Oracle
{
  public class OracleRepository
  {
    #region 단일 connection 사용
    string _connectionString = string.Empty;
    public OracleRepository(string constr)
    {
      _connectionString = constr;
    }
    OracleConnection CreateConnection()
    {
      var con = new OracleConnection(_connectionString);
      con.Open();
      return con;
    }
    public List<T> Query<T>(string query)
    {
      try
      {
        using (var conn = CreateConnection())
        {
          return conn.Query<T>(query).ToList();
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return new List<T>();
      }
    }
    #endregion 단일 connection 사용

    #region Multi-Account
    public OracleRepository() { }

    // 여러 connection을 선택해서 사용하는 경우

    Dictionary<string, string> Connections = new Dictionary<string, string>();

    public void AddConnection(string account, string constr)
    {
      if (Connections.ContainsKey(account))
      {
        throw new Exception($"{account} already exist");
      }
      Connections.Add(account, constr);
    }

    OracleConnection CreateConnection(string account)
    {
      if (Connections.TryGetValue(account, out string constr))
      {
        var con = new OracleConnection(constr); con.Open(); return con;
      }
      throw new Exception($"Unknown account : {account}");
    }
    public List<T> Query<T>(string account, string query)
    {
      try
      {
        using (var conn = CreateConnection(account))
        {
          return conn.Query<T>(query).ToList();
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return new List<T>();
      }
    }
    public bool Query(string account, string query)
    {
      try
      {
        using (var conn = CreateConnection(account))
        {
          return conn.Query(query) != null;
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return false;
      }
    }
    public List<T> QueryProcedure<T>(string account, string procedure, Dictionary<string, string> dic)
    {
      try
      {
        using (var conn = CreateConnection(account))
        {
          return conn.OracleProcedure<T>(procedure, dic);
        }
      }
      catch (Exception ex)
      {
        LOG.except(ex);
        return new List<T>();
      }
    }
    #endregion Multi-Account
  }
}
