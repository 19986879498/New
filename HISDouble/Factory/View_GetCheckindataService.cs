using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using HISDouble.Interfaces;
using Microsoft.Extensions.Configuration;

namespace HISDouble.Factory
{
   public class View_GetCheckindataService: BaseServices, IView_GetCheckindataService
    {
        public View_GetCheckindataService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
        }

        public IDapperContext dapper { get; set; }

        private IDbConnection dbConnection;

        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
    }
}
