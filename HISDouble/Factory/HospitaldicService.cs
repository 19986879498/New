using Dapper.Contrib.Extensions;
using HISDouble.Interfaces;
using HISDouble.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
   public class HospitaldicService: BaseServices, IHospitaldicService
    {
        public HospitaldicService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
        }

        public IDapperContext dapper { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        public IDbConnection dbConnection = null;
        /// <summary>
        /// 根据code查询医院信息
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public Hospitaldic HosQueryByCode(string code)
        {
            dbConnection = dapper.GetDbConnection(conn);
            return dbConnection.GetAll<Hospitaldic>().FirstOrDefault(f => f.Hoscode.Equals(code));
        }
        /// <summary>
        /// 查询医院字典表
        /// </summary>
        /// <returns></returns>
        public List<Hospitaldic> HosQuery()
        {
          return  dbConnection.GetAll<Hospitaldic>().ToList();
        }

        public JsonResult HosResult()
        {
            List<Hospitaldic> hosList = HosQuery();
            if (hosList==null)
            {
                return Function.GetErrResult("查询无数据");

            }
            return Function.GetResultList<Hospitaldic>(hosList);
        }
        /// <summary>
        /// 医疗机构查询
        /// </summary>
        /// <returns></returns>
        public List<HospitalInfo> GetHospitalInfos()
        {
            this.dbConnection = this.dapper.GetDbConnection(this.connRoot.GetConnectionString("OraclezjhisYB").ToString());
            List<HospitalInfo> hoslist = this.dbConnection.GetAll<HospitalInfo>().OrderBy(x => int.Parse(x.SORTID)).ToList();
            return hoslist;
        }
        /// <summary>
        /// 获取医院Json数据
        /// </summary>
        /// <returns></returns>
        public JsonResult GetHosResult()
        {
            List<HospitalInfo> hoslist = this.GetHospitalInfos();
            if (hoslist==null)
            {
                return Function.GetErrResult("获取医院列表失败！");
            }
            JsonResult js = Function.GetResultList<HospitalInfo>(hoslist);
            return js;
        }
    }
}
