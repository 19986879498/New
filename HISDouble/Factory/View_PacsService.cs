using HISDouble.Interfaces;
using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace HISDouble.Factory
{
    public class View_PacsService:BaseServices, IView_PacsService
    {
        public View_PacsService(IDapperContext dapper)
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
        /// <summary>
        /// 查询pacs检查（返回json结果）
        /// </summary>
        /// <param name="keshiCode"></param>
        /// <returns></returns>
        public JsonResult GetDepartmentQuery(DiviceParams keshiCode)
        {
            List<View_Pacs> view_Pacs = new List<View_Pacs>();
            try
            {
               view_Pacs = this.GetDepartment(keshiCode);

            }
            catch (Exception ex)
            {
                return Function.GetErrResult(ex.Message);
            }
            return Function.GetResultList<View_Pacs>(view_Pacs);
        }
        /// <summary>
        /// 查询pacs检查
        /// </summary>
        /// <param name="keshiCode"></param>
        /// <returns></returns>
        public List<View_Pacs> GetDepartment(DiviceParams keshiCode)
        {
            List<View_Pacs> entities = new List<View_Pacs>();
            if (keshiCode == null || keshiCode.DeptCode == "" || keshiCode.DeptCode == null)
            {
                entities = this.dbConnection.GetAll<View_Pacs>().Where(i=>i.DEPARTMENTID != "4").ToList();
            }
            else
            {
                entities = this.dbConnection.GetAll<View_Pacs>().Where(x => x.DEPARTMENTID == keshiCode.DeptCode).ToList();

            }
            return entities;
        }
        /// <summary>
        /// 保存pacs检查（返回json结果）
        /// </summary>
        /// <param name="PreInspection">pacs检查报告对象</param>
        /// <returns></returns>
        public JsonResult SaveCheckData(PreInspection PreInspection)
        {
            bool isSave = this.SaveCheckData(PreInspection, ref this.ErrorMsg);
            if (isSave)
            {
                return Function.GetSuccessResult(ErrorMsg);
            }
            else
            {
                return Function.GetSuccessResult(ErrorMsg);
            }
        }
        /// <summary>
        /// 保存pacs检查
        /// </summary>
        /// <param name="PreInspection">pacs检查报告对象</param>
        /// <param name="Err">错误信息</param>
        /// <returns></returns>
        public bool SaveCheckData(PreInspection preInspection, ref string Err)
        {
            DBFunction.OpenConnService(dbConnection);
            IDbTransaction trans = dbConnection.BeginTransaction();
            string Key = "ID";
            string KeyVal = "Seq_PreInspection.nextval";
            int res=  DBFunction.Insert<PreInspection>(preInspection, dbConnection, ref trans, ref Err,Key,KeyVal);
            if (res<0)
            {
                trans.Rollback();
                return false;

            }
            trans.Commit();
            return true;
        }
    }
}
