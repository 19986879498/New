using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HISDouble.Interfaces;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HISDouble.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BedsInfoController : ControllerBase
    {
        private readonly ILogger<BedsInfoController> _logger;
        private readonly IView_BedsInfoService _view_BedsInfoService;

        public BedsInfoController(ILogger<BedsInfoController> logger,IView_BedsInfoService view_BedsInfoService)
        {
            this._logger = logger;
            this._view_BedsInfoService = view_BedsInfoService;
        }
        /// <summary>
        /// 根据code查询床位信息
        /// </summary>
        /// <param name="UserCode">用户Code</param>
        /// <returns></returns>
        [HttpPost,Route("BedsInfoQuery")]
        public IActionResult BedsInfoQuery([FromBody] dynamic dy)
        {
            //string UserCode, string UserCode
            JObject jobj = Function.GetJobjByDy(dy);
            this._logger.LogWarning("请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+ "根据code查询床位信息请求参数：\n"+jobj.ToString());
            string UserCode = jobj.GetValue("UserCode").ToString();
            string RadioCode = jobj.GetValue("RadioCode").ToString();
            JsonResult ht = _view_BedsInfoService.BedsInfoQueryResult(UserCode, RadioCode);
            this._logger.LogWarning("请求返回参数：\n" + ht.Value);
            return new JsonResult(ht.Value);
        }
        /// <summary>
        /// 根据code查询床位信息
        /// </summary>
        /// <param name="UserCode">用户Code</param>
        /// <returns></returns>
        [HttpPost, Route("GetBedInfoByDept")]
        public IActionResult GetBedInfoByDept([FromBody] dynamic dy)
        {
            //string UserCode, string UserCode
            JObject jobj = Function.GetJobjByDy(dy);
            this._logger.LogWarning("请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "根据code查询床位信息请求参数：\n" + jobj.ToString());
            string DeptCode = jobj.GetValue("DeptCode",StringComparison.OrdinalIgnoreCase).ToString();
            JsonResult ht = _view_BedsInfoService.BedsInfoQueryResult(DeptCode);
            this._logger.LogWarning("请求返回参数：\n" + ht.Value);
            return new JsonResult(ht.Value);
        }
    }
}
