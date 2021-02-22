using Dapper.Contrib.Extensions;
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
  public  class MedicaldicService: BaseServices, IMedicaldicService
    {
        public MedicaldicService(IDapperContext dapper)
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
        /// 医保类型查询
        /// </summary>
        /// <returns></returns>
        public List<Medicaldic> MedQuery()
        {
            return dbConnection.GetAll<Medicaldic>().ToList();
        }
        /// <summary>
        /// 医保类型查询返回结果
        /// </summary>
        /// <returns></returns>
        public JsonResult GetResultMedQuery()
        {
            List<Medicaldic> list = MedQuery();
            if (list==null)
            {
                return Function.GetErrResult("医保类型查询为空！");
            }
            return Function.GetResultList<Medicaldic>(list);
        }
    }
}
