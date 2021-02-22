using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Interfaces
{
   public interface IUserinfoService
    {
        /// <summary>
        /// 验证登录
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        bool ValidateUser(ref Userinfo userInfo, ref string Err);
        /// <summary>
        /// 验证登录信息获取Userinfo
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        Userinfo GetUserInfo(string UserName, string Password);
        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="param"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        IEnumerable<Userinfo> GetUserinfos(RequestQueryParam param, ref string Err);

        /// <summary>
        /// 获取用户查询api
        /// </summary>
        /// <returns></returns>
        JsonResult GetUserInfoResult(RequestQueryParam param);
        /// <summary>
        /// 保存或修改用户
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        bool SaveOrUpdateUser(Userinfo userinfo, ref string Err);
        /// <summary>
        /// 保存用户（返回结果值）
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        JsonResult SaveUser(Userinfo userinfo);
        /// <summary>
        /// 获取所有用户信息
        /// </summary>
        /// <returns></returns>
        List<Userinfo> GetUserinfos(string usercode);

    }
}
