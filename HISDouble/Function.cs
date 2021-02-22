using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace HISDouble
{
    public static class Function
    {
        public static string Err = string.Empty;
        public static int Code = 1; 
        private static IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public static string rmSql = @"select *
  from (select zjhis.fun_get_dept_name(a.dept_code) DeptName,
               '' BedsCount,
               count(*) HospitalAttendance,
               (select count(*)
                  from zjhis.fin_ipr_inmaininfo b
                 where b.dept_code = a.dept_code
                   and b.in_date > trunc(sysdate)) HospitalVisits,
               (select count(*)
                  from zjhis.fin_ipr_inmaininfo b
                 where b.dept_code = a.dept_code
                   and b.out_date > trunc(sysdate)) Discharges
          from zjhis.fin_ipr_inmaininfo a
         where a.in_state = 'I'
           and a.patient_no not like '%B%'
           and a.dept_code not in ('0120', '0124')
         group by a.dept_code
         order by a.dept_code desc)

union all

select '全部' as DeptName,
       '' as BedsCount,
       (select count(*)
          from zjhis.fin_ipr_inmaininfo a
         where a.in_state = 'I'
           and a.patient_no not like '%B%'
           and a.dept_code not in ('0120', '0124')) HospitalAttendance,
       (select count(*)
          from zjhis.fin_ipr_inmaininfo b
         where b.in_date > trunc(sysdate)) HospitalVisits,
       (select count(*)
          from zjhis.fin_ipr_inmaininfo b
         where b.out_date > trunc(sysdate)) Discharges
  from dual
  
  union all
  select '今日门诊实时挂号人次' as DeptName,
       '' as BedsCount,
       count(*) as HospitalAttendance,
       0 as HospitalAttendance,
       0 as HospitalVisits
  from zjhis.fin_opr_register a
 where a.reg_date > trunc(sysdate)
   and a.reg_date < trunc(sysdate) + 1
   and a.trans_type = '1'
   and not exists (select 1
          from zjhis.fin_opr_register b
         where b.clinic_code = a.clinic_code
           and b.trans_type = '2')";

        public static string zyfykqSql = @"select *
  from (select fun_get_dept_name(a.dept_code) DeptName,
               '' BedsCount,
               count(*) HospitalAttendance,
               (select count(*)
                  from fin_ipr_inmaininfo b
                 where b.dept_code = a.dept_code
                   and b.in_date > trunc(sysdate)) HospitalVisits,
               (select count(*)
                  from fin_ipr_inmaininfo b
                 where b.dept_code = a.dept_code
                   and b.out_date > trunc(sysdate)) Discharges
          from fin_ipr_inmaininfo a
         where a.in_state = 'I'
           and a.patient_no not like '%B%'
           and a.dept_code not in ('0120', '0124')
         group by a.dept_code
         order by a.dept_code desc)

union all

select '全部' as DeptName,
       '' as BedsCount,
       (select count(*)
          from fin_ipr_inmaininfo a
         where a.in_state = 'I'
           and a.patient_no not like '%B%'
           and a.dept_code not in ('0120', '0124')) HospitalAttendance,
       (select count(*)
          from fin_ipr_inmaininfo b
         where b.in_date > trunc(sysdate)) HospitalVisits,
       (select count(*)
          from fin_ipr_inmaininfo b
         where b.out_date > trunc(sysdate)) Discharges
  from dual
  
  union all
  select '今日门诊实时挂号人次' as DeptName,
       '' as BedsCount,
       count(*) as HospitalAttendance,
       0 as HospitalAttendance,
       0 as HospitalVisits
  from fin_opr_register a
 where a.reg_date > trunc(sysdate)
   and a.reg_date < trunc(sysdate) + 1
   and a.trans_type = '1'
   and not exists (select 1
          from fin_opr_register b
         where b.clinic_code = a.clinic_code
           and b.trans_type = '2')";
        private static string dsSql = @"select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='董市镇卫生院' or t.deptname='今日门诊实时挂号人次'
union select
       '' as JGID,
        '' as ORGANIZNAME,
       '全部' as DeptName,
       (select sum (BEDSCOUNT) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='董市镇卫生院' )as BEDSCOUNT,
       (select sum (HOSPITALATTENDANCE)from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='董市镇卫生院') as HOSPITALATTENDANCE,
       (select sum(HOSPITALVISITS) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='董市镇卫生院') as HOSPITALVISITS,
       (select sum(DISCHARGES) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='董市镇卫生院') as DISCHARGES
  from (select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='董市镇卫生院')";
        private static string afsSql = @"select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='安福寺镇中心卫生院' or t.deptname='今日门诊实时挂号人次'
union select
       '' as JGID,
        '' as ORGANIZNAME,
       '全部' as DeptName,
       (select sum (BEDSCOUNT) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='安福寺镇中心卫生院' )as BEDSCOUNT,
       (select sum (HOSPITALATTENDANCE)from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='安福寺镇中心卫生院') as HOSPITALATTENDANCE,
       (select sum(HOSPITALVISITS) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='安福寺镇中心卫生院') as HOSPITALVISITS,
       (select sum(DISCHARGES) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='安福寺镇中心卫生院') as DISCHARGES
  from (select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='安福寺镇中心卫生院')";
        private static string gjdSql = @"select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='顾家店镇卫生院' or t.deptname='今日门诊实时挂号人次'
union select
       '' as JGID,
        '' as ORGANIZNAME,
       '全部' as DeptName,
       (select sum (BEDSCOUNT) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='顾家店镇卫生院' )as BEDSCOUNT,
       (select sum (HOSPITALATTENDANCE)from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='顾家店镇卫生院') as HOSPITALATTENDANCE,
       (select sum(HOSPITALVISITS) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='顾家店镇卫生院') as HOSPITALVISITS,
       (select sum(DISCHARGES) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='顾家店镇卫生院') as DISCHARGES
  from (select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='顾家店镇卫生院')";
        private static string xlzSql = @"select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='仙女镇卫生院' or t.deptname='今日门诊实时挂号人次'
union select
       '' as JGID,
        '' as ORGANIZNAME,
       '全部' as DeptName,
       (select sum (BEDSCOUNT) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='仙女镇卫生院' )as BEDSCOUNT,
       (select sum (HOSPITALATTENDANCE)from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='仙女镇卫生院') as HOSPITALATTENDANCE,
       (select sum(HOSPITALVISITS) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='仙女镇卫生院') as HOSPITALVISITS,
       (select sum(DISCHARGES) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='仙女镇卫生院') as DISCHARGES
  from (select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='仙女镇卫生院')";
        private static string blzSql = @"select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='百里洲镇中心卫生院' or t.deptname='今日门诊实时挂号人次'
union select
       '' as JGID,
        '' as ORGANIZNAME,
       '全部' as DeptName,
       (select sum (BEDSCOUNT) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='百里洲镇中心卫生院' )as BEDSCOUNT,
       (select sum (HOSPITALATTENDANCE)from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='百里洲镇中心卫生院') as HOSPITALATTENDANCE,
       (select sum(HOSPITALVISITS) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='百里洲镇中心卫生院') as HOSPITALVISITS,
       (select sum(DISCHARGES) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='百里洲镇中心卫生院') as DISCHARGES
  from (select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='百里洲镇中心卫生院')";
        private static string qxtSql = @"select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='七星台镇卫生院' or t.deptname='今日门诊实时挂号人次'
union select
       '' as JGID,
        '' as ORGANIZNAME,
       '全部' as DeptName,
       (select sum (BEDSCOUNT) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='七星台镇卫生院' )as BEDSCOUNT,
       (select sum (HOSPITALATTENDANCE)from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='七星台镇卫生院') as HOSPITALATTENDANCE,
       (select sum(HOSPITALVISITS) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='七星台镇卫生院') as HOSPITALVISITS,
       (select sum(DISCHARGES) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='七星台镇卫生院') as DISCHARGES
  from (select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='七星台镇卫生院')";
        private static string wazSql = @"select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='问安镇中心卫生院' or t.deptname='今日门诊实时挂号人次'
union select
       '' as JGID,
        '' as ORGANIZNAME,
       '全部' as DeptName,
       (select sum (BEDSCOUNT) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='问安镇中心卫生院' )as BEDSCOUNT,
       (select sum (HOSPITALATTENDANCE)from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='问安镇中心卫生院') as HOSPITALATTENDANCE,
       (select sum(HOSPITALVISITS) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='问安镇中心卫生院') as HOSPITALVISITS,
       (select sum(DISCHARGES) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='问安镇中心卫生院') as DISCHARGES
  from (select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='问安镇中心卫生院')";
        private static string wjwSql = @"select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='枝江市卫健委' or t.deptname='今日门诊实时挂号人次'
union select
       '' as JGID,
        '' as ORGANIZNAME,
       '全部' as DeptName,
       (select sum (BEDSCOUNT) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='枝江市卫健委' )as BEDSCOUNT,
       (select sum (HOSPITALATTENDANCE)from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='枝江市卫健委') as HOSPITALATTENDANCE,
       (select sum(HOSPITALVISITS) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='枝江市卫健委') as HOSPITALVISITS,
       (select sum(DISCHARGES) from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='枝江市卫健委') as DISCHARGES
  from (select * from  fxpt.pt_view_rjxx t where  t.ORGANIZNAME='枝江市卫健委')";
        /// <summary>
        /// dynamic转jobj
        /// </summary>
        /// <param name="dy"></param>
        /// <returns></returns>
        public static JObject GetJobjByDy(dynamic dy)
        {
            return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(dy));
        }
        /// <summary>
        /// 返回成功的api数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static JsonResult GetResult<T>(T obj)
        {
            return new JsonResult(new { code = 200, data = obj, message = "成功！" });
        }
        /// <summary>
        /// 返回成功的api数据集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static JsonResult GetResultList<T>(List<T> obj)
        {
            return new JsonResult(new { code = 200, data = obj, count = obj.Count, message = "成功！" });
        }
        /// <summary>
        /// 返回错误的api数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static JsonResult GetErrResult(string obj)
        {
            return new JsonResult(new { code = 500, data = obj, message = "调用失败！" });
        }
        /// <summary>
        /// 新增的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dbConnection"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Insert<T>(T obj, System.Data.IDbConnection dbConnection, ref IDbTransaction trans) where T : class
        {

            long result = SqlExec.Insert(dbConnection, obj, trans);
            // long result = dbConnection.Execute(SqlStr,referral, trans);
            if (result > 0)
            {
                return 1;
            }
            else
            {
                trans.Rollback();
                DBFunction.CloseConnService(dbConnection);
                return -1;
            }
        }
        /// <summary>
        /// 修改的方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="dbConnection"></param>
        /// <param name="trans"></param>
        /// <returns></returns>
        public static int Update<T>(T obj, System.Data.IDbConnection dbConnection, ref IDbTransaction trans) where T : class
        {

            bool result = SqlExec.Update<T>(dbConnection, obj, trans);
            // long result = dbConnection.Execute(SqlStr,referral, trans);
            if (result)
            {
                return 1;
            }
            else
            {
                return -1;
            }
        }
        /// <summary>
        /// 调用成功（增删改）
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static JsonResult GetSuccessResult(string obj)
        {
            return new JsonResult(new { code = 200, data = obj, message = "调用成功！" });
        }
        /// <summary>
        /// 获取get请求
        /// </summary>
        /// <param name="Url"></param>
        /// <returns></returns>
        public static string GetRequest(string Url)
        {
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(Url);
            req.Method = "get";
            req.ContentType = "application/json;charset=utf-8";
            WebResponse response = req.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);
            return streamReader.ReadToEnd();

        }
        /// <summary>
        /// 获取Post请求
        /// </summary>
        /// <param name="Url"></param>
        /// <param name="Data"></param>
        /// <returns></returns>
        public static string PostRequest(string Url, string Data)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);
            request.Method = "post";
            request.ContentType = "application/json;charset=utf-8";
            byte[] bytearr = Encoding.UTF8.GetBytes(Data);
            request.ContentLength = bytearr.Length;
            Stream stream = request.GetRequestStream();
            stream.Write(bytearr, 0, bytearr.Length);
            stream.Close();
            WebResponse response = request.GetResponse();
            stream = response.GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();

        }
        /// <summary>
        /// 对象转成jObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tobj"></param>
        /// <returns></returns>
        public static JObject getObjByJobj<T>(T tobj)
        {
            string tstr = JsonConvert.SerializeObject(tobj);
            return JsonConvert.DeserializeObject<JObject>(tstr);
        }
        /// <summary>
        /// 将dataset数据集转换成json对象
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static ArrayList getJObject(DataSet ds)
        {
            ArrayList arr = new ArrayList();
            if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    Dictionary<string, string> j = new Dictionary<string, string>();
                    for (int i = 0; i < item.ItemArray.Length; i++)
                    {
                        j.Add(ds.Tables[0].Columns[i].ColumnName.ToString(), item.ItemArray[i].ToString());
                    }
                    arr.Add(j);
                }
            }
            return arr;
        }
        /// <summary>
        /// 获取用户姓名
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static string GetName(string Name, IDbConnection connection, string HosCode)
        {
            string userName = "";
            switch (HosCode)
            {
                case "01":
                    string sql = $"select e.empl_name from zjhis.com_employee e where e.empl_code='{Name}'";// and e.valid_state='1'
                    dynamic dy = connection.QueryFirst(sql);
                    JObject j = GetJobjByDy(dy);
                    HosCode = j.GetValue("EMPL_NAME").ToString();
                    break;
                default:
                    connection.ConnectionString = root.GetConnectionString("OrclDBPacs");
                    string sqxz = $"select e.XM from  fxpt.DIC_PRACTITIONER e where e.GH='{Name}'";// and e.valid_state='1'
                    dynamic dyxz = connection.QueryFirst(sqxz);
                    JObject jxz = GetJobjByDy(dyxz);
                    HosCode = jxz.GetValue("XM").ToString();
                    break;
            }
            return HosCode;
        }
        /// <summary>
        /// 对象集合转换成JObj集合
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="List"></param>
        /// <returns></returns>
        public static List<JObject> getJobjListByList<T>(List<T> List)
        {
            List<JObject> jlist = new List<JObject>();
            foreach (T item in List)
            {
                JObject j = getObjByJobj<T>(item);
                jlist.Add(j);
            }
            return jlist;
        }
        /// <summary>
        /// 获取不同医院数据库的名称
        /// </summary>
        /// <param name="jgName"></param>
        /// <returns></returns>
        public static string GetSqlName(string jgName)
        {
            switch (jgName)
            {
                case "rm":
                    return "ZJhisConnStr";
                case "zy":
                    return "ZYhisConnStr";
                case "fy":
                    return "FYhisConnStr";
                case "kq":
                    return "KQhisConnStr";
                case "xz":
                    return "OrclDBPacs";
                default:
                    return "OrclDBPacs";
            }
          
        }
        public static string getSqlByName(string jgName)
        {
            switch (jgName)
            {
                case "rm":
                    return rmSql;
                case "zy":
                    return zyfykqSql;
                case "fy":
                    return zyfykqSql;
                case "kq":
                    return zyfykqSql;
                case "ds":
                    return  dsSql;
                case "afs":
                    return afsSql;
                case "waz":
                    return wazSql;
                case "qxt":
                    return qxtSql;
                case "blz":
                    return blzSql;
                case "xlz":
                    return xlzSql;
                case "gjd":
                    return gjdSql;
                case "wjw":
                    return wjwSql;
                default:
                    return "";
            }
        }
        /// <summary>
        /// 将object转换成T
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="Obj"></param>
        /// <returns></returns>
        public static T GetT<T>(object Obj)
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(Obj));
        }
    }
}
