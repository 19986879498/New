using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableAttribute = Dapper.Contrib.Extensions.TableAttribute;

namespace HISDouble.Models
{
    /// <summary>
    /// 在住院患者表
    /// </summary>
   [Table("zjhis.View_Beds")]
    public class View_Beds
    {
        public virtual string Id { get; set; }
        public virtual string Hos_code { get; set; }
        public virtual string Hos_name { get; set; }
        public virtual string DepartmentsCode { get; set; }
        public virtual string DepartmentsName { get; set; }
        public virtual string Beds { get; set; }
    }
   
}