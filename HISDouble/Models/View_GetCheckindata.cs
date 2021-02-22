using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{
    /// <summary>
    /// 打卡信息表
    /// </summary>
    [Table("zjhis.CLOCKINGDATA")]
    public class View_GetCheckindata
    {
        //用户id
        public virtual string USERID { get; set; }
        //打卡规则名称
        public virtual string GROUPNAME { get; set; }
        //打卡类型。字符串，目前有：上班打卡，下班打卡，外出打卡
        public virtual string CHECKIN_TYPE { get; set; }
        //	异常类型，字符串，包括：时间异常，地点异常，未打卡，wifi异常，非常用设备。如果有多个异常，以分号间隔
        public virtual string EXCEPTION_TYPE { get; set; }
        //打卡时间。Unix时间戳
        public virtual string CHECKIN_TIME { get; set; }
        //打卡地点title
        public virtual string LOCATION_TITLE { get; set; }
        //打卡地点详情
        public virtual string LOCATION_DETAIL { get; set; }
        //打卡wifi名称
        public virtual string WIFINAME { get; set; }
        //打卡备注
        public virtual string NOTES { get; set; }
        //打卡的MAC地址/bssid
        public virtual string WIFIMAC { get; set; }
        //打卡的附件media_id，可使用media/get获取附件
        public virtual string MEDIAIDS { get; set; }
        //位置打卡地点纬度
        public virtual string LAT { get; set; }
        //位置打卡地点经度
        public virtual string LNG { get; set; }
        //打卡设备id
        public virtual string DEVICEID { get; set; }
        /// <summary>
        /// 部门ID
        /// </summary>
        public virtual string DEPT_ID { get; set; }
        //ID
        [Key]
        public virtual string ID { get; set; } = Guid.NewGuid().ToString("N");
    }
   
}