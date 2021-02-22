using HISDouble.Models;
using Microsoft.AspNetCore.JsonPatch.Internal;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IHospitaldicService
    {
        /// <summary>
        /// 根据code查询医院信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        Hospitaldic HosQueryByCode(string code);

        /// <summary>
        /// 获取医院字典表
        /// </summary>
        /// <returns></returns>
        List<Hospitaldic> HosQuery();
        /// <summary>
        /// 获取字典表请求的结果
        /// </summary>
        /// <returns></returns>
        JsonResult HosResult();
        /// <summary>
        ///获取医疗机构集合
        /// </summary>
        /// <returns></returns>
        List<HospitalInfo> GetHospitalInfos();
        /// <summary>
        /// 获取医院查询接口
        /// </summary>
        /// <returns></returns>
        JsonResult GetHosResult();

    }
}
