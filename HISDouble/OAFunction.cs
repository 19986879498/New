using HISDouble.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ServiceReference1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble
{
    public static class OAFunction
    {
        public static int Code = 0;
        public static string Err = "";
        /// <summary>
        /// 获取工作流对象
        /// </summary>
        /// <param name="jobj"></param>
        /// <returns></returns>
        public static WorkflowRequestInfo GetWorkflowRequestInfo(JObject jobj)
        {
            string workInfo = jobj.GetValue("WorkflowRequestInfo").ToString();
            WorkflowRequestInfo info = new WorkflowRequestInfo();
            List<WorkflowRequestTableRecord> worklist = new List<WorkflowRequestTableRecord>();
            try
            {
                info = JsonConvert.DeserializeObject<WorkflowRequestInfo>(workInfo);

            }
            catch (Exception ex)
            {
                Code = -1;
                Err = "json数据格式发生错误！" + ex.Message;
                return null;
            }
            var test = JsonConvert.DeserializeObject<JObject>(workInfo.ToString()).GetValue("workflowMainTableInfo").ToString();
            JObject obj = JsonConvert.DeserializeObject<JObject>(test);
            List<JObject> obj2 = JsonConvert.DeserializeObject<List<JObject>>(obj.GetValue("requestRecords").ToString());
            try
            {
                foreach (var item in obj2)
                {
                    JObject jobjreq = Function.getObjByJobj<object>(item.GetValue("WorkflowRequestTableRecord"));
                    List<WorkflowRequestTableField> wlist = new List<WorkflowRequestTableField>();
                    List<JObject> jlist = JsonConvert.DeserializeObject<List<JObject>>(JsonConvert.SerializeObject(jobjreq.GetValue("workflowRequestTableFields")));
                    foreach (var item1 in jlist)
                    {
                        wlist.Add(new WorkflowRequestTableField()
                        {
                            fieldName = item1.GetValue("fieldName").ToString(),
                            fieldValue = item1.GetValue("fieldValue").ToString(),
                            fieldOrder = int.Parse(item1.GetValue("fieldOrder").ToString()),
                            view = bool.Parse(item1.GetValue("view").ToString()),
                            edit = bool.Parse(item1.GetValue("edit").ToString()),
                            mand = bool.Parse(item1.GetValue("mand").ToString())
                        });
                    }
                    worklist.Add(new WorkflowRequestTableRecord()
                    {
                        recordOrder = int.Parse(jobjreq.GetValue("recordOrder").ToString()),
                        workflowRequestTableFields = wlist.ToArray()
                    });

                }
            }
            catch (Exception ex)
            {
                Code = -1;
                Err = "请检查参数名称是否正确！" + ex.Message;
                return info;
            }
            info.workflowMainTableInfo.requestRecords = worklist.ToArray();
            return info;
        } 
        /// <summary>
        /// 返回状态码的信息
        /// -1：创建流程失败
        ///-2：没有创建权限
        ///-3：创建流程失败
        ///-4：字段或表名不正确
        ///-5：更新流程级别失败
        ///-6：无法创建流程待办任务
        ///-7：流程下一节点出错，请检查流程的配置，在OA中发起流程进行测试
        ///-8：流程节点自动赋值操作错误*/
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetStatusMsg(string code)
        {

            string Msg = "创建成功！";
            switch (code)
            {
                case "-1":
                    Msg = "创建流程失败!";
                    break;
                case "-2":
                    Msg = "没有创建权限!";
                    break;
                case "-3":
                    Msg = "创建流程失败!";
                    break;
                case "-4":
                    Msg = "字段或表名不正确!";
                    break;
                case "-5":
                    Msg = "更新流程级别失败!";
                    break;
                case "-6":
                    Msg = "无法创建流程待办任务!";
                    break;
                case "-7":
                    Msg = "流程下一节点出错，请检查流程的配置，在OA中发起流程进行测试!";
                    break;
                case "-8":
                    Msg = "流程节点自动赋值操作错误!";
                    break;
                default:
                    break;
            }
            return Msg;
        }


    }
}
