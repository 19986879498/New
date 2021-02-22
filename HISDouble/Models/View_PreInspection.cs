using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 患者信息表
    /// </summary>
    [Table("pt_view_patientsinfo")]
    public class View_PreInspection
    {
        //public virtual string ID { get; set; }
        [Key]
        public virtual string JGID { get; set; }

        public virtual string ORGANIZNAME { get; set; }

        public virtual string CARDNO { get; set; }

        public virtual string INPATIENTNO { get; set; }

        public virtual string PAME { get; set; }

        public virtual string PGENDER { get; set; }

        public virtual string PBIRTHDAY { get; set; }

        public virtual string PADDRESS { get; set; }

        public virtual string PPHONE { get; set; }
        //ID
        public virtual string PIDCARE { get; set; }

        public virtual string DEPARTMENTCODE { get; set; }

        public virtual string DEPARTMENTNAME { get; set; }

        public virtual string HOS_CODE { get; set; }
        
        public virtual string HOS_NAME { get; set; }

        public virtual string PACK_CODE { get; set; }

        public virtual string PACK_NAME { get; set; }

        public virtual string IN_DATE { get; set; }




    }
}
