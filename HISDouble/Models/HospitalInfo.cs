using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    [Table("dic_medicalinstitutionsinfo")]
    public class HospitalInfo
    {
        [Key]
        public string HID { get; set; }
        public string ZBID { get; set; }
        public string JGMC { get; set; }
        public string ORG_CODE { get; set; }
        public string SORTID { get; set; }
    }
}
