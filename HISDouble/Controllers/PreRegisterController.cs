using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HISDouble.Interfaces;
using HISDouble.Models;
using HISDouble.Models.Base;
using log4net.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HISDouble.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PreRegisterController : ControllerBase
    {
        private readonly ILogger<PreRegisterController> _logger;
        private readonly IView_PreRegisterService _view_PreRegisterService;
        private readonly IView_contractunitService _view_ContractunitService;
        private readonly IView_PreInspectionService _view_PreInspectionService;
        private readonly IView_PacsService _view_PacsService;

        public PreRegisterController(ILogger<PreRegisterController> logger, IView_PreRegisterService view_PreRegisterService, IView_contractunitService view_ContractunitService, IView_PreInspectionService view_PreInspectionService,IView_PacsService view_PacsService)
        {
            this._logger = logger;
            this._view_PreRegisterService = view_PreRegisterService;
            this._view_ContractunitService = view_ContractunitService;
            this._view_PreInspectionService = view_PreInspectionService;
            this._view_PacsService = view_PacsService;
        }
        /// <summary>
        /// 患者信息查询    
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost, Route("PatientInformationQuery")]
        public IActionResult PatientInformationQuery([FromBody] dynamic MedicalRecordNo)
        {
            JObject j = Function.GetJobjByDy(MedicalRecordNo);
            string no = string.Empty;
            if (!j.ContainsKey("MedicalRecordNo"))
            {
                return Function.GetErrResult("请求入参出现错误!  找不到参数名为MedicalRecordNo的参数！");
            }
            no = j.GetValue("MedicalRecordNo").ToString();
           
            _logger.LogWarning(" 患者信息查询请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            JsonResult ht = _view_PreRegisterService.GetPatientQueryResult(j.GetValue("MedicalRecordNo").ToString());
            _logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));

            // Hashtable ht = accountBLL.QueryAnnex(id);
            return Ok(ht.Value);
        }
        /// <summary>
        /// 合同單位數據源
        /// </summary>
        /// <returns></returns>
        [HttpGet, Route("ContractUnitQuery")]
        public IActionResult ContractUnitQuery()
        {
            JsonResult jsonResult = this._view_ContractunitService.ContractUnitQuery();
            return Ok(jsonResult.Value);
        }
        /// <summary>
        /// 患者信息查询（预约检查）
        /// </summary>
        /// <param name="CardNo">卡号</param>
        /// <returns></returns>
        [HttpPost, Route("PatientInformationQueryByCard")]
        public IActionResult PatientInformationQueryByCard([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            string CardNo = string.Empty;
            if (!j.ContainsKey("CardNo"))
            {
                return Function.GetErrResult("请求入参出现错误!  找不到参数名为CardNo的参数！");
            }
            CardNo = j.GetValue("CardNo").ToString();
            
           
            _logger.LogWarning(" 患者信息查询（预约检查）请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            JsonResult js = this._view_PreInspectionService.PatientInformationQueryByCard(CardNo);
            _logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(js.Value));
            return Ok(js.Value);
        }
        /// <summary>
        /// 保存检查信息
        /// </summary>
        /// <param name="entity">检查信息对象</param>
        /// <returns></returns>
        [HttpPost,Route("SaveCheck")]
        public IActionResult SaveCheck([FromBody] dynamic PreInspection)
        {
            JObject j = Function.GetJobjByDy(PreInspection);
            PreInspection inspection = new PreInspection();
            string preInspection = string.Empty;
          
               bool isexist= j.ContainsKey("preInspection");
                if (!isexist)
                {
                  return  Function.GetErrResult($"请求入参出现错误，没有找到参数名为preInspection的参数");
                }
                 preInspection = j.GetValue("preInspection").ToString();
                inspection = JsonConvert.DeserializeObject<PreInspection>(preInspection);
          
            _logger.LogWarning(" 保存检查信息请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + j.ToString());
            JsonResult ht = _view_PacsService.SaveCheckData(inspection);
            _logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }
        /// <summary>
        /// 科室数,设备数据源（预约检查）
        /// </summary>
        /// <returns></returns>
        [HttpPost,Route("GetDepartment")]
        public IActionResult GetDepartment([FromBody] DiviceParams keshiCode)
        {
            _logger.LogWarning(" 科室数,设备数据源（预约检查）请求时间：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求参数：\n" + JsonConvert.SerializeObject(keshiCode));
            JsonResult ht = _view_PacsService.GetDepartmentQuery(keshiCode);
            _logger.LogWarning("请求回参：" + JsonConvert.SerializeObject(ht.Value));
            return Ok(ht.Value);
        }
    }
}
