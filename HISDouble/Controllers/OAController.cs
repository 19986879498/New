using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HISDouble.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceReference1;
using ServiceReference2;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HISDouble.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OAController : ControllerBase
    {
        private readonly WorkflowServicePortType _type;
        private readonly ILogger<OAController> _logger;
        private readonly HrmServicePortType _hrm;
        private readonly IDapperContext _dbContext;

        public OAController(ServiceReference1.WorkflowServicePortType type, ILogger<OAController> logger, HrmServicePortType hrm, IDapperContext dbContext)
        {
            this._type = type;
            this._logger = logger;
            this._hrm = hrm;
            this._dbContext = dbContext;
        }
        /// <summary>
        /// 错误信息
        /// </summary>
        private string Error = string.Empty;
        private int ErrorCode = 0;
        /// <summary>
        /// 4.1.1创建工作流程
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("doCreateWorkflowRequest")]
        public IActionResult doCreateWorkflowRequest([FromBody] dynamic dynamic)
        {
            JObject jobj = Function.GetJobjByDy(dynamic);
            if (!jobj.ContainsKey("userId"))
            {
                return Function.GetErrResult("入参传入出错，没有找到参数名为userId的参数");
            }
            if (!jobj.ContainsKey("WorkflowRequestInfo"))
            {
                return Function.GetErrResult("入参传入出错，没有找到参数名为WorkflowRequestInfo的参数");
            }
            string userId = jobj.GetValue("userId").ToString();
            #region 获取工作流参数对象
            WorkflowRequestInfo info = OAFunction.GetWorkflowRequestInfo(jobj);
            if (OAFunction.Code == -1)
            {
                return Function.GetErrResult(OAFunction.Err);
            }
            #endregion
            #region 获取userid
            //获取userID
            //UserBean[] UserID = this._hrm.getHrmUserInfoAsync("192.168.188.142", null, null, null, null, null).Result;
            string UserId = this._dbContext.GetSqlOneResult($"select h.id from hrmresource h where h.loginid='{userId}'", ref this.ErrorCode);
            if (ErrorCode == -1)
            {
                Error = UserId;
                return Function.GetErrResult(Error);
            }
            #endregion
            info.creatorId = UserId;
        
            #region sql赋值
            //转入医院
            if (GetSqlOne("select h.id from hrmsubcompany h where h.subcompanyname='{0}'", "sqdw",info.workflowMainTableInfo.requestRecords)==-1)
            {
                return Function.GetErrResult(Error);
            }
            //转出医院
            if (GetSqlOne("select h.id from hrmsubcompany h where h.subcompanyname='{0}'", "zwyljg", info.workflowMainTableInfo.requestRecords) == -1)
            {
                return Function.GetErrResult(Error);
            }
            #endregion
            //写入日志
            this._logger.LogWarning($"OA工作流创建的请求时间:{System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")}\n请求数据：\n{jobj.ToString()}");
            //workflowRequestTableFields
            doCreateWorkflowRequestRequest req = new doCreateWorkflowRequestRequest()
            {
                in0 = info,
                in1 = int.Parse(UserId)
            };
            Task<doCreateWorkflowRequestResponse> doResult = null;
            try
            {
                doResult = this._type.doCreateWorkflowRequestAsync(req);
            }
            catch (Exception ex)
            {
                return Function.GetErrResult(ex.Message);
            }
            string returnID = doResult.Result.@out;
            string Msg = OAFunction.GetStatusMsg(returnID);
            int Status = 200;
            if (Convert.ToInt32(returnID) < 0)
            {
                Status = 500;
            }
            return Ok(new { code = Status, message = Msg, requestid = returnID });
        }
    

        // POST api/<OAController>
        [HttpPost, Route("test")]
        public IActionResult Post([FromBody] dynamic value)
        {
            return Ok(value+"负载均衡");
        }

        // PUT api/<OAController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<OAController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        private int GetSqlOne(string sql, string name, WorkflowRequestTableRecord[] wlist)
        {
            foreach (var item in wlist)
            {
                var dwobj = item.workflowRequestTableFields.Where(u => u.fieldName == name).FirstOrDefault();
                string dw = dwobj.fieldValue;
                string Sql = string.Format(sql, dw);
                dwobj.fieldValue = this._dbContext.GetSqlOneResult(Sql, ref this.ErrorCode);
                if (ErrorCode == -1)
                {
                    this.Error = Sql + " 此sql查询出现错误";
                    return -1;

                }
            }
            return 1;
        }
    }
}
