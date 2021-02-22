using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 登录用户
    /// </summary>
    [Table("zjhis.turn_USERINFO")]
    public class Userinfo
    {
        [Key]
        public virtual int Id { get; set; }
        public virtual string Hospitalcode { get; set; }
        public virtual string Hospitalname { get; set; }
        public virtual string Hospitallevel { get; set; }
        public virtual string Departmentscode { get; set; }
        public virtual string Departments { get; set; }
        public virtual string Hospitaladdress { get; set; }
        public virtual string Username { get; set; }
        public virtual string Name { get; set; }
        public virtual string Useremail { get; set; }
        public virtual string Userphone { get; set; }
        public virtual string Userrole { get; set; }
        public virtual string Userpwd { get; set; }
        public virtual int IsAvailable { get; set; }
    }
}
