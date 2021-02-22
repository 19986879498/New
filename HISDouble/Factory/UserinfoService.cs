using Dapper;
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
    public class UserinfoService : BaseServices, IUserinfoService
    {
        public UserinfoService(IDapperContext dapper)
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
        /// 验证用户
        /// </summary>
        /// <param name="userInfo"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        public bool ValidateUser(ref Userinfo userInfo, ref string Err)
        {
            userInfo = this.GetUserInfo(userInfo.Username, userInfo.Userpwd);
            if (userInfo == null)
            {
                Err = "用户验证失败！";
                return false;
            }
            Err = "用户验证成功！";
            return true;
        }
        /// <summary>
        /// 验证登录
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Password"></param>
        /// <returns></returns>
        public Userinfo GetUserInfo(string UserName, string Password)
        {
            Userinfo userinfo= dbConnection.GetAll<Userinfo>().FirstOrDefault(f => f.Username == UserName && f.Userpwd == Password&&f.IsAvailable==1);
            if (userinfo==null)
            {
                return null;
            }
            userinfo.Name = Function.GetName(userinfo.Username,this.dbConnection,userinfo.Hospitalcode);
            return userinfo;

        }
        /// <summary>
        /// 查询患者信息列表
        /// </summary>
        /// <param name="param"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        public IEnumerable<Userinfo> GetUserinfos(RequestQueryParam param, ref string Err)
        {
            //IEnumerable<Userinfo> userinfos = dbConnection.GetAll<Userinfo>();
            IEnumerable<Userinfo> userinfos = null;
            if (param!=null&&param.inputInfo!="")
            {
                userinfos= this.dbConnection.Query<Userinfo>($"select *from zjhis.turn_USERINFO d where (d.Username like '%{param.inputInfo}%') and d.isavailable=1").ToList();
            }
            else
            {
                userinfos = dbConnection.GetAll<Userinfo>().Where(w=>w.IsAvailable==1);
            };
            if (userinfos == null)
            {
                Err = "查询用户信息失败！";
                return null;
            }
            List<Userinfo> userinfos1 = userinfos.Skip((param.pageNum - 1) * param.pageSize).Take(param.pageSize).ToList();
            foreach (var item in userinfos1)
            {
                item.Name = Function.GetName(item.Username, this.dbConnection,item.Hospitalcode);
            }
            return userinfos1;
        }
        /// <summary>
        /// 获取用户查询的api
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public JsonResult GetUserInfoResult(RequestQueryParam param)
        {
            string Err = string.Empty;
            List<Userinfo> userList = this.GetUserinfos(param, ref Err).ToList();
            if (userList == null)
            {
                return Function.GetErrResult(Err);
            }
            return Function.GetResultList<Userinfo>(userList);
        }
        /// <summary>
        /// 保存或修改用户信息
        /// </summary>
        /// <param name="userinfo"></param>
        /// <param name="Err"></param>
        /// <returns></returns>
        public bool SaveOrUpdateUser(Userinfo userinfo, ref string Err)
        {
            //Userinfo user = SqlExec.GetAll<Userinfo>(dbConnection).FirstOrDefault(f => f.Username == userinfo.Username && f.Userpwd == userinfo.Userpwd && f.Id == userinfo.Id);
            Userinfo user = SqlExec.GetAll<Userinfo>(dbConnection).FirstOrDefault(f => f.Username == userinfo.Username &&  f.Id == userinfo.Id);
            userinfo.IsAvailable = 1;
            try
            {
                DBFunction.OpenConnService(dbConnection);
                IDbTransaction trans = dbConnection.BeginTransaction();
                if (user == null)
                {

                    bool isInert = SqlExec.Insert<Userinfo>(dbConnection, userinfo, trans) > 0;
                    if (isInert)
                    {
                        Err = "用户表新增操作成功";
                        trans.Commit();
                        return isInert;
                    }
                    else
                    {
                        Err = "用户表新增操作失败";
                        trans.Rollback();
                        return isInert;
                    }
                }
                else
                {
                    bool isUpdate = SqlExec.Update<Userinfo>(dbConnection, userinfo, trans);
                    if (isUpdate)
                    {
                        Err = "用户表修改操作成功";
                        trans.Commit();
                        return isUpdate;
                    }
                    else
                    {
                        Err = "用户表修改操作失败";
                        trans.Rollback();
                        return isUpdate;
                    }

                }
            }
            catch (Exception ex)
            {
                this.ErrorMsg = ex.Message;
                return false;
            }
            finally
            {
                dbConnection.Close();
            }
        }
        /// <summary>
        /// 保存用户（返回json结果）
        /// </summary>
        /// <param name="userinfo"></param>
        /// <returns></returns>
        public JsonResult SaveUser(Userinfo userinfo)
        {
            string Err = string.Empty;
            bool IsExec = this.SaveOrUpdateUser(userinfo, ref Err);
            return Function.GetSuccessResult(Err);

        }
        /// <summary>
        /// 获取所有userinfo
        /// </summary>
        /// <returns></returns>
        public List<Userinfo> GetUserinfos(string usercode)
        {
         List<Userinfo>   userinfos = dbConnection.GetAll<Userinfo>().Where(w => w.IsAvailable == 1 && w.Username==usercode).ToList();
            foreach (var item in userinfos)
            {
                item.Name = Function.GetName(item.Username, this.dbConnection,item.Hospitalcode);
            }
            return userinfos;
        }
    }
}
