using Dapper.Contrib.Extensions;
using HISDouble.DapperConfig;
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
    [Table("hrmdepartment")]
    public class View_hrmdepartment
    {
        public virtual string ID { get; set; }
        public virtual string DEPARTMENTMARK { get; set; }
        public virtual string DEPARTMENTNAME { get; set; }
        public virtual int SUBCOMPANYID1 { get; set; }
        public virtual int SUPDEPID { get; set; }
        public virtual string ALLSUPDEPID { get; set; }
        public virtual string CANCELED { get; set; }
        public virtual string DEPARTMENTCODE { get; set; }
        public virtual int COADJUTANT { get; set; }
        public virtual string ZZJGBMFZR { get; set; }
        public virtual string ZZJGBMFGLD { get; set; }
        public virtual string JZGLBMFZR { get; set; }
        public virtual string JZGLBMFGLD { get; set; }
        public virtual string BMFZR { get; set; }
        public virtual string BMFGLD { get; set; }
        public virtual string OUTKEY { get; set; }
        public virtual int BUDGETATUOMOVEORDER { get; set; }
        public virtual string ECOLOGY_PINYIN_SEARCH { get; set; }
        public virtual int TLEVEL { get; set; }
        public virtual string CREATED { get; set; }
        public virtual int CREATER { get; set; }
        public virtual string MODIFIED { get; set; }
        public virtual int MODIFIER { get; set; }
        public virtual string UUID { get; set; }
        public virtual string SHOWORDER { get; set; }
        public virtual int SHOWORDEROFTREE { get; set; }
    }
}