using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Models.Base
{
    public class RequestQueryParam
    { /// <summary>
      /// 页数
      /// </summary>
        [DefaultValue(1)]
        public int pageNum { get; set; }
        /// <summary>
        /// pageSize
        /// </summary>
        [DefaultValue(10)]
        public int pageSize { get; set; }
        /// <summary>
        /// 医院
        /// </summary>
        [DefaultValue("")]
        public string hospital { get; set; }

        /// <summary>
        /// 上转下转标记  1上  2下
        /// </summary>
        [DefaultValue(1)]
        public int sign { get; set; }
        /// <summary>
        ///搜索框输入内容
        /// </summary>
         public string inputInfo { get; set; }
    }
}
