using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IView_PreRegisterService
    {
        /// <summary>
        /// 查询患者基本信息
        /// </summary>
        /// <param name="MedicalRecordNo"></param>
        /// <returns></returns>
        View_PreRegister PatientQuery(string MedicalRecordNo,ref string Err);
        /// <summary>
        /// 查询患者基本信息（api返回结果值）
        /// </summary>
        /// <param name="MedicalRecordNo"></param>
        /// <returns></returns>
        JsonResult GetPatientQueryResult(string MedicalRecordNo);

        

    }
}
