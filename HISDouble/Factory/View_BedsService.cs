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
   public class View_BedsService: BaseServices, IView_BedsService
    {
        public View_BedsService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
        }

        public IDapperContext dapper { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        private IDbConnection dbConnection = null;
        public View_Beds BedsQueryByCode(string hoscode, string depcode)
        {
            this.dbConnection = dapper.GetDbConnection(conn);
            return dbConnection.GetAll<View_Beds>().FirstOrDefault<View_Beds>(h=>h.Hos_code==hoscode&&h.DepartmentsCode==depcode);
        }
        /// <summary>
        /// 获取医院科室床位
        /// </summary>
        /// <param name="hoscode"></param>
        /// <returns></returns>
        public List<View_Beds> BedsQueryByCode(string hoscode)
        {
            return dbConnection.ViewGetAll<View_Beds>().Where(v => v.Hos_code == hoscode).ToList();
        }
        /// <summary>
        /// 获取医院科室床位(请求结果)
        /// </summary>
        /// <param name="hoscode"></param>
        /// <returns></returns>
        public JsonResult BedsQueryByCodeResult(string hoscode)
        {
            List<View_Beds> list = BedsQueryByCode(hoscode);
            if (list==null)
            {
                return Function.GetErrResult("获取医院科室床位失败！");
            }
            return Function.GetResultList<View_Beds>(list);
        }
    }
}
