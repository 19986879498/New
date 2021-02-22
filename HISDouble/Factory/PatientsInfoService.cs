using HISDouble.Interfaces;
using HISDouble.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
   public class PatientsInfoService: BaseServices, IPatientsInfoService
    {
        public PatientsInfoService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
        }

        public IDapperContext dapper { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        /// <summary>
        /// Dapper对象
        /// </summary>
        public IDbConnection dbConnection = null;
        /// <summary>
        /// 通过卡号和住院号查询患者基本信息
        /// </summary>
        /// <param name="CardNo">卡号</param>
        /// <param name="InpatientNo">住院号</param>
        /// <param name="patientsinfo">患者基本信息</param>
        /// <returns></returns>
        public Patientsinfo SelectByCardNo(string CardNo, string InpatientNo,ref Patientsinfo patientsinfo)
        {
            this.dbConnection = this.dapper.GetDbConnection(conn);
            patientsinfo = dbConnection.GetAll<Patientsinfo>().FirstOrDefault(i => i.CardNo == CardNo && i.InpatientNo == InpatientNo);
            return patientsinfo;
        }
    }
}
