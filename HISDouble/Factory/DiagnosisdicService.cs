using Dapper;
using HISDouble.Interfaces;
using HISDouble.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
   public class DiagnosisdicService: BaseServices, IDiagnosisdicService
    {
        public DiagnosisdicService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbConnection= dapper.GetDbConnection(conn);
        }

        public IDapperContext dapper { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        /// <summary>
        /// 获取dapper对象
        /// </summary>
        private IDbConnection dbConnection = null;
        /// <summary>
        /// 获取转出诊断信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<Diagnosisdic> SelectAll(string code,string name)
        {
            //List<Diagnosisdic> diagnosisdics = this.dbConnection.GetAll<Diagnosisdic>().Where(l => l.DiagnosisID == code).ToList();
            List<Diagnosisdic> diagnosisdics = this.dbConnection.Query<Diagnosisdic>($"select *from zjhis.turn_diagnosisdic d where d.diagnosisid='{code}' and d.diagnosis='{name}'").ToList();
            return diagnosisdics;
        }

        
    }
}
