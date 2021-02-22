using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
    public interface IView_OrderService
    {
        /// <summary>
        /// 患者医嘱查询
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        List<View_Order> GetOrderQuery(string hoscode, string depcode);
        /// <summary>
        /// 医嘱返回结果
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        JsonResult OrderQuery(string hoscode, string depcode);
    }
}
