using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HISDouble.Interfaces;
using HISDouble.Jwt;
using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HISDouble.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAnnexService ann = null;
        private readonly ILogger<AccountController> logger;
        /// <summary>
        /// 转诊服务
        /// </summary>
        public IReferralService _referralService { get; }

        private readonly IUserinfoService _userinfoService;

        public ITokenHelper _tokenHelper { get; }

        private readonly IView_PatientsInfoService _view_PatientsInfoService;
        private readonly IDiagnosisService _diagnosisService;
        private readonly IView_OrderService _view_OrderService;
        private readonly IHospitaldicService _hospitaldicService;
        private readonly IMedicaldicService _medicaldicService;
        private readonly IView_BedsService _view_BedsService;
        private IOutpatientService _outpatientService;
        private readonly IView_QueryAuditService _view_queryaduitservice;
        private readonly IView_QueryByAuditService _view_querybyauditservice;
        private readonly IView_QueryAllService _view_queryallservice;
        private readonly IOrderService _orderService;
        private readonly IView_DiagnosisService _view_DiagnosisService;

        /// <summary>
        /// 返回的错误信息
        /// </summary>
        private string ErrorMsg = string.Empty;
        public AccountController(IAnnexService annex, ILogger<AccountController> logger, IReferralService referralService, IUserinfoService userinfoService, ITokenHelper tokenHelper, IView_PatientsInfoService view_PatientsInfoService, IDiagnosisService diagnosisService, IView_OrderService view_OrderService, IHospitaldicService hospitaldicService, IMedicaldicService medicaldicService, IView_BedsService view_BedsService, IOutpatientService outpatientService, IView_QueryAuditService view_queryAuditService, IView_QueryByAuditService view_querybyauditservice, IView_QueryAllService view_queryallservice, IOrderService orderService, IView_DiagnosisService view_DiagnosisService)
        {
            this.ann = annex;
            this.logger = logger;
            this._referralService = referralService;
            this._userinfoService = userinfoService;
            this._tokenHelper = tokenHelper;
            this._view_PatientsInfoService = view_PatientsInfoService;
            this._diagnosisService = diagnosisService;
            this._view_OrderService = view_OrderService;
            this._hospitaldicService = hospitaldicService;
            this._medicaldicService = medicaldicService;
            this._view_BedsService = view_BedsService;
            _outpatientService = outpatientService;
            this._view_queryaduitservice = view_queryAuditService;
            this._view_querybyauditservice = view_querybyauditservice;
            this._view_queryallservice = view_queryallservice;
            this._orderService = orderService;
            this._view_DiagnosisService = view_DiagnosisService;
        }


        /// <summary>
        /// 转诊查询
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("AnnexQuery")]
        public IActionResult AnnexQuery([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            logger.LogWarning("请求时间\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "请求入参" + j.ToString());
            int Id = 0;
            try
            {
                Id = Convert.ToInt32(j.GetValue("id").ToString());
            }
            catch (Exception ex)
            {
                return Function.GetErrResult($"请求的参数不正确，请检查!!,没有找到参数名为id的参数");
            }
            IEnumerable<Annex> annices = this.ann.QueryAnnex(Id);
            JsonResult res = Function.GetResult<IEnumerable<Annex>>(annices);
            logger.LogWarning("请求出参" + JsonConvert.SerializeObject(res.Value));
            return res;
        }
        /// <summary>
        /// 转诊保存
        /// </summary>
        /// <param name="params"></param>
        /// <returns></returns>
        // POST api/<AccountController>
        [HttpPost, Route("SaveReferral")]
        public IActionResult SaveReferral([FromBody] Params @params)
        {
            string Err = string.Empty;
            logger.LogWarning("转诊保存请求时间\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "请求入参:\n" + JsonConvert.SerializeObject(@params));
            int result = this._referralService.TranSaveReferral(@params, ref Err);
            if (result > 0)
            {
                return Function.GetResult<string>("保存成功！");
            }
            else
            {
                return Function.GetErrResult(Err);
            }
        }
        /// <summary>
        /// 用户查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost, Route("QueryUser")]
        public IActionResult QueryUser([FromBody] RequestQueryParam param)
        {

            logger.LogWarning(" 患者信息查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + JsonConvert.SerializeObject(param));
            JsonResult ht = _userinfoService.GetUserInfoResult(param);
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }
        /// <summary>
        /// 用户保存
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        [HttpPost, Route("SaveUser")]
        public IActionResult SaveUser([FromBody] Userinfo userinfo)
        {
            logger.LogWarning(" 用户保存请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + JsonConvert.SerializeObject(userinfo));
            JsonResult ht = _userinfoService.SaveUser(userinfo);
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }
        /// <summary>
        /// 接收病人
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("Receive")]
        public IActionResult Receive([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            logger.LogWarning("转诊保存请求时间\n" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "请求入参:\n" + j.ToString());
            string status = string.Empty;
            int id = 0;
            try
            {
                status = j.GetValue("status").ToString();
                id = int.Parse(j.GetValue("id").ToString());
            }
            catch (Exception)
            {
                return Function.GetErrResult("请求入参 出现错误！");
            }
            JsonResult ht = this._referralService.QueryReferral(id, status);

            return Ok(ht.Value);
        }
        /// <summary>
        /// 转诊查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost, Route("GetReferral")]
        public IActionResult GetReferral([FromBody] RequestQueryParam param)
        {
            logger.LogWarning("转诊查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + JsonConvert.SerializeObject(param));
            IEnumerable<Referral> refList = _referralService.QueryReferral(param, ref ErrorMsg);
            refList = refList.Skip((param.pageNum - 1) * param.pageSize).Take(param.pageSize).ToList();
            if (refList == null)
            {
                return Function.GetErrResult(ErrorMsg);
            }
            JsonResult result = Function.GetResultList<Referral>(refList.ToList());
            logger.LogWarning("转诊查询返回时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n返回参数：\n" + JsonConvert.SerializeObject(result.Value));
            return Ok(result.Value);
        }
        /// <summary>
        /// 更改转诊状态  1=>上转待审 2=>上转已审 3=>上转未通过 4=>上转接收 5=>上转已收
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status">修改的状态</param>
        /// <param name="reason">修改描述</param>
        /// <returns></returns>
        [HttpPost, Route("AuditById")]
        public IActionResult AuditById([FromBody] dynamic dy)
        {
            int id = 0; string status = string.Empty; string reason = string.Empty;
            JObject j = Function.GetJobjByDy(dy);
            logger.LogWarning("更改转诊状态请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            try
            {
                id = Convert.ToInt32(j.GetValue("id").ToString());
                status = j.GetValue("status").ToString();
                reason = j.GetValue("reason").ToString();
            }
            catch
            {
                return Function.GetErrResult("参数获取有误！");
            }
            JsonResult ht = _referralService.UpdateReferralResult(id, status, reason);
            logger.LogWarning("更改转诊状态返回时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n返回参数：\n" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }

        /// <summary>
        /// 删除转诊记录
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost, Route("DeleteById")]
        public IActionResult DeleteById([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            logger.LogWarning("删除转诊记录请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            int id = 0;
            try
            {
                id = Convert.ToInt32(j.GetValue("id").ToString());
            }
            catch
            {
                return Function.GetErrResult("参数获取有误！");
            }
            JsonResult ht = _referralService.DeleteByIdResult(id);
            logger.LogWarning("删除转诊记录返回时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n返回参数：\n" + JsonConvert.SerializeObject(ht.Value));

            return Ok(ht.Value);
        }
        /// <summary>
        /// 登录api
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost, Route("Login")]
        public IActionResult Login([FromBody] Userinfo account)
        {
            logger.LogWarning("登录api请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + JsonConvert.SerializeObject(account));
            bool isLogin = this._userinfoService.ValidateUser(ref account, ref this.ErrorMsg);
            Hashtable hb = new Hashtable();
            if (isLogin)
            {
                hb = new Hashtable()
                {
                    {"code",200 },
                    { "data",account},
                    { "token",_tokenHelper.CreateToken<Userinfo>(account).TokenStr},
                    { "msg",ErrorMsg}
                };

            }
            else
            {
                hb = new Hashtable()
                {
                    {"code",500 },
                    { "data","无法找到该用户信息"},
                    { "msg",ErrorMsg}
                };
            }
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(hb));
            return Ok(hb);
        }
        /// <summary>
        /// 获取token
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [HttpPost, Route("GetToken")]
        public IActionResult GetToken([FromBody] dynamic dy)
        {
            JObject jboj = Function.GetJobjByDy(dy);
            logger.LogWarning("获取token请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + jboj.ToString());
            if (!jboj.ContainsKey("userCode"))
            {
                return Function.GetErrResult("请求失败！找不到参数名为userCode的参数");
            }
            Userinfo user = _userinfoService.GetUserinfos(jboj.GetValue("userCode").ToString()).FirstOrDefault();//f => f.Username.Equals(jboj.GetValue("userCode").ToString())
            ErrorMsg = "查询成功！";
            Hashtable hb = new Hashtable();
            if (user == null || string.IsNullOrEmpty(user.Username))
            {
                hb = new Hashtable()
                {
                    {"code",500 },
                    { "data","无法找到该用户信息"},
                    { "msg",ErrorMsg}
                };
            }
            else
            {
                hb = new Hashtable()
                {
                    {"code",200 },
                    { "data",_tokenHelper.CreateToken<Userinfo>(user).TokenStr},
                    { "msg",ErrorMsg}
                };
            }
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(hb));
            return Ok(hb);
        }
        /// <summary>
        ///  患者信息查询*
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <param name="level"></param>
        /// <returns></returns>
        // DELETE api/<AccountController>/5
        [HttpPost, Route("InpatientQuery")]
        public IActionResult InpatientQuery([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            logger.LogWarning(" 患者信息查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            JsonResult ht = _view_PatientsInfoService.PatientsQuery(j.GetValue("hoscode").ToString(), j.GetValue("depcode").ToString(), j.GetValue("level").ToString());
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }
        /// <summary>
        /// 查询审核人
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        [HttpPost, Route("QueryAudit")]
        public IActionResult QueryAudit([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            logger.LogWarning(" 审核人信息查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            JsonResult ht = _view_queryaduitservice.QueryAudit(j.GetValue("hoscode").ToString(), j.GetValue("depcode").ToString());
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }
        /// <summary>
        /// 审核查询
        /// </summary>
        /// <param name="param"></param>
        /// <param name="pageNum">页数</param>
        /// <param name="pageSize">每页数量</param>
        /// <param name="hospital">医院</param>
        /// <param name="sign">上转下转标记  1上  2下</param>
        /// <returns></returns>
        /// 
        [HttpPost, Route("QueryByAudit")]
        public IActionResult QueryByAudit([FromBody] RequestQueryParam param)
        {
            //JObject j = Function.GetJobjByDy(dy);
            logger.LogWarning(" 审核查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + param);
            JsonResult ht = _view_querybyauditservice.QueryByAudit(param);
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }
        /// <summary>
        /// 医保审核查询
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost, Route("QueryAll")]
        public IActionResult QueryAll([FromBody] RequestQueryParam param)
        {
            logger.LogWarning(" 医保审核查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + param);
            JsonResult ht = _view_queryallservice.QueryAll(param);
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }

        /// <summary>
        /// 诊断信息查询
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        [HttpPost, Route("DiagnosisQuery")]
        public IActionResult DiagnosisQuery([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            if (!j.ContainsKey("depcode"))
            {
                return Function.GetErrResult("找不到参数名为depcode的参数");
            }
            if (!j.ContainsKey("hoscode"))
            {
                return Function.GetErrResult("找不到参数名为hoscode的参数");
            }
            if (!j.ContainsKey("level"))
            {
                return Function.GetErrResult("找不到参数名为level的参数");
            }
            logger.LogWarning("诊断信息查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            JsonResult js = _view_DiagnosisService.getDiagnosisResult(j.GetValue("hoscode").ToString(), j.GetValue("depcode").ToString(), j.GetValue("level").ToString());
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(js.Value));
            return js;
        }
        /// <summary>
        /// 患者医嘱查询
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        [HttpPost, Route("OrderQuery")]
        public IActionResult OrderQuery([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            if (!j.ContainsKey("depcode"))
            {
                return Function.GetErrResult("找不到参数名为depcode的参数");
            }
            if (!j.ContainsKey("hoscode"))
            {
                return Function.GetErrResult("找不到参数名为hoscode的参数");
            }
            if (!j.ContainsKey("level"))
            {
                return Function.GetErrResult("找不到参数名为level的参数");
            }
            logger.LogWarning("患者医嘱查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            JsonResult jr = _orderService.getOrderResult(j.GetValue("hoscode").ToString(), j.GetValue("depcode").ToString(), j.GetValue("level").ToString());
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(jr.Value));
            return jr;
        }
        /// <summary>
        /// 医院字典*
        /// </summary>
        /// <returns></returns>
        [HttpPost, Route("HosQuery")]
        public IActionResult HosQuery()
        {
            logger.LogWarning("医院字典查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + "无");
            JsonResult ht = _hospitaldicService.HosResult();
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }
        /// <summary>
        /// 医保类型*
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("MedQuery")]
        public IActionResult MedQuery()
        {
            logger.LogWarning("医保类型查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + "无");
            JsonResult ht = _medicaldicService.GetResultMedQuery();
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }
        /// <summary>
        /// 获取医院科室床位*
        /// </summary>
        /// <param name="hoscode">医院编码</param>
        /// <returns></returns>
        [HttpPost, Route("BedsQueryByCode")]
        public IActionResult BedsQueryByCode([FromBody] dynamic hoscode)
        {
            JObject j = Function.GetJobjByDy(hoscode);
            logger.LogWarning("获取医院科室床位请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            string code = string.Empty;
            try
            {
                code = j.GetValue("hoscode").ToString();
            }
            catch (Exception)
            {

                return Function.GetErrResult("入参数据无效！");
            }

            JsonResult ht = _view_BedsService.BedsQueryByCodeResult(code);
            logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }

        [HttpPost, Route("OutPatientQuery")]
        public IActionResult OutPatientQuery([FromBody] dynamic dy)
        {
            JObject obj = Function.GetJobjByDy(dy);
            logger.LogWarning("请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + obj.ToString());
            string deptCode = ""; string doctCode = "";
            try
            {
                deptCode = obj.GetValue("deptCode").ToString();
                doctCode = obj.GetValue("doctCode").ToString();
            }
            catch (Exception)
            {
                return Function.GetErrResult("参数传递有误");
            }
            JsonResult js = this._outpatientService.GetOutPatientResult(doctCode, deptCode);
            logger.LogWarning("返回参数\n" + JsonConvert.SerializeObject(js.Value));
            return new JsonResult(js.Value);
        }
        [HttpPost, Route("OutPatientQueryByDept")]
        public IActionResult OutPatientQueryByDept([FromBody] dynamic dy)
        {
            JObject obj = Function.GetJobjByDy(dy);
            logger.LogWarning("请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + obj.ToString());
            string deptCode = "";
            try
            {
                deptCode = obj.GetValue("deptCode", StringComparison.OrdinalIgnoreCase).ToString();
            }
            catch (Exception)
            {
                return Function.GetErrResult("参数传递有误");
            }
            JsonResult js = this._outpatientService.GetOutPatientResult(deptCode);
            logger.LogWarning("返回参数\n" + JsonConvert.SerializeObject(js.Value));
            return new JsonResult(js.Value);
        }
    }
}
