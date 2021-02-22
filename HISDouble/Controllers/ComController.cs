using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using HISDouble.DapperConfig;
using HISDouble.Interfaces;
using HISDouble.Models;
using log4net.Core;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static HISDouble.Factory.DapperContext;
using ServiceReference3;
using Oracle.ManagedDataAccess.Client;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace HISDouble.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ComController : ControllerBase
    {
        private readonly ILogger<ComController> _logger;
        private readonly IOutpatientService _outpatientService;
        private readonly ICheckInDataListService _checkInDataListService;
        private readonly IDapperContext _dapperContext;
        private readonly IHospitaldicService _hospitaldicService;
        private readonly SqlServiceSoap _sqlService;

        //private readonly IOracleFactory _oracleFactory;
        private string ErrMsg = string.Empty;
        private int ErrCode = 0;

        public ComController(ILogger<ComController> logger, IOutpatientService outpatientService, ICheckInDataListService checkInDataListService, IDapperContext dapperContext, IHospitaldicService hospitaldicService, SqlServiceSoap sqlService)/*IOracleFactory oracleFactory*/
        {
            _logger = logger;
            _outpatientService = outpatientService;
            _checkInDataListService = checkInDataListService;
            this._dapperContext = dapperContext;
            this._hospitaldicService = hospitaldicService;
            this._sqlService = sqlService;
            //this._oracleFactory = oracleFactory;
        }
        // GET: api/<ComController>
        [HttpGet, Route("GetCompany")]
        public JObject GetCompany()
        {
            string Url = "https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid=ww30de27d5efb6a9db&corpsecret=svUTeBOxvOq_4PudJhfXYg08-Gc8voUt65B2-nw0CAo";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.GetRequest(Url));
            return jObject;
        }
        #region 智慧医院接口
        [HttpGet, Route("GetDepartments")]
        public JObject GetDepartments()
        {
            string Url = "http://111.47.69.7:6000/api/com/getAllDeptroom";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.PostRequest(Url, ""));
            return jObject;
        }
        /// <summary>
        /// 查询医生
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("GetDoctors")]
        public IActionResult GetDoctors([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            if (!j.ContainsKey("data"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为data的参数");
            }
            string data = j.GetValue("data").ToString();
            string Url = "http://111.47.69.7:6000/api/com/getDoctorScheduling";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.PostRequest(Url, data));
            return Ok(jObject);
        }
        /// <summary>
        /// 预约挂号
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("Prereg")]
        public IActionResult Prereg([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            if (!j.ContainsKey("data"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为data的参数");
            }
            string data = j.GetValue("data").ToString();
            string Url = "http://111.47.69.7:6000/api/opb/appointment";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.PostRequest(Url, data));
            return Ok(jObject);
        }

        /// <summary>
        /// 预约挂号确认
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("PreregisterConfrim")]
        public IActionResult PreregisterConfrim([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            if (!j.ContainsKey("data"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为data的参数");
            }
            string data = j.GetValue("data").ToString();
            string Url = "http://111.47.69.7:6000/api/opb/updateAppointmentStatus";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.PostRequest(Url, data));
            return new JsonResult(jObject);
        }
        /// <summary>
        /// 取消预约挂号
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("CancelPrereg")]
        public IActionResult CancelPrereg([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            if (!j.ContainsKey("rec_id"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为rec_id的参数");
            }
            string data = "{\"appointmentId\":\"" + j.GetValue("rec_id").ToString() + "\",\"appointmentStatus\":\"0\",\"payStatus\":\"0\",\"total\":\"0\",\"payType\":\"1\"}";
            string Url = "http://111.47.69.7:6000/api/opb/updateAppointmentStatus";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.PostRequest(Url, data));
            if (jObject.GetValue("msg").ToString() == "预约已取消！")
            {
                //修改code值
                jObject.Remove("code");
                jObject.Add("code", 200);
            }
            return Ok(jObject);
        }
        /// <summary>
        /// 预约挂号退号
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("BackPrereg")]
        public IActionResult BackPrereg([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            if (!j.ContainsKey("tiket_no"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为tiket_no的参数");
            }
            string data = "{\"tiket_no\":\"" + j.GetValue("tiket_no").ToString()+"\"}";
            string Url = "http://111.47.69.7:6000/api/opb/registeredBack";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.PostRequest(Url, data));
            if (jObject.GetValue("msg").ToString() == "退号成功！")
            {
                //修改code值
                jObject.Remove("code");
                jObject.Add("code", 200);
            }
            return Ok(jObject);
        }
        /// <summary>
        /// 获取医生号源
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("GetTimeNum")]
        public IActionResult GetTimeNum([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            if (!j.ContainsKey("data"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为data的参数");
            }
            string data = j.GetValue("data").ToString();
            string Url = "http://111.47.69.7:6000/api/com/getDoctorAppointmentOrders";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.PostRequest(Url, data));
            return Ok(jObject);
        }
        #endregion

        [HttpPost, Route("Department")]
        public IActionResult Department([FromBody] dynamic dy)
        {
            JObject job = Function.GetJobjByDy(dy);
            string token = ""; int id = 0;
            if (!job.ContainsKey("token"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为token的参数");
            }
            if (!job.ContainsKey("id"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为id的参数");
            }
            token = job.GetValue("token").ToString();
            id = Convert.ToInt32(job.GetValue("id").ToString());


            string Url = $"https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token={token}&id={id}";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.GetRequest(Url));
            return Ok(jObject);
        }
        /// <summary>
        /// 企业微信获取部门用户信息
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        [HttpPost, Route("getSimpleList")]
        public IActionResult getSimpleList([FromBody] dynamic dy)
        {
            JObject job = Function.GetJobjByDy(dy);
            string access_token = ""; int department_id = 0; int fetch_child = 0;
            if (!job.ContainsKey("access_token"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为access_token的参数");
            }
            if (!job.ContainsKey("department_id"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为department_id的参数");
            }
            if (!job.ContainsKey("fetch_child"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为fetch_child的参数");
            }

            access_token = job.GetValue("access_token").ToString();
            department_id = Convert.ToInt32(job.GetValue("department_id").ToString());
            fetch_child = Convert.ToInt32(string.IsNullOrEmpty(job.GetValue("fetch_child").ToString()) ? "0" : job.GetValue("fetch_child").ToString());


            string Url = $"https://qyapi.weixin.qq.com/cgi-bin/user/simplelist?access_token={access_token}&department_id={department_id}&fetch_child={fetch_child}";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.GetRequest(Url));
            return Ok(jObject);
        }
        [HttpPost, Route("GetdeptList")]
        public IActionResult GetdeptList([FromBody] dynamic dy)
        {
            JObject obj = Function.GetJobjByDy(dy);
            string useridlist = ""; string token = ""; int opencheckindatatype = 0;
            int starttime = 0; int endtime = 0;
            if (!obj.ContainsKey("opencheckindatatype"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为opencheckindatatype的参数");
            }
            if (!obj.ContainsKey("starttime"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为starttime的参数");
            }
            if (!obj.ContainsKey("endtime"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为endtime的参数");
            }
            if (!obj.ContainsKey("useridlist"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为useridlist的参数");
            }
            if (!obj.ContainsKey("token"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为token的参数");
            }
            opencheckindatatype = int.Parse(obj.GetValue("opencheckindatatype").ToString());
            starttime = int.Parse(obj.GetValue("starttime").ToString());
            endtime = int.Parse(obj.GetValue("endtime").ToString());
            useridlist = obj.GetValue("useridlist").ToString();
            token = obj.GetValue("token").ToString();

            string data = "{\"opencheckindatatype\":" + opencheckindatatype + ",\"starttime\": " + starttime + ",\"endtime\":  " + endtime + ",\"useridlist\":" + useridlist + "}";
            string Url = $"https://qyapi.weixin.qq.com/cgi-bin/checkin/getcheckindata?access_token={token}";
            JObject jObject = JsonConvert.DeserializeObject<JObject>(Function.PostRequest(Url, data));
            return Ok(jObject);
        }

        /// <summary>
        /// 保存打卡信息
        /// </summary>
        /// <param name="entity">打卡实体</param>
        /// <returns></returns>
        [HttpPost, Route("SaveCheck")]
        public IActionResult SaveCheck([FromBody] dynamic obj)
        {
            JObject jboj = Function.GetJobjByDy(obj);
            _logger.LogWarning("保存打卡信息请求时间：\n" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n请求数据：" + jboj.ToString());
            CheckInDataList checkInData = new CheckInDataList();
            if (!jboj.ContainsKey("data"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为data的参数");
            }
            if (!jboj.ContainsKey("istarttime"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为istarttime的参数");
            }
            if (!jboj.ContainsKey("endtime"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为endtime的参数");
            }
            if (!jboj.ContainsKey("dept_id"))
            {
                return Function.GetErrResult($"请求入参出现错误，没有找到参数名为dept_id的参数");
            }
            checkInData.CheckData = JsonConvert.DeserializeObject<List<View_GetCheckindata>>(jboj.GetValue("data").ToString());
            checkInData.StartTime = jboj.GetValue("istarttime").ToString();
            checkInData.EndTime = jboj.GetValue("endtime").ToString();
            checkInData.DEPT_ID = jboj.GetValue("dept_id").ToString();
            JsonResult ht = _checkInDataListService.SaveCheckindata(checkInData);
            _logger.LogWarning("保存打卡信息请求时间：\n" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n返回数据：" + JsonConvert.SerializeObject(ht.Value));
            return ht;
        }
        #region 国家绩效考核指标查询接口
        [HttpPost, Route("QueryZBInfoByDate")]
        public IActionResult QueryZBInfoByDate([FromBody] dynamic dynamic)
        {

            JObject jobj = Function.GetJobjByDy(dynamic);
            Console.WriteLine("请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\n国家绩效考核指标查询接口的入参" + jobj.ToString());
            string date = Convert.ToDateTime(jobj.GetValue("date").ToString()).ToString("yyyy-MM-01");
            string sql = $"select to_number(replace(p.target_code,'txtbox')) as target_code,p.target_name,p.target_value from zjhis.per_for_mance p where p.assessment_time=to_date('{date}','yyyy-mm-dd')";
            List<JObject> list = new List<JObject>();
            var dt = this._dapperContext.GetSqlSet(sql, "ZJhisConnStr", ref this.ErrCode);
            /* "TARGET_CODE": 11.0,
              "TARGET_NAME": "11、电子病历应用水平评级",
    "TARGET_VALUE": "四级"*/

            for (int i = 0; i < 28; i++)
            {
                List<JObject> blist = new List<JObject>();
                foreach (var item in dt)
                {
                    JObject Jobj = Function.getObjByJobj<object>(item);
                    blist.Add(Jobj);
                }

                if (blist.Where(b => (Convert.ToInt32(b.GetValue("TARGET_CODE").ToString()) == i + 1)).Count() > 0)
                {
                    list.Add(blist.Where(b => Convert.ToInt32(b.GetValue("TARGET_CODE").ToString()) == i + 1).FirstOrDefault());
                }
                else
                {
                    JObject obj = new JObject();
                    obj.Add("TARGET_CODE", i + 1);
                    obj.Add("TARGET_NAME", "");
                    obj.Add("TARGET_VALUE", "");
                    list.Add(obj);
                }


            }
            if (ErrCode == -1)
            {
                Function.GetErrResult(sql + "此sql查询失败");
            }
            if (list.Count == 0 || list == null)
            {
                return new JsonResult(new { msg = "没有找到如何指标的信息！", data = "查询结果为空", code = 404 });
            }

            return new JsonResult(new { msg = "查询成功！", data = list, code = 200 });
        }
        #endregion

        #region 科室查询接口
        [HttpPost, Route("QueryDepart")]
        public IActionResult QueryDepart()
        {

            Console.WriteLine(" 科室查询接口请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            string sql = "select *from hrmdepartment";
            var dt = this._dapperContext.GetSqlSet(sql, "OracleUserInfo", ref this.ErrCode);
            if (ErrCode == -1)
            {
                Function.GetErrResult(sql + "此sql查询失败");
            }
            if (dt.Count == 0 || dt == null)
            {
                return new JsonResult(new { msg = "没有找到如何指标的信息！", data = "查询结果为空", code = 404 });
            }

            return new JsonResult(new { msg = "查询成功！", data = dt, code = 200 });
        }
        #endregion

        #region 医保接口
        [HttpPost, Route("QueryYB")]
        public IActionResult QueryYB([FromBody] dynamic dy)
        {
            JObject jobj = Function.GetJobjByDy(dy);

            Console.WriteLine(" 医保接口请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + jobj.ToString());
            if (!jobj.ContainsKey("start"))
            {
                return Function.GetErrResult("请求失败！没有找到参数名为start参数");
            }
            if (!jobj.ContainsKey("end"))
            {
                return Function.GetErrResult("请求失败！没有找到参数名为end参数");
            }
            string start = Convert.ToDateTime(jobj.GetValue("start").ToString()).ToString("yyyy-MM-dd 00:00:00");
            string end = Convert.ToDateTime(jobj.GetValue("end").ToString()).ToString("yyyy-MM-dd 23:59:59");
            string sql = @"select t.hosname,
       t.pactname,
       t.typename,
       sum(t.pub_cost) as pubcost,
       sum(t.pay_cost) as paycost,
       sum(t.over_cost) as overcost,
       sum(t.hos_cost) as hoscost,
       sum(t.offical_cost) as officalcost,
       sum(t.second_cost) as secondcost,
       sum(t.clivl_cost) as clivlcost,
       sum(t.lxpub_cost) as lxpubcost,
       sum(t.jkfp_cost) as jkfpcost,
       sum(t.mzyl_cost) as mzylcost,
       sum(t.cblyycd_cost) as cblyycdcost,
       sum(t.yfbz_cost) as yfbzcost,
       sum(t.tot_cost) as totcost,
       sum(t.own_cost) as owncost,
       t.totalamount as totamount,
       t.totalamountspent as totalamountspent,
       t.percentage as percentage,
       t.wz_cost1 as wzcost1,
       t.wz_cost2 as wzcost2,
       t.wz_cost3 as wzcost3,
       t.wz_cost4 as wzcost4,
       t.wz_cost5 as wzcost5,
       t.wz_count1 as wzcount1,
       t.wz_count2 as wzcount2,
       t.wz_count3 as wzcount3,
       t.wz_count4 as wzcount4,
       t.wz_count5 as wzcount5,
       t.jf_cost as jfcount,
       to_char(t.jf_reason) as jfreason
  from si_inmaintotal t
 where t.balance_date >=
       to_date('{0}', 'yyyy-mm-dd HH24:mi:ss')
   and t.balance_date <=
       to_date('{1}', 'yyyy-mm-dd HH24:mi:ss')
 group by t.hosname,
          t.pactname,
          t.typename,
          t.totalamount,
          t.totalamountspent,
          t.percentage,
          t.wz_cost1,
          t.wz_cost2,
          t.wz_cost3,
          t.wz_cost4,
          t.wz_cost5,
          t.wz_count1,
          t.wz_count2,
          t.wz_count3,
          t.wz_count4,
          t.wz_count5,
          t.jf_cost,
          to_char(t.jf_reason),
          to_char(t.balance_date, 'yyyy-mm')
 order by t.hosname, t.pactname, t.typename asc";
            sql = string.Format(sql, start, end);
            var dt = this._dapperContext.getList(sql.ToUpper(), "OraclezjhisYB", ref this.ErrCode);
            // JsonResult js = this._dapperContext.GetResult(dt, "HosName".ToUpper(),ref ErrCode);
            if (ErrCode == -1)
            {
                Function.GetErrResult(sql + "此sql查询失败");
            }
            if (dt.Count == 0 || dt == null)
            {
                return new JsonResult(new { msg = "没有找到如何指标的信息！", data = "查询结果为空", code = 404 });
            }

            return Function.GetResultList<JObject>(dt);
        }
        #endregion

        #region 测试api
        //[HttpPost ,Route("Test")]
        //public IActionResult Test()
        //{
        //    string Start = "2020-10-20 00:00:00";
        //    string End = "2020-10-20 23:59:59";
        //    this._oracleFactory.Add("BeginDate", DbType.Date, ParameterDirection.Input, DateTime.Parse(Start));
        //    this._oracleFactory.Add("EndDate", DbType.Date, ParameterDirection.Input, DateTime.Parse(End));
        //    this._oracleFactory.Add("SID", DbType.String, ParameterDirection.Output,"");
        //    string Result = this._oracleFactory.Get<string>("pkg_ybtotal.insertinfo", "SID");
        //    return Ok()
        //}
        #endregion

        #region 医疗机构查询
        [HttpPost, Route("QueryHos")]
        public IActionResult QueryHos()
        {

            Console.WriteLine(" 医疗机构查询接口请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            JsonResult js = this._hospitaldicService.GetHosResult();

            return js;
        }
        #endregion

        #region 查询乡镇检查项目
        [HttpPost, Route("QueryCheckItem")]
        public IActionResult QueryCheckItem([FromBody] dynamic dy)
        {
            Console.WriteLine(" 乡镇卫生院检查接口请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            JObject j = Function.GetJobjByDy(dy);
            if (!j.ContainsKey("typeName"))
            {
                return Function.GetErrResult("入参传入错误，没有找到参数名为typeName的参数");
            }
            if (!j.ContainsKey("name"))
            {
                return Function.GetErrResult("入参传入错误，没有找到参数名为name的参数");
            }
            if (!j.ContainsKey("orgCode"))
            {
                return Function.GetErrResult("入参传入错误，没有找到参数名为orgCode的参数");
            }
            if (!j.ContainsKey("begin"))
            {
                return Function.GetErrResult("入参传入错误，没有找到参数名为begin的参数");
            }
            if (!j.ContainsKey("end"))
            {
                return Function.GetErrResult("入参传入错误，没有找到参数名为begin的参数");
            }
            string name =string.IsNullOrEmpty(j.GetValue("name").ToString())?"ALL" : j.GetValue("name").ToString();

            string sql = $"select * from fxpt.view_ws_rmyy  w where (w.lx like '%{j.GetValue("typeName",StringComparison.OrdinalIgnoreCase).ToString()}%' or '全部'='{j.GetValue("typeName", StringComparison.OrdinalIgnoreCase).ToString()}') and (w.brxm like '%{name}%' or '{name}'='ALL')  and (w.organizcode = '{j.GetValue("orgCode", StringComparison.OrdinalIgnoreCase).ToString()}' or 'ALL'='{j.GetValue("orgCode", StringComparison.OrdinalIgnoreCase).ToString()}') and w.kdrq >= to_date('{j.GetValue("begin", StringComparison.OrdinalIgnoreCase).ToString()}', 'yyyy-mm-dd hh24:mi:ss') and w.kdrq < to_date('{j.GetValue("end", StringComparison.OrdinalIgnoreCase).ToString()}', 'yyyy-mm-dd hh24:mi:ss')";
            var dt = this._dapperContext.GetSqlSet(sql, "OrclDBPacs", ref this.ErrCode);
            if (ErrCode == -1)
            {
                Function.GetErrResult(sql + "此sql查询失败");
            }
            if (dt.Count == 0 || dt == null)
            {
                return new JsonResult(new { msg = "没有找到任何检查项目的信息！", data = "查询结果为空", code = 404 });
            }

            return new JsonResult(new { msg = "查询成功！", data = dt, code = 200 });
        }
        #endregion

        #region 电子发票查询接口
        [HttpPost, Route("QueryEinvoiceBill")]
        public IActionResult QueryEinvoiceBill([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            Console.WriteLine(" 枝江市人民医院电子发票接口请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "电子发票请求入参" + j.ToString());

            if (!j.ContainsKey("patientId"))
            {
                return Function.GetErrResult("入参传入错误，没有找到参数名为patientId的参数");
            }

            if (!j.ContainsKey("begin"))
            {
                return Function.GetErrResult("入参传入错误，没有找到参数名为begin的参数");
            }
            if (!j.ContainsKey("end"))
            {
                return Function.GetErrResult("入参传入错误，没有找到参数名为begin的参数");
            }
            string start = j.GetValue("begin", StringComparison.OrdinalIgnoreCase).ToString();
            string End = j.GetValue("end", StringComparison.OrdinalIgnoreCase).ToString();
            string PatientId = j.GetValue("patientId", StringComparison.OrdinalIgnoreCase).ToString();
            if (string.IsNullOrEmpty(start) || string.IsNullOrEmpty(End))
            {
                start = System.DateTime.Now.AddDays(-30).ToString("yyyy-MM-dd HH:mm:ss");
                End = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            }
            string sql = "select  e.inpatient_no as \"inpatientno\", e.patient_no as \"patientno\", e.card_no as  \"cardno\",  e.hisinvoice_no as \"hisinvoiceno\",  e.his_name as \"hisname\",  replace(to_char(e.pictureurl),'http://172.22.156.72:7001','https://api.lizhi.co') as \"pictureurl\",  e.tot_cost as \"totcost\", e.oper_date as \"operdate\" from zjhis.fin_com_einvoicebill e where e.vaild_flag = '1' and e.cancel_flag = '1' and e.exe_flag = '1'  and e.patient_no is not null   and e.patient_no = '{0}'  and e.oper_date >= to_date('{1}', 'yyyy-mm-dd hh24:mi:ss') and e.oper_date < to_date('{2}', 'yyyy-mm-dd hh24:mi:ss') union all  select e.inpatient_no as \"inpatientno\", e.patient_no as \"patientno\", e.card_no as  \"cardno\",  e.hisinvoice_no as \"hisinvoiceno\", e.his_name as \"hisname\", replace(to_char(e.pictureurl),'http://172.22.156.72:7001','https://api.lizhi.co') as \"pictureurl\",  e.tot_cost as \"totcost\",  e.oper_date as \"operdate\" from zjhis.fin_com_einvoicebill e where e.vaild_flag = '1' and e.cancel_flag = '1'  and e.exe_flag = '1'   and e.card_no is not null  and e.card_no = '{0}' and e.oper_date >= to_date('{1}', 'yyyy-mm-dd hh24:mi:ss')   and e.oper_date < to_date('{2}', 'yyyy-mm-dd hh24:mi:ss')";
            sql = string.Format(sql, PatientId, start, End);
            var dt = this._dapperContext.GetSqlSet(sql, "ZJhisConnStr", ref this.ErrCode);
            if (ErrCode == -1)
            {
                Function.GetErrResult(sql + "此sql查询失败");
            }
            if (dt.Count == 0 || dt == null)
            {
                return new JsonResult(new { msg = "没有找到任何电子发票的信息！", data = "查询结果为空", code = 404 });
            }

            return new JsonResult(new { msg = "查询成功！", data = dt, code = 200 });
        }
        #endregion

        #region 住院日结
        [HttpPost, Route("GetDayBalance")]
        public IActionResult GetDayBalance([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            Console.WriteLine(" 住院日结接口请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "参数是：" + j.ToString());
            if (!j.ContainsKey("code"))
            {
                return Function.GetErrResult("入参出现错误！没有找到参数名为code的参数");
            }
            string code = j.GetValue("code").ToString();
            string sql = Function.getSqlByName(code);
            string sqlName = Function.GetSqlName(code);
            if (string.IsNullOrEmpty(sql) || string.IsNullOrEmpty(sqlName))
            {
                return Function.GetErrResult("您传入的参数有误请检查参数的入参！");
            }
            var dt = this._dapperContext.getList(sql.ToUpper(), sqlName, ref this.ErrCode);
            // JsonResult js = this._dapperContext.GetResult(dt, "HosName".ToUpper(),ref ErrCode);
            if (ErrCode == -1)
            {
                Function.GetErrResult(sql + "此sql查询失败");
            }
            if (dt.Count == 0 || dt == null)
            {
                return new JsonResult(new { msg = "没有找到住院日结的信息！", data = "查询结果为空", code = 404 });
            }

            return Function.GetResultList<JObject>(dt);
        }
        #endregion

        #region 人民医院科室查询
        [HttpPost, Route("RMDeptQuery")]
        public IActionResult RMDeptQuery([FromBody] dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            Console.WriteLine(" 人民医院科室查询接口请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "参数是：" + j.ToString());
            if (!j.ContainsKey("Type"))
            {
                return Function.GetErrResult("入参出现错误！没有找到参数名为Type的参数");
            }
            string type = j.GetValue("Type").ToString();
            string code = "";
            if (type == "1")
            {
                code = "I";
            }
            else if (type == "2")
            {
                code = "C";
            }
            else
            {
                code = "O";
            }

            string sql = "select d.dept_code as \"deptCode\", d.dept_name as \"deptName\",d.dept_address as \"address\" from zjhis.com_department d  where d.dept_type = '" + code.ToString() + "' and d.valid_state = '1' order by d.simple_name asc";
            var dt = this._dapperContext.GetSqlSet(sql, "ZJhisConnStr", ref this.ErrCode);
            if (ErrCode == -1)
            {
                Function.GetErrResult(sql + "此sql查询失败");
            }
            if (dt.Count == 0 || dt == null)
            {
                return new JsonResult(new { msg = "没有找到如何指标的信息！", data = "查询结果为空", code = 404 });
            }

            return new JsonResult(new { msg = "查询成功！", data = dt, code = 200 });
        }
        #endregion

        #region 人民医院人员值班查询
        [HttpPost, Route("RMZBQuery1")]
        public IActionResult RMZBQuery1()
        {

            Console.WriteLine(" 人民医院人员值班查询接口请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

            string sql = @"select * from  T_SQUENCE_XXK 
where squence_time>= (select CONVERT(varchar(10), GETDATE() - 1, 120)) 
and squence_time<= (select CONVERT(varchar(10), GETDATE(), 120))  
and squence_name<>'夜休' and squence_name<>'下夜班'
 and(squence_name like('%夜%') or squence_name like('%24%')
 or squence_name like('%晚%') or squence_name = '值班'  or  squence_name = 'P'  or squence_name = 'N')
 order by dept_name,squence_time";

            var dt = DBFunction.GetSqlServerList(sql);
            if (dt.Count == 0 || dt == null)
            {
                return new JsonResult(new { msg = "没有找到如何指标的信息！", data = "查询结果为空", code = 404 });
            }

            return new JsonResult(new { msg = "查询成功！", data = dt, code = 200 });




        }
        #endregion

        #region 人民医院人员值班查询
        [HttpPost, Route("RMZBQuery")]
        public IActionResult RMZBQuery()
        {

            Console.WriteLine(" 人民医院人员值班查询接口请求日期：" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")  );
            string sql = @"select * from  T_SQUENCE_XXK 
where squence_time>= (select CONVERT(varchar(10), GETDATE() - 1, 120)) 
and squence_time<= (select CONVERT(varchar(10), GETDATE(), 120))  
and squence_name<>'夜休' and squence_name<>'下夜班'
 and(squence_name like('%夜%') or squence_name like('%24%')
 or squence_name like('%晚%') or squence_name = '值班'  or  squence_name = 'P'  or squence_name = 'N')
 order by dept_name,squence_time";

            //  SqlServiceSoapClient sqlservice = new SqlServiceSoapClient( SqlServiceSoapClient.EndpointConfiguration.SqlServiceSoap);
            string Result =this. _sqlService.GetEmployeeList(new GetEmployeeListRequest() { Body=new GetEmployeeListRequestBody() { sql=sql} }).Body.GetEmployeeListResult;
            //ArrayList arr
            ArrayList dt = JsonConvert.DeserializeObject<ArrayList>(Result);
            if (dt.Count == 0 || dt == null)
            {
                return new JsonResult(new { msg = "没有找到如何指标的信息！", data = "查询结果为空", code = 404 });
            }

            return new JsonResult(new { msg = "查询成功！", data = dt, code = 200 });

        }
        #endregion

        #region 人民医院排班医生查询接口
        [HttpPost,Route("QueryPBDoct")]
        public IActionResult QueryPBDoct([FromBody]  dynamic dy)
        {
            JObject j = Function.GetJobjByDy(dy);
            Console.WriteLine("人民医院排班医生查询接口数据，请求时间"+System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")+"请求数据："+j.ToString());
            string deptId = j.GetValue("deptid", StringComparison.OrdinalIgnoreCase).ToString();
            string Docdate = j.GetValue("Docdate", StringComparison.OrdinalIgnoreCase).ToString();
            List<string> paramList= new List<string>() { "deptId", "Docdate", "ResultSet", "ReturnCode", "ErrorMsg" };
            List<OracleDbType> commandTypes = new List<OracleDbType>(){ OracleDbType.Varchar2,OracleDbType.Varchar2,OracleDbType.RefCursor,OracleDbType.Int32,OracleDbType.Varchar2};
            List<ParameterDirection> directions = new List<ParameterDirection>() { ParameterDirection.Input,ParameterDirection.Input,ParameterDirection.Output,ParameterDirection.Output,ParameterDirection.Output};
            List<object> objlist = new List<object>() { deptId,Docdate,"","",""};
          var res=this._dapperContext.ExecProcedure("zjhis.PKG_SXZZ.QueryPBList", paramList, commandTypes, directions,objlist,ref ErrMsg,ref ErrCode);
            if (ErrCode==-1)
            {
                return Function.GetErrResult(ErrMsg);
            }
            if (res.Count==0||res==null)
            {
                return Function.GetErrResult("数据查询为空！");
            }
            return Function.GetResultList<object>(res);
        }
        #endregion
    }
}
