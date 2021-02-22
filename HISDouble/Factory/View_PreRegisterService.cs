using Dapper;
using HISDouble.Interfaces;
using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
   public class View_PreRegisterService: BaseServices, IView_PreRegisterService
    {
        public View_PreRegisterService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
        }

        public static IConfiguration Configuration { get; private set; }
        public IDapperContext dapper { get; set; }

        private IDbConnection dbConnection;

        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        /// <summary>
        /// 获取患者基本信息
        /// </summary>
        /// <param name="MedicalRecordNo"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        public View_PreRegister PatientQuery(string MedicalRecordNo, ref string Err)
        {
            DBFunction.OpenConnService(dbConnection);
            var item = dbConnection.ViewGetAll<View_PreRegister>($"where MEDICALRECORDNO='{MedicalRecordNo}'").FirstOrDefault();
            if (item==null)
            {
                Err = "查询不到该患者的基本信息";
                return null;
            }
            return item;
        }
        /// <summary>
        /// 获取患者基本信息查询webapijson数据
        /// </summary>
        /// <param name="MedicalRecordNo"></param>
        /// <returns></returns>
        public JsonResult GetPatientQueryResult(string MedicalRecordNo)
        {
            string Err = string.Empty;
            View_PreRegister preRegister = this.PatientQuery(MedicalRecordNo, ref Err);
            if (preRegister==null)
            {
                return Function.GetErrResult(Err);
            }
            return Function.GetResult<View_PreRegister>(preRegister);
        }
    }
}
