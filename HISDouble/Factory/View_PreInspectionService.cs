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
    public class View_PreInspectionService : BaseServices, IView_PreInspectionService
    {
        private readonly IDapperContext _dapper;

        public IDbConnection DbConnection { get; set; }

        public View_PreInspectionService(IDapperContext dapper)
        {
            this._dapper = dapper;
            this.DbConnection = dapper.GetDbConnection(conn);
        }
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");

        /// <summary>
        /// 患者信息查询（预约检查） Json返回结果
        /// </summary>
        /// <param name="CardNo">卡号</param>
        /// <returns></returns>
        public JsonResult PatientInformationQueryByCard(string CardNo)
        {
            View_PreInspection view_Contractunit = this.PatientQueryByCard(CardNo, ref ErrorMsg);
            if (view_Contractunit==null)
            {
                return Function.GetErrResult(this.ErrorMsg);
            }
            return Function.GetResult<View_PreInspection>(view_Contractunit);
        }
        /// <summary>
        /// 患者信息查询（预约检查）
        /// </summary>
        /// <param name="CardNo">卡号</param>
        /// <returns></returns>
        public View_PreInspection PatientQueryByCard(string CardNo, ref string Err)
        {
            this.DbConnection.ConnectionString = this.connRoot.GetConnectionString("OrclDBPacs").ToString();
            var preInfo = this.DbConnection.GetAll<View_PreInspection>().ToList().FirstOrDefault(u => u.CARDNO == CardNo);
           
            if (preInfo == null || string.IsNullOrEmpty(preInfo.CARDNO))
            {
                Err = $"查询不该卡号为{CardNo}的患者信息";
                return null;
            }
            preInfo.PBIRTHDAY = DateTime.Parse(preInfo.PBIRTHDAY).ToString("yyyy-MM-dd HH:mm:ss");
            return preInfo;
        }
    }
}
