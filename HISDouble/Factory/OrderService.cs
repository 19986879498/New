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
   public class OrderService: BaseServices, IOrderService
    {
        public OrderService(IDapperContext dapper)
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
        

        public List<View_Order> OrderQuery(string hoscode, string depcode)
        {
            //List<View_Order> view_Diagnoses = dbConnection.GetAll<View_Order>().Where(w => w.CardNo == hoscode && w.InpatientNo == depcode).ToList();
            List<View_Order> view_Order = dbConnection.Query<View_Order>($"SELECT * FROM zjhis.View_Order where CardNo='{hoscode}' and InpatientNo ='{depcode}'").ToList();

            return view_Order;
        }
        /// <summary>
        /// 乡镇医院医嘱查询
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        public List<View_xzOrder> xzOrderQuery(string hoscode, string depcode)
        {
            this.dbConnection = this.dapper.GetDbConnection(this.connRoot.GetConnectionString("OrclDBPacs").ToString());
            List<View_xzOrder> view_xzOrder = dbConnection.Query<View_xzOrder>($"SELECT * FROM PT_View_Order where CardNo='{depcode}' and InpatientNo ='{hoscode}'").ToList();

            return view_xzOrder;
        }
        /// <summary>
        /// 医共体医嘱查询
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        public JsonResult getOrderResult(string hoscode, string depcode, string level)
        {
            if (level=="xz")
            {
                return Function.GetResultList<View_xzOrder>(xzOrderQuery(hoscode, depcode));
            }
            else if (level=="zj")
            {
                return Function.GetResultList<View_Order>(OrderQuery(hoscode, depcode));
            }
            else
            {
                return Function.GetErrResult("你输入的代码不存在！");
            }
        }
    }
}
