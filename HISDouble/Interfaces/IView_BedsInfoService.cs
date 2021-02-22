using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IView_BedsInfoService
    {
        /// <summary>
        /// 获取床位信息
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="RadioCode"></param>
        /// <returns></returns>
        List<View_BedsInfo> BedsInfoQuery(string UserCode, string RadioCode);
        /// <summary>
        /// 获取床位信息
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="RadioCode"></param>
        /// <returns></returns>
        List<View_BedsInfo> BedsInfoQuery(string DeptCode);
        /// <summary>
        /// 获取床位信息（请求返回值Json）
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="RadioCode"></param>
        /// <returns></returns>
        JsonResult BedsInfoQueryResult(string UserCode, string RadioCode);
        /// <summary>
        /// 根据科室获取床位信息（请求返回值Json）
        /// </summary>
        /// <param name="UserCode"></param>
        /// <param name="RadioCode"></param>
        /// <returns></returns>
        JsonResult BedsInfoQueryResult(string DeptCode);
    }
}
