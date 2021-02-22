using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
  public  interface IMedicaldicService
    {
        /// <summary>
        /// 医保类别查询
        /// </summary>
        /// <returns></returns>
        List<Medicaldic> MedQuery();
        /// <summary>
        /// 请求结果医保类别查询
        /// </summary>
        /// <returns></returns>
        JsonResult GetResultMedQuery();
    }
}
