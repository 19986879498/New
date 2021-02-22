using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 床位信息
    /// </summary>
    [Table("zjhis.FIN_IPR_INMAININFO")]
    public class View_BedsInfo
    {
        public virtual string INPATIENT_NO { get; set; }//住院流水号
        public virtual string Patient_No { get; set; }    //住院号
        public virtual string Name { get; set; }         //患者姓名
        public virtual string Sex_Code { get; set; }      //患者性别
        public virtual string Tend { get; set; }         //护理级别
        public virtual string Dietetic_Mark { get; set; } //饮食
        public virtual string Ext_Flag2 { get; set; }     //年龄
        public virtual string Dept_Code { get; set; }     //机构代码
        public virtual string House_Doc_Code { get; set; } //医师代码
        public virtual string Bed_No { get; set; }        //床位号
        public virtual string Clinic_Diagnose { get; set; }//诊断   Diag_Name   
        public virtual string In_state { get; set; }
        public virtual string IN_TIMES { get; set; }//住院次数
        public virtual string BIRTHDAY { get; set; }//生日
        public virtual string In_Date { get; set; }//住院日期
        public virtual string Out_Date { get; set; }//出院日期
    }
}