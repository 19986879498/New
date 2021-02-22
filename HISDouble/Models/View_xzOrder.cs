using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 患者医嘱查询
    /// </summary>
    [Table("PT_View_Order")]
    public class View_xzOrder
    {
        [Key]
        public virtual string Id { get; set; }
        public virtual string CardNo { get; set; }
        public virtual string InpatientNo { get; set; }
        public virtual string Dtype { get; set; }
        public virtual string Dcontent { get; set; }
        public virtual string Dspecifications { get; set; }
        public virtual string Dnumber { get; set; }
        public virtual string Dusage { get; set; }
        public virtual string Dfrequency { get; set; }
    }
   
}