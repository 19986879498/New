using Dapper;
using HISDouble.Interfaces;
using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
    public class OutpatientService:BaseServices,IOutpatientService
    {
        public OutpatientService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbconn = dapper.GetDbConnection(conn);
        }

        public IDbConnection dbconn = null;

        public IDapperContext dapper { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        /// <summary>
        /// 查询门诊医生
        /// </summary>
        /// <param name="doctCode"></param>
        /// <param name="deptCode"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        public List<Models.Outpatient> GetOutpatients(string doctCode,string deptCode, ref string Err)
        {
            string StrSql = @"select  *
  from fin_opr_register r
 where r.reg_date > trunc(sysdate)
   and r.valid_flag = '1'
   and (r.doct_code='{0}' or r.doct_code is null or r.doct_code='') and r.dept_code='{1}'";
            StrSql = string.Format(StrSql, doctCode,deptCode );
            List<Outpatient> outList = this.dbconn.Query<Outpatient>(StrSql).ToList();
            foreach (var item in outList)
            {
                item.DIAG_NAME = dbconn.QueryFirstOrDefault<string>("select d.diag_name from met_com_diagnose d where d.inpatient_no='" + item.CLINIC_CODE + "'")==null?"": dbconn.QueryFirstOrDefault<string>("select d.diag_name from met_com_diagnose d where d.inpatient_no='" + item.CLINIC_CODE + "'").ToString();
                item.BIRTHDAY = DateTime.Parse(item.BIRTHDAY).ToString("yyyy-MM-dd HH:mm:ss");
                item.REG_DATE = DateTime.Parse(item.REG_DATE).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (outList==null)
            {
                Err = "查询无数据！";
            }
            return outList;
        }
        public List<Models.Outpatient> GetOutpatients(string deptCode, ref string Err)
        {
            deptCode = string.IsNullOrEmpty(deptCode) ? "ALL" : deptCode;
            string StrSql = @"select  *
  from fin_opr_register r
 where r.reg_date > trunc(sysdate)
   and r.valid_flag = '1'
and (r.dept_code='{0}' or '{0}'='ALL')";
            StrSql = string.Format(StrSql, deptCode);
            List<Outpatient> outList = this.dbconn.Query<Outpatient>(StrSql).ToList();
            foreach (var item in outList)
            {
                item.DIAG_NAME = dbconn.QueryFirstOrDefault<string>("select d.diag_name from met_com_diagnose d where d.inpatient_no='" + item.CLINIC_CODE + "'") == null ? "" : dbconn.QueryFirstOrDefault<string>("select d.diag_name from met_com_diagnose d where d.inpatient_no='" + item.CLINIC_CODE + "'").ToString();
                item.BIRTHDAY = DateTime.Parse(item.BIRTHDAY).ToString("yyyy-MM-dd HH:mm:ss");
                item.REG_DATE = DateTime.Parse(item.REG_DATE).ToString("yyyy-MM-dd HH:mm:ss");
            }
            if (outList == null)
            {
                Err = "查询失败！";
            }
            return outList;
        }
        public JsonResult GetOutPatientResult(string doctCode, string deptCode)
        {
            string ErrorMsg = string.Empty;
            List<Outpatient> outList = this.GetOutpatients(doctCode, deptCode, ref ErrorMsg);
            if (outList.Count>0)
            {
                return Function.GetResultList<Outpatient>(outList);
            }
            ErrorMsg = "查询无数据或查询数量为0！";
            return Function.GetErrResult(ErrorMsg);
        }
        public JsonResult GetOutPatientResult(  string deptCode)
        {
            string ErrorMsg = string.Empty;
            List<Outpatient> outList = this.GetOutpatients(deptCode, ref ErrorMsg);
            if (outList.Count > 0)
            {
                return Function.GetResultList<Outpatient>(outList);
            }
            ErrorMsg = "查询无数据或查询数量为0！";
            return Function.GetErrResult(ErrorMsg);
        }
    }
}
