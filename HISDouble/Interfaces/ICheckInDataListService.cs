using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface ICheckInDataListService
    {
        /// <summary>
        /// 保存打卡信息返回结果数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonResult SaveCheckindata(CheckInDataList obj);
        /// <summary>
        /// 保存打卡信息 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SaveCheck(CheckInDataList obj);
    }
}
