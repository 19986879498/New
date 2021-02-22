using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 患者信息表
    /// </summary>
    [Table("zjhis.View_contractunit")]
    public class View_contractunit
    {
        //类型
        public virtual string type { get; set; }
        //Code
        public virtual string code { get; set; }
        //名称
        public virtual string name { get; set; }
        //编码
        public virtual string spell_code { get; set; }
        //ID
        [Key]
        public virtual string id { get; set; }
    }
}