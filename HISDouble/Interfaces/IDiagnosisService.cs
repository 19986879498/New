using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IDiagnosisService
    {
        /// <summary>
        /// 保存诊断
        /// </summary>
        /// <param name="referral"></param>
        /// <param name="diagnosis"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        bool SaveDiagnosis(Referral referral, DiagnosisParams diagnosis, IDbTransaction trans, ref string Err);
        /// <summary>
        /// 根据Rid查询诊断信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        List<Diagnosis> DiagnosisQueryByRid(int id);
        /// <summary>
        /// 获取诊断信息
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        JsonResult DiagnosisQuery(string hoscode, string depcode);

    }
}
