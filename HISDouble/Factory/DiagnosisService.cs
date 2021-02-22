using HISDouble.Interfaces;
using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Z.Dapper.Plus;

namespace HISDouble.Factory
{
   public class DiagnosisService: BaseServices, IDiagnosisService
    {
        public DiagnosisService(IDapperContext dapper,IDiagnosisdicService diagnosisdicService,IView_DiagnosisService view_DiagnosisService)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
            this._diagnosisdicService = diagnosisdicService;
            this._view_DiagnosisService = view_DiagnosisService;
        }

        public IDapperContext dapper { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        public IDbConnection dbConnection=null;

        public IDiagnosisdicService _diagnosisdicService { get; private set; }

        private readonly IView_DiagnosisService _view_DiagnosisService;

        /// <summary>
        /// 保存诊断信息
        /// </summary>
        /// <param name="referral"></param>
        /// <param name="diagnosis"></param>
        /// <param name="trans"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        public bool SaveDiagnosis(Referral referral, DiagnosisParams diagnosis,IDbTransaction trans, ref string Err)
        {
            bool roll = true;
            if (diagnosis==null)
            {
                Err = "诊断信息不得为null";
                return false;
            }
            if (diagnosis.diagnosis!=null)
            {
                List<Diagnosis> listdiagnosis = diagnosis.diagnosis;
                if (listdiagnosis.Count == 0 || listdiagnosis==null)
                {
                    Err = "诊断信息为空，无法进行保存";
                    return false;
                }
                listdiagnosis.Where(x => x.Dtype != "");
                List<Diagnosis> diagnoses = DiagnosisQueryByRid(diagnosis.id);

          
                //批量删除listdiagnosis  Diagnosis
                //var isDeleteList = this.dbConnection.BulkDelete<Diagnosis>(diagnoses).Current;

                //if (isDeleteList.Count()<0)
                //{
                //    Err = "批量删除Diagnosis表中的数据失败";
                //    trans.Rollback();
                //    return false;
                //}
                //批量删除
                foreach (Diagnosis dg in diagnoses)
                {
                    dg.Rid = diagnosis.id;
                    bool isInsert = dbConnection.Delete<Diagnosis>(dg,trans) ;
                   
                   
                }
                //批量新增
                foreach (Diagnosis dg in listdiagnosis)
                {
                    dg.Rid = diagnosis.id;
                   bool isInsert= Function.Insert<Diagnosis>(dg, dbConnection, ref trans) > 0;
                    if (!isInsert)
                    {
                        Err = "批量新增Diagnosis表中的数据失败";
                        trans.Rollback();
                        return false;
                    }
                }
                return roll;
            }
            Err = "诊断信息不为空";
            return false;
        }
        /// <summary>
        /// 根据Rid查询诊断信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<Diagnosis> DiagnosisQueryByRid(int id)
        {
            this.dbConnection = this.dapper.GetDbConnection(conn);
            List<Diagnosis> diagList = dbConnection.ViewGetAll<Diagnosis>().Where(l => l.Rid == id).ToList();
            return diagList;
        }
        /// <summary>
        /// 返回参数
        /// </summary>
        /// <param name="hoscode"></param>
        /// <param name="depcode"></param>
        /// <returns></returns>
        public JsonResult DiagnosisQuery(string hoscode, string depcode)
        {
            List<View_Diagnosis> view_Diagnoses = _view_DiagnosisService.DiagnosisQuery(hoscode, depcode);
            if (view_Diagnoses==null)
            {
                return Function.GetErrResult("查询无数据！");
            }
            return Function.GetResultList<View_Diagnosis>(view_Diagnoses);
        }
    }
}
