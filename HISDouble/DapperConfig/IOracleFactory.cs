using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.DapperConfig
{
  public  interface IOracleFactory
    {
        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="Name">参数名称</param>
        /// <param name="oracleDbType">参数数据类型</param>
        /// <param name="parameterDirection">参数类型</param>
        /// <param name="obj">值</param>
        /// <param name="size">大小</param>
        void Add(string Name, DbType oracleDbType, ParameterDirection parameterDirection, object? obj = null, int? size = null);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="PreName">存储过程名称</param>
        /// <param name="OutName">返回参数名称</param>
        /// <returns></returns>
        T Get<T>(string PreName, string OutName);

    }
}
