using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 医保类型
    /// </summary>
    [Table("zjhis.turn_medicaldic")]
    public class Medicaldic
    {
        public virtual int Id { get; set; }
        public virtual int MedCode { get; set; }
        public virtual string MedName { get; set; }
    }
}