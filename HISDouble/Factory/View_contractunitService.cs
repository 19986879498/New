using Dapper;
using HISDouble.Interfaces;
using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
    public class View_contractunitService : BaseServices, IView_contractunitService
    {

        public View_contractunitService(IDapperContext dapper)
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
        /// 查询所有合同单位
        /// </summary>
        /// <returns></returns>
        public List<View_contractunit> SelectAll()
        {
            //  return dbConnection.Query<View_contractunit>("select *from View_contractunit").ToList();
            return dbConnection.GetAll<View_contractunit>().ToList();
        }
        /// <summary>
        /// 查询所有合同单位json处理结果
        /// </summary>
        /// <returns></returns>
        public JsonResult ContractUnitQuery()
        {
            List<View_contractunit> contracList = this.SelectAll();
            if (contracList == null || contracList.Count == 0)
            {
                return Function.GetErrResult("合同单位查询无数据！！");
            }
            return Function.GetResultList<View_contractunit>(contracList);
        }
      

    }
}
