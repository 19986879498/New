using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

namespace HISDouble.Models.Base
{
    /// <summary>
    /// WorkflowRequestInfo\工作流程请求信息类
    /// </summary>
    public class WorkflowRequestInfo
    {
        public string requestId { get; set; }//请求ID
        public string requestName { get; set; }//请求标题
        public string requestLevel { get; set; }//请求重要级别
        public string messageType { get; set; }//短信提醒
        public workflowBaseInfo workflowBaseInfo { get; set; }//流程类型
        public string currentNodeName { get; set; }//当前节点名称
        public string currentNodeId { get; set; }//当前节点ID
        public string status { get; set; }//流程状态
        public string creatorId { get; set; }//创建者
        public string createTime { get; set; }//创建时间
        public string lastOperatorName { get; set; }//最后操作者名称
        public string lastOperateTime { get; set; }//最后操作时间
        public bool canView { get; set; }//是否可查看
        public bool canEdit { get; set; }//是否可编辑
        public bool mustInputRemark { get; set; }//签字意见是否必填
        public workflowMainTableInfo workflowMainTableInfo { get; set; }//主表信息
        public WorkflowDetailTableInfo workflowDetailTableInfos { get; set; }//明细表信息
        public WorkflowRequestLog workflowRequestLogs { get; set; }//流转日志信息
        public string[] WorkflowHtmlTemplete { get; set; }//HTML显示模板(0,ipad/1,iphone)
        public string[] WorkflowHtmlShow { get; set; }//解析后的HTML显示内容(0, ipad/1, iphone)
        public string beagentid { get; set; }//被代理人
        public string[] workflowPhrases { get; set; }//流程短语
    }
    /// <summary>
    /// WorkflowBaseInfo\工作流信息类
    /// </summary>
    public class workflowBaseInfo
    {
        public string workflowId { get; set; }//工作流ID
        public string workflowName { get; set; }//工作流标题
        public string workflowTypeId { get; set; }//工作流类型ID
        public string workflowTypeName { get; set; }//工作流类型名称
    }
    /// <summary>
    ///WorkflowRequestTableRecord\表记录类
    /// </summary>
    public class WorkflowRequestTableRecord {
        public workflowRequestTableField workflowRequestTableFields { get; set; }//记录字段
        public int recordOrder { get; set; }//记录数
    }
    /// <summary>
    ///WorkflowMainTableInfo\主表类
    /// </summary>
    public class workflowMainTableInfo {
        public WorkflowRequestTableRecord requestRecords { get; set; }////记录
        public string tableDBName { get; set; }//数据库名
    }
    /// <summary>
    ///WorkflowDetailTableInfo\明细表类
    /// </summary>
    public class WorkflowDetailTableInfo
    {
        public string tableDBName { get; set; }//数据库名
        public string[] tableFieldName { get; set; }//字段名
        public string tableTitle { get; set; }//标题
        public WorkflowRequestTableRecord workflowRequestTableRecords { get; set; }//记录
    }
    /// <summary>
    ///WorkflowRequestLog\流转日志类
    /// </summary>
    public class WorkflowRequestLog {
        public string agentor { get; set; }//代理人
        public string agentorDept { get; set; }//代理人部门
        public string annexDocHtmls { get; set; }
        public string id { get; set; }//代理人ID
        public string nodeId { get; set; }//节点
        public string nodeName { get; set; }//节点名称
        public string operateDate { get; set; }//操作日期
        public string operateTime { get; set; }//操作时间
        public string operateType { get; set; }//操作类型
        public string operatorDept { get; set; }//操作部门
        public string operatorId { get; set; }//操作者ID
        public string operatorName { get; set; }//操作者名称
        public string operatorSign { get; set; }//操作者签字意见
        public string receivedPersons { get; set; }//接收人
        public string remark { get; set; }//备注
        public string remarkSign { get; set; }
        public string signDocHtmls { get; set; }
        public string signWorkFlowHtmls { get; set; }
    }
    /// <summary>
    /// WorkflowRequestTableField\表记录字段类
    /// </summary>
    public class workflowRequestTableField {
        public string browserurl { get; set; }//URL
        public bool edit { get; set; }//是否可编辑
        public string fieldDBType { get; set; }//字段数据库类型
        public string fieldFormName { get; set; }//表单名称
        public string fieldHtmlType { get; set; }//HTML类型
        public string fieldId { get; set; }//字段id
        public string fieldName { get; set; }//字段名称
        public int fieldOrder { get; set; }//字段数
        public string fieldShowName { get; set; }//字段显示名称
        public string fieldShowValue { get; set; }//字段显示值
        public string fieldType { get; set; }//字段类型
        public string fieldValue { get; set; }//字段值
        public string filedHtmlShow { get; set; }//HTML显示
        public bool mand { get; set; }
        public string[] selectnames { get; set; }//选择框名称
        public string[] selectvalues { get; set; }//选择框值
        public bool view { get; set; }//是否课件
    }
}
