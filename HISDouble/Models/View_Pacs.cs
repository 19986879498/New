using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    [Table("pacs31.v_devicetable")]
    public class View_Pacs
    {
        //设备ID
        [Key]
        public virtual string DEVICEID { get; set; }
        //设备类型ID
        public virtual string DEVICETYPEID { get; set; }
        //设备类型名称
        public virtual string DEVICETYPENAME { get; set; }
        //
        public virtual string MODALITY { get; set; }
        //设备ID编码
        public virtual string DEVICECODE { get; set; }
        //科室名称
        public virtual string DEVICENAME { get; set; }
        //科室ID
        public virtual string DEPARTMENTID { get; set; }
        //科室名称
        public virtual string DEPARTMENTNAME { get; set; }
        //是否有效
        public virtual string ISAVAILABLE { get; set; }
        //
        public virtual string OPERATORID { get; set; }
        //
        public virtual string OPERATETIME { get; set; }
        //
        public virtual string REMARKINFO { get; set; }
        //
        public virtual string ALLACOUNT { get; set; }
        //
        public virtual string CANUSEYEAR { get; set; }
        //
        public virtual string MAINTAINPERYEAR { get; set; }
        //
        public virtual string ADDINTIME { get; set; }
        //
        public virtual string ROOMNAME { get; set; }
        //
        public virtual string SIMPLECODE { get; set; }
        //
        public virtual string CLEARDATE { get; set; }
    }
}
