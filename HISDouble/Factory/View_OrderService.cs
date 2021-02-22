using Dapper;
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
   public class View_OrderService: BaseServices, IView_OrderService
    {
        public View_OrderService(IDapperContext dapper)
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
        public List<View_Order> GetOrderQuery(string hoscode, string depcode)
        {
            return dbConnection.ViewGetAll<View_Order>().Where(r => r.CardNo == hoscode && r.InpatientNo == depcode).ToList();
        }

        public JsonResult OrderQuery(string hoscode, string depcode)
        {
            //List<View_Order> orderlist = this.GetOrderQuery(hoscode, depcode);
            List<View_Order> orderlist = dbConnection.Query<View_Order>($"SELECT * FROM zjhis.View_Order where CardNo='{hoscode}' and InpatientNo ='{depcode}'").ToList();

            if (orderlist == null)
            {
                return Function.GetErrResult("查询无数据");
            }
            return Function.GetResultList<View_Order>(orderlist);
        }

    }
}
