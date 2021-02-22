using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble.Models
{

    public class CheckInDataList
    {
        private List<View_GetCheckindata> checkData = new List<View_GetCheckindata>();
        /// <summary>
        /// 打卡的数据
        /// </summary>
        public List<View_GetCheckindata> CheckData
        {
            get { return checkData; }
            set { checkData = value; }
        }
        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 结束时间
        /// </summary>
        public string EndTime { get; set; }
        /// <summary>
        /// 部门id
        /// </summary>
        public string DEPT_ID { get; set; }
    }
}
