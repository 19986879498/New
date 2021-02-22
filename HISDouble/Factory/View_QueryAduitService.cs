using Dapper;
using HISDouble.Interfaces;
using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
   public class View_QueryAduitService: BaseServices, IView_QueryAuditService
    {
        public View_QueryAduitService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
        }

        public IDapperContext dapper { get; set; }

        private IDbConnection dbConnection;
        /// <summary>
        /// 数据库配置对象
        /// </summary>
        private IConfigurationRoot connRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        public List<Userinfo> QueryAudit(string hoscode, string depcode)
        {
            this.dbConnection = this.dapper.GetDbConnection(connRoot.GetConnectionString("ZJhisConnStr").ToString());
            List<Userinfo> userlist = dbConnection.ViewGetAll<Userinfo>().Where(w => w.Hospitalcode == hoscode && w.Departmentscode == depcode&&w.IsAvailable==1).ToList();
            foreach (var item in userlist)
            {
                item.Name = Function.GetName(item.Username, this.dbConnection, item.Hospitalcode);
            }
            return userlist;
             
        }

        JsonResult IView_QueryAuditService.QueryAudit(string hoscode, string depcode)
        {
            List<Userinfo> UserinfoList = this.QueryAudit(hoscode, depcode);
            if (UserinfoList == null || UserinfoList.Count == 0)
            {
                return Function.GetErrResult("查询审核人为空！");
            }
            return Function.GetResultList<Userinfo>(UserinfoList);
        }
    }
}
