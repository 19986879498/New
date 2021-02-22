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
    [Table("zjhis.turn_Annex")]
    public class Annex
    {
        public virtual int Id { get; set; }
        public virtual int Rid { get; set; }
        public virtual string CardNo { get; set; }
        public virtual string InpatientNo { get; set; }
        public virtual string Name { get; set; }
        public virtual string Type { get; set; }
        public virtual string Actualname { get; set; }
        public virtual string Url { get; set; }
        public virtual string Author { get; set; }
    }
   
}