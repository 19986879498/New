using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 病人信息表
    /// </summary>
    [Dapper.Contrib.Extensions.Table("zjhis.View_PatientsInfo")]
    public class View_PatientsInfo
    {
        [Dapper.Contrib.Extensions.Key]
        [Column("CARDNO")]
        public string CardNo { get; set; }
        [Column("INPATIENTNO")]
        public string InpatientNo { get; set; }
        public string Pame { get; set; }
        public string DepartmentCode { get; set; }
        public string Pgender { get; set; }
        public DateTime Pbirthday { get; set; }
        public string Paddress { get; set; }
        public string Pphone { get; set; }
        public string Pidcare { get; set; }
        public string DepartmentName { get; set; }
        public string Hos_code { get; set; }
        public string Hos_name { get; set; }
        public string IN_DATE { get; set; }
        public string Pack_code { get; set; }
        public string Pack_name { get; set; }
    }

}