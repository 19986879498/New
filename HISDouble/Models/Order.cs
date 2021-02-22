using Dapper.Contrib.Extensions;
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
   [Table("zjhis.turn_Order")]
    public class Order 
    {
        //public virtual int Id { get; set; }
        //public virtual int Rid { get; set; }
        //public virtual string CardNo { get; set; }
        //public virtual string InpatientNo { get; set; }
        //public virtual string Type { get; set; }
        //public virtual string Content { get; set; }
        //public virtual int Isout { get; set; }

        public virtual string Id { get; set; }
        public virtual int Rid { get; set; }
        public virtual string CardNo { get; set; }
        public virtual string InpatientNo { get; set; }
        public virtual string Dtype { get; set; }
        public virtual string Dcontent { get; set; }
        public virtual string Dspecifications { get; set; }
        public virtual string Dnumber { get; set; }
        public virtual string Dusage { get; set; }
        public virtual string Dfrequency { get; set; }
        public virtual int Isout { get; set; }
    }
   
}