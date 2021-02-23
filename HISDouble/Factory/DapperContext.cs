using Dapper;
using Dapper.Oracle;
using HISDouble.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Oracle.EntityFrameworkCore.Storage.Internal;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
    public class DapperContext : BaseServices, IDapperContext
    {
        public DapperContext(IDbConnection connection)
        {
            this.Conn = connection;
        }
        public string Err = string.Empty;
        public int Code = 1;
        public IDbConnection Conn { get; set; }

        public IDbConnection GetDbConnection(string ConnectionString)
        {
            this.Conn = new Oracle.ManagedDataAccess.Client.OracleConnection(ConnectionString);
            return Conn;
        }

        public IDbConnection GetDbSqlConnection(string ConnectionString)
        {
            this.Conn = new SqlConnection(ConnectionString);
            return Conn;
        }

        public List<JObject> getList(string sql, string SqlName, ref int ReturnCode, SqlType? sqltype = null)
        {

            List<JObject> jlist = new List<JObject>();
            if (sqltype == null)
            {
                Conn = this.GetDbConnection(this.connRoot.GetConnectionString(SqlName));
            }
            else if (sqltype == SqlType.SqlServer)
            {
                Conn = this.GetDbSqlConnection(this.connRoot.GetConnectionString(SqlName));
            }
            IEnumerable<dynamic> ds = Conn.Query(sql);
            foreach (var item in ds)
            {
                JObject j = Function.GetJobjByDy(item);
                jlist.Add(j);
            }
            return jlist;
        }

        public JsonResult GetResult(List<JObject> jlist, string GroupName, ref int ReturnCode)
        {
            List<object> jsons = new List<object>();
            foreach (var item in jlist.Select(y => y.GetValue("HOSNAME").ToString()).Distinct())
            {
                List<object> objlist = new List<object>();
                foreach (var item1 in jlist.Select(y => y.GetValue("PACTNAME").ToString()).Distinct())
                {
                    List<object> objlist2 = new List<object>();
                    foreach (var item2 in jlist.Select(y => y.GetValue("TYPENAME").ToString()).Distinct())
                    {
                        JsonResult js2 = new JsonResult(new { TypeName = item2, data = jlist.Where(w => w.GetValue("HOSNAME").ToString() == item && w.GetValue("PACTNAME").ToString() == item1 && w.GetValue("TYPENAME").ToString() == item2) });
                        objlist2.Add(js2.Value);
                    }
                    JsonResult js = new JsonResult(new { pactName = item1, data = objlist2 });
                    objlist.Add(js.Value);
                }
                var jresult = new JsonResult(new { hosName = item, msg = objlist });
                jsons.Add(jresult.Value);
            }

            return new JsonResult(new { code = 200, data = jsons, message = "查询成功" });
        }

        /// <summary>
        /// 查询sql结果
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public string GetSqlOneResult(string sql, ref int ResultCode, SqlType? sqlType = null)
        {
            if (sqlType == null)
            {
                Conn = this.GetDbConnection(this.connRoot.GetConnectionString("OracleUserInfo"));
            }
            else if (sqlType == SqlType.SqlServer)
            {
                Conn = this.GetDbSqlConnection(this.connRoot.GetConnectionString("OracleUserInfo"));
            }

            IEnumerable<object> list = this.Conn.Query(sql);
            if (list.Count() != 1)
            {
                ResultCode = -1;
                return $"查询语句出现错误！请检查Sql{sql}是否正确！";
            }
            JObject ResultObj = Function.GetJobjByDy(this.Conn.Query(sql).FirstOrDefault());
            string Result = ResultObj.Values().FirstOrDefault().ToString();
            ResultCode = 1;
            return Result;
        }

        public string GetSqlOneResult(string sql, ref int ResultCode)
        {
            throw new NotImplementedException();
        }

        public List<object> GetSqlSet(string sql, string SqlName, ref int ReturnCode, SqlType? sqlType = null)
        {
            if (sqlType == null)
            {
                Conn = this.GetDbConnection(this.connRoot.GetConnectionString(SqlName));
            }
            else if (sqlType == SqlType.SqlServer)
            {
                Conn = this.GetDbSqlConnection(this.connRoot.GetConnectionString(SqlName));
            }
            IEnumerable<object> ds = Conn.Query<object>(sql, commandTimeout: 10000);
            return ds.ToList();
        }
        #region 存储过程的操作
        public List<object> ExecProcedure(string name, List<string> paraNameList, List<OracleDbType> dbTypesList, List<ParameterDirection> directions, List<object> values,ref string  Error,ref int code)
        {
            IDbConnection db = this.GetDbConnection(connRoot.GetConnectionString("ZJhisConnStr").ToString());
            List<object> obj = new List<object>();
            var dyParam = new OracleDynamicParameters();
            var dtParam = new List<OracleParameter>();
            for (int i = 0; i < paraNameList.Count; i++)
            {
                //dtParam.Add(new OracleParameter() { OracleDbType=dbTypesList.ElementAt(i),ParameterName=paraNameList.ElementAt(i),Direction=directions.ElementAt(i),Value=values.ElementAt(i)});
                dyParam.Add(paraNameList.ElementAt(i), value: values.ElementAt(i), dbType: (OracleMappingType?)dbTypesList.ElementAt(i), direction: directions.ElementAt(i));
            }
            db.Open();
            try
            {
                obj = db.Query(name, param: dyParam, commandType: CommandType.StoredProcedure).ToList();
                var returncode = dyParam.GetParameter("ReturnCode").AttachedParam.Value.ToString();
                var ErrMsg = dyParam.GetParameter("ErrorMsg").AttachedParam.Value.ToString();
                if (returncode == "-1")
                {
                    code = -1;
                    Error = ErrMsg;
                }
                code = int.Parse(returncode);
                Error = ErrMsg;
            }
            catch (Exception ex)
            {
                Error = ex.Message;
                code = -1;
            }
            finally
            {
                db.Close();
            }
            return obj;
        }
        #endregion
        public enum SqlType
        {
            SqlServer,
            Oracle,
            MySql,
            SqlLite
        }

    }

}
