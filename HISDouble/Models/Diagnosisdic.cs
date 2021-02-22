using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 上转诊断字典表
    /// </summary>
    [Table("zjhis.TURN_DIAGNOSISDIC")]
    public class Diagnosisdic
    {
        public virtual int Id { get; set; }
        public virtual string DiagnosisID { get; set; }
        public virtual string Diagnosis { get; set; }
    }
   
}