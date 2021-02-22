using Dapper;
using HISDouble.Interfaces;
using Microsoft.Extensions.Configuration;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace HISDouble.DapperConfig
{
    public class OracleFactory: IOracleFactory
    {
        public OracleFactory(IDapperContext dapperContext)
        {
            this._dapperContext = dapperContext;
            this.db=this._dapperContext.GetDbConnection(root.GetConnectionString("OraclezjhisYB"));
        }
        private IDbConnection db = null;
        private IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        private   readonly DynamicParameters dynamicParameters = new DynamicParameters();
        private readonly List<OracleParameter> oracleParameters = new List<OracleParameter>();
        private readonly IDapperContext _dapperContext;

        public   void Add(string Name,DbType oracleDbType,ParameterDirection parameterDirection,object ?obj=null,int?size=null)
        {
            if (size.HasValue)
            {
                dynamicParameters.Add(Name, obj, oracleDbType, parameterDirection, size.Value);
            }
            else
            {
                dynamicParameters.Add(Name, obj, oracleDbType, parameterDirection);
            }
            
        }
       public  T Get<T> (string PreName,string OutName)
        {
         var result=   this.db.Execute(PreName, dynamicParameters, commandType: CommandType.StoredProcedure);
        T tval=    dynamicParameters.Get<T>(OutName);
            return tval;
        }
    }
}
