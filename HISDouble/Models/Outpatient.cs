using Dapper.Contrib.Extensions;

namespace HISDouble.Models
{
    /// <summary>
    /// 门诊助手
    /// </summary>
    [Table("zjhis.fin_opr_register")]
    public class Outpatient
    {
        public virtual string CLINIC_CODE { get; set; }   //门诊号/发票号   唯一
        public virtual string DOCT_NAME { get; set; }    //医师姓名
        public virtual string DOCT_CODE { get; set; }    //医师编码
        public virtual string NAME { get; set; }         //患者姓名
        public virtual string SEX_CODE { get; set; }     //患者性别
        public virtual string BIRTHDAY { get; set; }     //生日
        public virtual string CARD_NO { get; set; }      //卡号
        public virtual string VALID_FLAG { get; set; }   //是否有效
        public virtual string REG_DATE { get; set; }     //挂号日期
        public virtual string REGLEVL_CODE { get; set; } //挂号级别编码
        public virtual string REGLEVL_NAME { get; set; } //挂号级别名称
        public virtual string DEPT_CODE { get; set; }    //科室编号 
        public virtual string DEPT_NAME { get; set; }    //科室名称
        public virtual string YNSEE { get; set; }//是否看诊
        public virtual object DIAG_NAME { get; set; }   //诊断名称
    }
}