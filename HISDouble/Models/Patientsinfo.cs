using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    [Table("zjhis.turn_Patientsinfo")]
    public class Patientsinfo
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string CardNo { get; set; }
        public virtual string InpatientNo { get; set; }
        public virtual string Name { get; set; }
        public virtual string Birthday { get; set; }
        public virtual string SexCode { get; set; }
        public virtual string Idenno { get; set; }
        public virtual string Home { get; set; }
        public virtual string HomeTel { get; set; }
        public virtual string Mtype { get; set; }
        public virtual string Mcard { get; set; }
    }
}
