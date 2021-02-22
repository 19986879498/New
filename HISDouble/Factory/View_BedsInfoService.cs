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
   public class View_BedsInfoService: BaseServices, IView_BedsInfoService
    {
        public View_BedsInfoService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
        }

        public IDapperContext dapper { get; set; }

        private IDbConnection dbConnection;

        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        /// <summary>
        /// 获取住院信息
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="RadioCode"></param>
        /// <returns></returns>
        public List<View_BedsInfo> BedsInfoQuery(string UserCode, string RadioCode)
        {
            string WhereStr = RadioCode == "1" ? $"where dept_code={UserCode} and in_state='I'" : $"where house_doc_code={UserCode} and in_state='I'";
            List <View_BedsInfo> view_Beds = SqlExec.ViewGetAll<View_BedsInfo>(dbConnection,WhereStr);
            return view_Beds; 
        }

        /// <summary>
        /// 获取住院信息返回结果
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="RadioCode"></param>
        /// <returns></returns>
        public JsonResult BedsInfoQueryResult(string UserCode, string RadioCode)
        {
            List<View_BedsInfo> view_Beds = this.BedsInfoQuery(UserCode, RadioCode);
            if (view_Beds==null)
            {
                return Function.GetErrResult("没有查询到如何床位信息");
            }
            return Function.GetResultList<View_BedsInfo>(view_Beds);
        }
        /// <summary>
        /// BedInfosQuery根据科室查询 
        /// </summary>
        /// <param name="DeptCode"></param>
        /// <returns></returns>
        public List<View_BedsInfo> BedsInfoQuery(string DeptCode)
        {
            string WhereStr = $"where dept_code={DeptCode} and in_state='I'";
            List<View_BedsInfo> view_Beds = SqlExec.ViewGetAll<View_BedsInfo>(dbConnection, WhereStr);
            return view_Beds;
        }
        /// <summary>
        /// BedInfosQuery根据科室查询 
        /// </summary>
        /// <param name="DeptCode"></param>
        /// <returns></returns>
        public JsonResult BedsInfoQueryResult(string DeptCode)
        {
            List<View_BedsInfo> view_Beds = this.BedsInfoQuery(DeptCode);
            if (view_Beds == null)
            {
                return Function.GetErrResult("没有查询到如何床位信息");
            }
            return Function.GetResultList<View_BedsInfo>(view_Beds);
        }
    }
}
