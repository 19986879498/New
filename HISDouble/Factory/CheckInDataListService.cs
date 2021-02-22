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
   public class CheckInDataListService: BaseServices, ICheckInDataListService
    {
        public CheckInDataListService(IDapperContext dapper)
        {
            this.dapper = dapper;
            this.dbConnection = dapper.GetDbConnection(conn);
        }

        public IDapperContext dapper { get; set; }

        private IDbConnection dbConnection=null;
        /// <summary>
        /// 返回的错误信息
        /// </summary>
        public string ErrMsg = "";
        /// <summary>
        /// 连接字符串
        /// </summary>
        private string conn = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build().GetConnectionString("ZJhisConnStr");
        /// <summary>
        /// 保存打卡数据并返回结果
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public JsonResult SaveCheckindata(CheckInDataList obj)
        {
            if (obj.CheckData==null)
            {
                return Function.GetErrResult("打卡对象不得为空（null）");
            }
            if (obj.CheckData.Count==0)
            {
                return Function.GetErrResult("打卡对象的数据的数量为0");
            }
            if (!SaveCheck(obj))
            {
                return Function.GetErrResult(ErrMsg);
            }
            return Function.GetSuccessResult(ErrMsg);
        }
        /// <summary>
        /// 保存打卡数据
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public bool SaveCheck(CheckInDataList obj)
        {
            DBFunction.OpenConnService(dbConnection);
           IDbTransaction trans= this.dbConnection.BeginTransaction();
            ulong start = (ulong)Convert.ToInt32(obj.StartTime);
            ulong end=(ulong)Convert.ToInt32(obj.EndTime);
            IEnumerable<View_GetCheckindata> checkDelDatas = dbConnection.GetAll<View_GetCheckindata>().Where(r => r.DEPT_ID == obj.DEPT_ID &&((ulong)Convert.ToInt32(r.CHECKIN_TIME)>start&&(ulong)Convert.ToInt32(r.CHECKIN_TIME)< end));

            //先删除，后新增
            if (checkDelDatas.Count()>0)
            {
                foreach (var item in checkDelDatas)
                {
                    bool isDelete = SqlExec.Delete<View_GetCheckindata>(dbConnection, item, trans);
                    if (isDelete == false)
                    {
                        trans.Rollback();
                        DBFunction.CloseConnService(dbConnection);
                        ErrMsg = "删除失败！";
                        return false;
                    }
                }
            }
            //新增 
            foreach (View_GetCheckindata item in obj.CheckData)
            {
                if (item==null)
                {
                    ErrMsg = "对象为空，保存失败";
                    DBFunction.CloseConnService(dbConnection);
                    trans.Rollback();
                    return false;
                }
                bool isInsert = SqlExec.Insert<View_GetCheckindata>(dbConnection, item, trans) > 0;
                if (isInsert==false)
                {
                    trans.Rollback();
                    DBFunction.CloseConnService(dbConnection);
                    ErrMsg = "保存失败！";
                    return false;
                }
            }
            trans.Commit();
            DBFunction.CloseConnService(dbConnection);
            ErrMsg = "保存成功！";
            return true;
        }
    }
}
