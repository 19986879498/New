using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IView_PreInspectionService
    {
        /// <summary>
        /// 患者信息查询（预约检查） Json返回结果
        /// </summary>
        /// <param name="CardNo">卡号</param>
        /// <returns></returns>
        JsonResult PatientInformationQueryByCard(string CardNo);
        /// <summary>
        /// 患者信息查询（预约检查）
        /// </summary>
        /// <param name="CardNo">卡号</param>
        /// <returns></returns>
        View_PreInspection PatientQueryByCard(string CardNo,ref string Err);

    }
}
