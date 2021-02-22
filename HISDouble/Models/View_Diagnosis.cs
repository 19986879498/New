using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 诊断信息查询
    /// </summary>
    [Table("zjhis.View_Diagnosis")]
    public class View_Diagnosis
    {
        public virtual string Id { get; set; }
        public virtual string CardNo { get; set; }
        public virtual string InpatientNo { get; set; }
        public virtual string Dtype { get; set; }
        public virtual string Dmain { get; set; }
        public virtual string Dicd10 { get; set; }
        public virtual string Dname { get; set; }
    }
   
}