using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IReferralService
    {
        /// <summary>
        /// 保存转诊 -继承基类
        /// </summary>
        /// <param name="referral"></param>
        /// <param name="diagnosis"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        int TranSaveReferral(Params @params, ref string Err);
        /// <summary>
        /// 转诊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IEnumerable<Referral> QueryReferral(RequestQueryParam param, ref string Err);
        /// <summary>
        /// 查询转诊信息通过Rid
        /// </summary>
        /// <param name="Rid"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        Referral GetReferralByRid(int Rid, ref string Err);
        /// <summary>
        /// 查询转诊信息返回json
        /// </summary>
        /// <param name="Rid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        JsonResult QueryReferral(int Rid, string status);
        /// <summary>
        /// 保存转诊信息
        /// </summary>
        /// <param name="reffral"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        bool SaveOrReferral(Referral reffral, ref string Err);
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="referral"></param>
        /// <returns></returns>
        JsonResult UpdateReferralResult(int id, string status, string reason);
        /// <summary>
        /// 删除转诊信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool DeleteById(int id, ref string Err);
        /// <summary>
        /// 删除转诊信息（json结果）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        JsonResult DeleteByIdResult(int id);

    }
}
