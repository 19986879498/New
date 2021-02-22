using Dapper.Contrib.Extensions;
using HISDouble.DapperConfig;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 患者信息表
    /// </summary>
    [Table("zjhis.View_PreRegister")]
    public class View_PreRegister
    {
        //病历号
        [Column(Name = "MEDICALRECORDNO")]
        public virtual string MedicalRecordNo { get; set; }
        //医疗证号
        [Column(Name = "MEDICALCARDNUMBER")]
        public virtual string MedicalCardNumber { get; set; }
        //合同单位
        [Column(Name = "CONTRACTUNIT")]
        public virtual string ContractUnit { get; set; }
        //结算类别
        [Column(Name = "CONTRACTCATEGORY")]
        public virtual string ContractCategory { get; set; }
        //姓名
        [Column(Name = "CNAME")]
        public virtual string Cname { get; set; }
        //性别
        [Column(Name = "GENDER")]
        public virtual string Gender { get; set; }
        //出生日期
        [Column(Name = "CBIRTHDAY")]
        public virtual DateTime Cbirthday { get; set; }
        //诊别
        [Column(Name = "ZHENBIE")]
        public virtual string Zhenbie { get; set; }
        //年龄
        [Column(Name = "CAGE")]
        public virtual string Cage { get; set; }
        //联系电话
        [Column(Name = "TELPHONE")]
        public virtual string Telphone { get; set; }
        //账户余额
        [Column(Name = "ACCOUNTBALANCE")]
        public virtual string AccountBalance { get; set; }
        //联系地址
        [Column(Name = "CADDRESS")]
        public virtual string Caddress { get; set; }
        //身份证号
        [Column(Name = "IDCARD")]
        public virtual string IDCard { get; set; }
        //ID
        [Column(Name = "ID")]
        public virtual string ID { get; set; }
    }
}