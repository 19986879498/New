using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 医院信息表
    /// </summary>
    [Table("zjhis.turn_hospitaldic")]
    public class Hospitaldic
    {
        public virtual int Id { get; set; }
        public virtual string Hoscode { get; set; }
        public virtual string Hosname { get; set; }
    }
   
}