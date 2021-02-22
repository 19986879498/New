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
    /// 诊断表
    /// </summary>
    [Table("zjhis.turn_Diagnosis")]
    public class Diagnosis
    {

        [Column(Name ="ID")]
        public virtual string Id { get; set; }
        [Key]
        [Column(Name = "RID")]
        public virtual int Rid { get; set; }

        // [Column(Name = "HOS_CODE")]
        public  string Hos_code = "";
        //[Column(Name = "HOS_NAME")]
        public string Hos_name = "";
        [Column(Name = "CARDNO")]
        public virtual string CardNo { get; set; }
        [Column(Name = "INPATIENTNO")]
        public virtual string InpatientNo { get; set; }
        [Column(Name = "DTYPE")]
        public virtual string Dtype { get; set; }
        [Column(Name = "DMAIN")]
        public virtual string Dmain { get; set; }
        [Column(Name = "DICD10")]
        public virtual string Dicd10 { get; set; }
        [Column(Name = "DNAME")]
        public virtual string Dname { get; set; }
        [Column(Name = "ISOUT")]
        public virtual int Isout { get; set; }
    }
   
}