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
   public class View_PatientsInfoService: BaseServices, IView_PatientsInfoService
    {
        public  View_PatientsInfoService(IDapperContext dapper)
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

        public List<View_PatientsInfo> zjPatientsQuery(string hoscode, string depcode)
        {
          this.dbConnection=  this.dapper.GetDbConnection(connRoot.GetConnectionString("ZJhisConnStr").ToString());//ZYhisConnStr
            //List<View_PatientsInfo> list = dbConnection.GetAll<View_PatientsInfo>().ToList();
            List<View_PatientsInfo> list = dbConnection.Query<View_PatientsInfo>($"SELECT * FROM zjhis.View_PatientsInfo where Hos_code='{hoscode}' and DepartmentCode ='{depcode}'").ToList();
            return list;
        }

        public List<View_xzPatientsInfo> xzPatientsQuery(string hoscode, string depcode)
        {
           this.dbConnection= this.dapper.GetDbConnection(connRoot.GetConnectionString("OrclDBPacs").ToString());
            List<View_xzPatientsInfo> list = dbConnection.Query<View_xzPatientsInfo>($"SELECT * FROM pt_view_patientsinfo where Hos_code='{hoscode}' and DepartmentCode ='{depcode}'").ToList();
            return list;
        }

        public JsonResult PatientsQuery(string hoscode, string depcode, string level)
        {
            if (level == "zj")
            {
                return Function.GetResultList<View_PatientsInfo>(zjPatientsQuery(hoscode, depcode));
            }
            else if (level == "xz")
            {
                return Function.GetResultList<View_xzPatientsInfo>(xzPatientsQuery(hoscode, depcode));
            }
            else
            {
                return Function.GetErrResult("你输入的代码不存在！");
            }

        }
    }
}
