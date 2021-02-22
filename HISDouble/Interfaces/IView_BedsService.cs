using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IView_BedsService
    {
        /// <summary>
        /// 查询床位信息
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        View_Beds BedsQueryByCode(string hoscode, string depcode);
        /// <summary>
        /// 获取医院科室床位
        /// </summary>
        /// <param name="hoscode"></param>
        /// <returns></returns>
        List<View_Beds> BedsQueryByCode(string hoscode);
        /// <summary>
        /// 获取医院科室床位请求结果
        /// </summary>
        /// <param name="hoscode"></param>
        /// <returns></returns>
        JsonResult BedsQueryByCodeResult(string hoscode);
    }
}
