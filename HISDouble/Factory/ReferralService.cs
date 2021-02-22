using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Dapper.Contrib.Extensions;
using HISDouble.Interfaces;
using HISDouble.Models;
using HISDouble.Models.Base;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace HISDouble.Factory
{
    public class ReferralService : BaseServices, IReferralService
    {
        private readonly IDapperContext _dbContext;
        public ReferralService(IDapperContext dapper, IHospitaldicService hosService, IView_BedsService bedsService, IPatientsInfoService patientsInfoService, IDiagnosisService diagnosisService, IDapperContext dbContext)
        {
            this.dapper = dapper;
            this._hosService = hosService;
            this._bedsService = bedsService;
            this._patientsInfoService = patientsInfoService;
            this.dbConnection = dapper.GetDbConnection(conn);
            this._diagnosisService = diagnosisService;
            this._dbContext = dbContext;
        }
        private int ErrorCode = 0;
        /// <summary>
        /// dapper服务
        /// </summary>
        public IDapperContext dapper { get; set; }
        /// <summary>
        /// 医院信息服务
        /// </summary>
        public IHospitaldicService _hosService { get; }
        /// <summary>
        /// 床位基本信息服务
        /// </summary>
        public IView_BedsService _bedsService { get; private set; }
        /// <summary>
        /// 患者基本信息服务
        /// </summary>
        private IPatientsInfoService _patientsInfoService { get; set; }
        /// <summary>
        /// 患者基本信息实体类
        /// </summary>
        private Patientsinfo patientsinfo = new Patientsinfo();
        /// <summary>
        /// 数据库字符串
        /// </summary>
        public string SqlStr { get; set; }
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        /// <summary>
        /// 数据库服务
        /// </summary>
        public IDbConnection dbConnection = null;
        /// <summary>
        /// 诊断信息服务
        /// </summary>
        public IDiagnosisService _diagnosisService { get; private set; }

        public int TranSaveReferral(Params @params, ref string Err)
        {
            dbConnection = DBBind();
            Referral referral = @params.referral;

            referral.Timeout = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            if (@params.referral.Sign == 1)
            {
                referral.Status = "1";
            }
            else
            {
                referral.Status = "6";
            }
            referral/*.Patientsinfo*/.CardNo = referral.CardNo;
            referral/*.Patientsinfo*/.InpatientNo = referral.InpatientNo;
            referral.Patientsinfo.CardNo = referral.CardNo;
            referral.Patientsinfo.InpatientNo = referral.InpatientNo;
            referral.Hospitalinto = string.IsNullOrEmpty(referral.Hospitalcodeinto) ? "" : _hosService.HosQueryByCode(referral.Hospitalcodeinto) == null ? "" : _hosService.HosQueryByCode(referral.Hospitalcodeinto).Hosname;
            referral.Departmentsinto = this._dbContext.GetSqlOneResult($"select h.DEPARTMENTNAME from hrmdepartment h where h.ID='{referral.Departmentscodeinto}'", ref this.ErrorCode);
            //referral.Departmentsinto = string.IsNullOrEmpty(referral.Hospitalcodeinto) && string.IsNullOrEmpty(referral.Departmentscodeinto) ? "" : _bedsService.BedsQueryByCode(referral.Hospitalcodeinto, referral.Departmentscodeinto) == null ? "" : _bedsService.BedsQueryByCode(referral.Hospitalcodeinto, referral.Departmentscodeinto).DepartmentsName;
            ///保存转诊     ;
            /////开启事务
            DBFunction.OpenConnService(dbConnection);
            IDbTransaction trans = dbConnection.BeginTransaction();
            if (DBFunction.Insert<Referral>(referral, dbConnection, ref trans, ref Err, "ID", "zjhis.SEQ_turn_REFERRAL.Nextval") < 0)
            {
                dbConnection.Close();
                return -1;
            }

            //查询患者基本信息
            if (this._patientsInfoService.SelectByCardNo(referral.CardNo, referral.InpatientNo, ref patientsinfo) == null)
            {
                @params.referral.Patientsinfo.Birthday = Convert.ToDateTime(@params.referral.Patientsinfo.Birthday).ToString("yyyy-MM-dd HH:mm:ss");
                //新增患者基本信息
                
                if (DBFunction.Insert<Patientsinfo>(referral.Patientsinfo, dbConnection, ref trans, ref Err, "Id", "zjhis.SEQ_turn_Patientsinfo.Nextval") < 0)
                {
                    Err = "新增患者基本信息失败";

                    return -1;
                }
            }
            else
            {
                //修改患者基本信息
                if (Function.Update<Patientsinfo>(patientsinfo, dbConnection, ref trans) < 0)
                {
                    Err = "修改患者基本信息失败";
                    DBFunction.CloseConnService(dbConnection);
                    return -1;
                }
            }
            //保存诊断
            bool status = _diagnosisService.SaveDiagnosis(referral, @params.Diagnosis, trans, ref Err);
            if (!status)
            {
                trans.Rollback();
                DBFunction.CloseConnService(dbConnection);
                return -1;
            }
            trans.Commit();
            return 1;
        }
        public IDbConnection DBBind()
        {
            return dapper.GetDbConnection(conn);
        }
        /// <summary>
        /// 查询Refferral集合
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IEnumerable<Referral> QueryReferral(RequestQueryParam param, ref string Err)
        {
            try
            {
                IEnumerable<Referral> refList = dbConnection.GetAll<Referral>().Where(x => (x.Hospitalout == param.hospital || x.Hospitalinto == param.hospital) && x.Sign == param.sign);
                foreach (var item in refList)
                {
                    item.Patientsinfo = _patientsInfoService.SelectByCardNo(item.CardNo, item.InpatientNo, ref this.patientsinfo);
                }
                if (param.inputInfo.Length>0)
                {
                    refList = refList.Where(u => u.Patient.Contains(param.inputInfo));
                }
                
                Err = "查询成功！";
                return refList;
            }
            catch (Exception ex)
            {
                Err = ex.Message;
                return null;
            }
        }
        /// <summary>
        /// 通过Rid查询转诊信息
        /// </summary>
        /// <param name="Rid"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        public Referral GetReferralByRid(int Rid, ref string Err)
        {
            Referral referral = dbConnection.GetAll<Referral>().FirstOrDefault(f => f.ID == Rid);
            if (referral == null)
            {
                Err = "没有查询到对应的转诊信息";
                return null;
            }
            referral.Patientsinfo = _patientsInfoService.SelectByCardNo(referral.CardNo, referral.InpatientNo, ref this.patientsinfo);
            return referral;
        }
        /// <summary>
        /// 查询并修改转诊信息
        /// </summary>
        /// <param name="Rid"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public JsonResult QueryReferral(int Rid, string status)
        {
            string Err = string.Empty;
            Referral referral = this.GetReferralByRid(Rid, ref Err);
            referral.Status = status;

            referral.Timeinto = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            //修改
            DBFunction.OpenConnService(dbConnection);
            //开启事务
            IDbTransaction trans = dbConnection.BeginTransaction();
            bool isUpdate = Function.Update<Referral>(referral, dbConnection, ref trans) > 0;
            if (isUpdate)
            {
                trans.Commit();
                return Function.GetSuccessResult("修改成功！");
            }
            else
            {
                trans.Rollback();
                return Function.GetErrResult("修改失败！");
            }
        }
        /// <summary>
        /// 保存转诊信息
        /// </summary>
        /// <param name="reffral"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        public bool SaveOrReferral(Referral reffral, ref string Err)
        {
            Referral referral = this.GetReferralByRid(reffral.ID, ref Err);
            DBFunction.OpenConnService(dbConnection);
            IDbTransaction trans = dbConnection.BeginTransaction();
            bool isUpdate = Function.Update<Referral>(reffral, dbConnection, ref trans) > 0;
            if (isUpdate)
            {
                Err = "修改成功！";
                trans.Commit();
                DBFunction.CloseConnService(dbConnection);
                return true;
            }
            else
            {
                Err = "修改失败！"; 
                trans.Rollback();
                DBFunction.CloseConnService(dbConnection);
                return false;
            }
        }
        /// <summary>
        /// 修改返回结果
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <param name="reason"></param>
        /// <returns></returns>
        public JsonResult UpdateReferralResult(int id, string status, string reason)
        {
            string Err = string.Empty;
            Referral referral = this.GetReferralByRid(id, ref Err);
            if (referral == null)
            {
                return Function.GetErrResult(Err);
            }
            referral.Status = status;

            if (!string.IsNullOrWhiteSpace(reason))
            {
                referral.Reason = reason;
            }
            bool isUpdate = this.SaveOrReferral(referral, ref Err);
            return isUpdate ? Function.GetSuccessResult(Err) : Function.GetErrResult(Err);
        }

        public bool DeleteById(int id, ref string Err)
        {
            Referral referral = this.GetReferralByRid(id, ref Err);
            if (referral == null)
            {
                Err = "删除失败，没有找到对应的删除信息!";
                return false;
            }
            //开始事务
            DBFunction.OpenConnService(dbConnection);
            IDbTransaction trans = dbConnection.BeginTransaction();
            bool isDelete = SqlExec.Delete<Referral>(dbConnection, referral, trans);
            if (isDelete)
            {
                Err = "删除成功！";
                trans.Commit();
                return true;
            }
            else
            {
                Err = "删除失败！";
                trans.Rollback();
                return false;
            }
        }
        /// <summary>
        /// 删除返回结果
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public JsonResult DeleteByIdResult(int id)
        {
            string Err = string.Empty;
            bool isDelete = this.DeleteById(id, ref Err);
            if (isDelete)
            {
                return Function.GetSuccessResult(Err);
            }
            else
            {
                return Function.GetErrResult(Err);
            }
        }
    }
}
