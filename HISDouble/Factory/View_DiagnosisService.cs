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
  public class View_DiagnosisService: BaseServices, IView_DiagnosisService
    {
        public View_DiagnosisService(IDapperContext dapper)
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
        /// 查询诊断信息
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        public List<View_Diagnosis> DiagnosisQuery(string hoscode, string depcode)
        {
            List<View_Diagnosis> view_Diagnoses = dbConnection.GetAll<View_Diagnosis>().Where(w => w.CardNo == hoscode && w.InpatientNo == depcode).ToList();
            return view_Diagnoses;
        }

        public List<View_xzDiagnosis> xzDiagnosisQuery(string hoscode, string depcode)
        {
            this.dbConnection = this.dapper.GetDbConnection(this.connRoot.GetConnectionString("OrclDBPacs").ToString());
            List<View_xzDiagnosis> view_xzDiagnoses = dbConnection.GetAll<View_xzDiagnosis>().Where(w => w.CardNo == depcode && w.InpatientNo == hoscode).ToList();
            return view_xzDiagnoses;
        }

        public JsonResult getDiagnosisResult(string hoscode, string depcode, string level)
        {
            if (level == "xz")
            {
                return Function.GetResultList<View_xzDiagnosis>(xzDiagnosisQuery(hoscode, depcode));
            }
            else if (level == "zj")
            {
                return Function.GetResultList<View_Diagnosis>(DiagnosisQuery(hoscode, depcode));
            }
            else
            {
                return Function.GetErrResult("你输入的代码不存在！");
            }
        }
    }
}
