using Dapper;
using Dapper.Contrib.Extensions;
using HISDouble.Interfaces;
using HISDouble.Models;
using Microsoft.Extensions.Configuration;
using Oracle.EntityFrameworkCore.Storage.Internal;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
   public class AnnexService:BaseServices,IAnnexService
    {
        public AnnexService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbconn = dapper.GetDbConnection(conn);
        }

        public IDbConnection dbconn = null;

        public IDapperContext dapper { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IEnumerable<Annex> QueryAnnex(int id)
        {
            this.dbconn = this.dapper.GetDbConnection(conn);
            return dbconn.GetAll<Annex>().Where(u => u.Rid == id);
        }
    }
}
