using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IView_PacsService
    {
        /// <summary>
        /// 查询pacs检查（返回json结果）
        /// </summary>
        /// <param name="keshiCode"></param>
        /// <returns></returns>
        JsonResult GetDepartmentQuery(DiviceParams keshiCode);
        /// <summary>
        /// 查询pacs检查
        /// </summary>
        /// <param name="keshiCode"></param>
        /// <returns></returns>
        List<View_Pacs> GetDepartment(DiviceParams keshiCode);
        /// <summary>
        /// 保存pacs检查（返回json结果）
        /// </summary>
        /// <param name="PreInspection">pacs检查报告对象</param>
        /// <returns></returns>
        JsonResult SaveCheckData(PreInspection PreInspection);
        /// <summary>
        /// 保存pacs检查
        /// </summary>
        /// <param name="PreInspection">pacs检查报告对象</param>
        /// <param name="Err">错误信息</param>
        /// <returns></returns>
        bool SaveCheckData(PreInspection PreInspection, ref string Err);
    }
}
