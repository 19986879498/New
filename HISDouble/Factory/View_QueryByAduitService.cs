using Dapper;
using HISDouble.Interfaces;
using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
   public class View_QueryByAduitService : BaseServices, IView_QueryByAuditService
    {
        public View_QueryByAduitService(IDapperContext dapper, IPatientsInfoService patientsInfoService)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
            this._patientsInfoService = patientsInfoService;
        }

        public IDapperContext dapper { get; set; }

        private IDbConnection dbConnection;
        /// <summary>
        /// 患者基本信息服务
        /// </summary>
        private IPatientsInfoService _patientsInfoService { get; set; }
        /// <summary>
        /// 患者基本信息实体类
        /// </summary>
        private Patientsinfo patientsinfo = new Patientsinfo();
        /// <summary>
        /// 数据库配置对象
        /// </summary>
        private IConfigurationRoot connRoot = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        public List<Referral> QueryByAudits(RequestQueryParam param)
        {
            this.dbConnection = this.dapper.GetDbConnection(connRoot.GetConnectionString("ZJhisConnStr").ToString());
            List<Referral> list = null;
            if (param.inputInfo.Length > 0)
            {
                list= dbConnection.Query<Referral>($"SELECT * FROM zjhis.turn_REFERRAL where Hospitalout='{param.hospital}' and Sign in (0,1,2) and Patient like '%{param.inputInfo}%'").ToList();
            }
            else
            {
                list= dbConnection.Query<Referral>($"SELECT * FROM zjhis.turn_REFERRAL where Hospitalout='{param.hospital}' and Sign in (0,1,2)").ToList();
            }
            foreach (var item in list)
            {
                item.Patientsinfo = _patientsInfoService.SelectByCardNo(item.CardNo, item.InpatientNo, ref this.patientsinfo);
            }
            List<Referral> list1 = list.Skip((param.pageNum - 1) * param.pageSize).Take(param.pageSize).ToList();

            return list1;
        }
        public JsonResult QueryByAudit(RequestQueryParam Param)
        {
            List<Referral> ReferralList = this.QueryByAudits(Param);
            if (ReferralList == null || ReferralList.Count == 0)
            {
                return Function.GetErrResult("查询审核数据为空！");
            }
            return Function.GetResultList<Referral>(ReferralList);
        }
    }
}
