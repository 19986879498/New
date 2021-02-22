using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    [Table("zjhis.PreInspection")]
    public class PreInspection
    {  //public virtual string ID { get; set; }
        [Key]
        public virtual string ID { get; set; } = "";
        public virtual string CARDNO { get; set; } = "";
        public virtual string PAME { get; set; } = "";
        public virtual string PGENDER { get; set; } = "";
        public virtual DateTime PBIRTHDAY { get; set; } = DateTime.Parse("0001-01-01 00:00:00");
        public virtual string INPATIENTNO { get; set; } = "";
        public virtual string PIDCARE { get; set; }="";
        public virtual string PPHONE { get; set; }="";
        public virtual string PADDRESS { get; set; }="";
        public virtual string HospitalName { get; set; } = "";
        public virtual string DeptCode { get; set; } = "";
        public virtual string DeptName { get; set; } = "";
        public virtual string Equipmentname { get; set; } = "";
        public virtual string Equipmentcode { get; set; } = "";
        public virtual DateTime OrderDate { get; set; } = DateTime.Parse("0001-01-01 00:00:00");
    }
}
