using HISDouble.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
    public class BaseServices:IBaseService
    {
        /// <summary>
        /// 数据库配置对象
        /// </summary>
        public IConfigurationRoot connRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        public string ErrorMsg = string.Empty;
        public int ErrCode = 1;
    }
}
