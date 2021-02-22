using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Oracle.EntityFrameworkCore.Storage.Internal;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using static HISDouble.Factory.DapperContext;

namespace HISDouble.Interfaces
{
  public  interface IDapperContext
    {
        /// <summary>
        /// 切换数据库 Oracle
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        IDbConnection GetDbConnection(string ConnectionString);
        /// <summary>
        /// 切换数据库 Sqlserver
        /// </summary>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        IDbConnection GetDbSqlConnection(string ConnectionString);
        /// <summary>
        /// 查询sql 返回结果
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string GetSqlOneResult(string sql, ref int ResultCode,SqlType? sqlType= null);
        /// <summary>
        /// 查询sql返回数据集
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="ReturnCode">返回的结果代码</param>
        /// <returns></returns>
        List<object> GetSqlSet(string sql, string SqlName, ref int ReturnCode, SqlType? sqlType = null);
        /// <summary>
        /// 获取数据集获取 jobject对象
        /// </summary>
        /// <param name="sql">sql语句</param>
        /// <param name="SqlName">数据库对应的名称</param>
        /// <param name="ReturnCode">返回的结果代码</param>
        /// <returns></returns>
        List<JObject> getList(string sql, string SqlName, ref int ReturnCode,SqlType? sqlType= null);
        /// <summary>
        /// 获取分组结果集
        /// </summary>
        /// <param name="jlist"></param>
        /// <param name="GroupName"></param>
        /// <param name="ReturnCode"></param>
        /// <returns></returns>
        JsonResult GetResult(List<JObject> jlist, string GroupName, ref int ReturnCode);
        /// <summary>
        /// 执行存储过程
        /// </summary>
        /// <param name="name"></param>
        /// <param name="paraNameList"></param>
        /// <param name="dbTypesList"></param>
        /// <param name="directions"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        List<object> ExecProcedure(string name, List<string> paraNameList, List<OracleDbType> dbTypesList, List<ParameterDirection> directions, List<object> values, ref string Error, ref int code);
    }
}
