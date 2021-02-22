using Dapper;
using Dapper.Contrib.Extensions;
using Dapper.Oracle;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using static Dapper.Contrib.Extensions.SqlMapperExtensions;
using static HISDouble.Factory.DapperContext;

namespace HISDouble
{
    public static class DBFunction
    {


        private static SqlType sqltype = SqlType.Oracle;
        private static IConfigurationRoot root = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
        /// <summary>
        /// 数据库打开服务
        /// </summary>
        /// <param name="connection"></param>
        public static void OpenConnService(IDbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
                OpenConnService(connection);
            }
            else if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }
        /// <summary>
        /// 属性字典
        /// </summary>
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        /// <summary>
        /// key的属性字典
        /// </summary>
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();
        /// <summary>
        /// 表名字典
        /// </summary>
        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();
        public static TableNameMapperDelegate TableNameMapper;
        /// <summary>
        /// 数据库关闭服务
        /// </summary>
        /// <param name="connection"></param>
        public static void CloseConnService(IDbConnection connection)
        {
            if (connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            else if (connection.State == ConnectionState.Closed)
            {
                connection.Close();
                CloseConnService(connection);
            }
        }
        /// <summary>
        /// 新增语句 （自带主键）
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity"></param>
        /// <param name="connection"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        public static int Insert<T>(T entity, IDbConnection connection, ref IDbTransaction transaction, ref string Err, string PrimaryKey = "", string PrimaryKeyValue = "") where T : class
        {
            List<PropertyInfo> properties = TypePropertiesCache(entity.GetType());
            List<PropertyInfo> keypros = KeyPropertiesCache(entity.GetType());
            List<PropertyInfo> infos = properties.Except(keypros).ToList();
            string key = string.IsNullOrEmpty(PrimaryKey) ? "" : PrimaryKey + ",";
            string keyval = string.IsNullOrEmpty(PrimaryKeyValue) ? "" : PrimaryKeyValue + ",";
            System.Text.StringBuilder sbcolName = new System.Text.StringBuilder($"{key}");
            System.Text.StringBuilder sbValue = new System.Text.StringBuilder($"{keyval}");
            Newtonsoft.Json.Linq.JObject jObject = Newtonsoft.Json.JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(Newtonsoft.Json.JsonConvert.SerializeObject(entity));
            for (int i = 0; i < infos.Count; i++)
            {
                //赋值表的列名
                PropertyInfo property = infos.ElementAt(i);
                string ColName = property.Name;
                if (property.Name.ToUpper() == "AUDIT")
                {
                    ColName = $"\"{property.Name}\"";
                }
                sbcolName.Append(ColName);
                if (i < infos.Count - 1)
                {
                    sbcolName.Append(",");
                }
                string value = "";
                Type valueType = property.PropertyType;
                if (valueType.FullName == typeof(string).FullName)
                {
                    value = $"'{jObject.GetValue(property.Name).ToString()}'";
                }
                else if (valueType.FullName == typeof(int).FullName || valueType.FullName == typeof(double).FullName || valueType.FullName == typeof(float).FullName || valueType.FullName == typeof(long).FullName || valueType.FullName == typeof(decimal).FullName)
                {
                    value = jObject.GetValue(property.Name).ToString();
                }
                else if (valueType.FullName == typeof(DateTime).FullName || valueType.FullName.ToLower().Contains("datetime"))
                {
                    string time = "";
                    try
                    {
                        if (jObject.GetValue(property.Name).ToString().Length == "yyyy-MM-dd".Length)
                        {
                            time = DateTime.Parse(jObject.GetValue(property.Name).ToString()).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            time = DateTime.Parse(jObject.GetValue(property.Name).ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        }
                        Console.WriteLine(jObject.GetValue(property.Name).ToString());
                        Console.WriteLine(time);
                    }
                    catch (Exception)
                    {
                        time = "0001-01-01 00:00:00";
                    }
                    value = $"to_date('{time}','yyyy-mm-dd hh24:mi:ss')";
                }
                //赋值表的值
                sbValue.Append(value);
                if (i < infos.Count - 1)
                {
                    sbValue.Append(",");
                }
            }
            string Sql = $"insert into {GetTableName(typeof(T))} ({sbcolName}) values ({sbValue})";
            Console.WriteLine(Sql);
            long result = 1;
            try
            {
                result = connection.Execute(Sql, transaction: transaction);
                Err = $"新增表{GetTableName(typeof(T))}成功！";
            }
            catch (Exception ex)
            {
                result = -1;
                Err = ex.Message;

            }
            return (int)result;
        }
        /// <summary>
        /// 获取实体类中所有的属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<PropertyInfo> TypePropertiesCache(Type type)
        {
            if (TypeProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pis))
            {
                return pis.ToList();
            }
            PropertyInfo[] properties = type.GetProperties().Where(IsWriteable).ToArray();
            //properties = properties.Where(u => !u.PropertyType.IsClass).ToArray();
            TypeProperties[type.TypeHandle] = properties;
            return properties.ToList();
        }
        /// <summary>
        /// 获取类中Key标识的属性
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private static List<PropertyInfo> KeyPropertiesCache(Type type)
        {
            if (KeyProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
            {
                return pi.ToList();
            }
            List<PropertyInfo> allProperties = TypePropertiesCache(type);
            List<PropertyInfo> keyProperties = allProperties.Where((PropertyInfo p) => p.GetCustomAttributes(inherit: true).Any((object a) => a is KeyAttribute)).ToList();
            if (keyProperties.Count == 0)
            {
                PropertyInfo idProp = allProperties.Find((PropertyInfo p) => string.Equals(p.Name, "id", StringComparison.CurrentCultureIgnoreCase));
                if (idProp != null && !idProp.GetCustomAttributes(inherit: true).Any((object a) => a is ExplicitKeyAttribute))
                {
                    keyProperties.Add(idProp);
                }
            }
            KeyProperties[type.TypeHandle] = keyProperties;
            return keyProperties;
        }

        /// <summary>
        /// 判断是否可读
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        private static bool IsWriteable(PropertyInfo pi)
        {
            List<object> attributes = SqlMapper.AsList<object>((IEnumerable<object>)pi.GetCustomAttributes(typeof(WriteAttribute), inherit: false));
            if (attributes.Count != 1)
            {
                return true;
            }
            WriteAttribute writeAttribute = (WriteAttribute)attributes[0];
            return writeAttribute.Write;
        }
        private static string GetTableName(Type type)
        {
            if (TypeTableName.TryGetValue(type.TypeHandle, out string name))
            {
                return name;
            }
            if (TableNameMapper != null)
            {
                name = TableNameMapper(type);
            }
            else
            {
                dynamic tableAttrName = type.GetCustomAttribute<Dapper.Contrib.Extensions.TableAttribute>(inherit: false)?.Name ?? ((dynamic)type.GetCustomAttributes(inherit: false).FirstOrDefault((object attr) => attr.GetType().Name == "TableAttribute"))?.Name;
                if (tableAttrName != null)
                {
                    name = tableAttrName;
                }
                else
                {
                    name = type.Name + "s";
                    if (type.IsInterface && name.StartsWith("I"))
                    {
                        name = name.Substring(1);
                    }
                }
            }
            TypeTableName[type.TypeHandle] = name;
            return name;
        }
        #region SqlServer操作
        public static SqlConnection GetSqlConnection(string Name)
        {
            string SqlConnectName = root.GetConnectionString(Name).ToString();
            return new SqlConnection(SqlConnectName);
        }

        public static ArrayList GetSqlServerList(string sql)
        {
            SqlConnection conn = GetSqlConnection("SqlServerzjhisZB");
            try
            {
                // Console.WriteLine("读取数据之前"+JsonConvert.SerializeObject(conn));
                conn.Open();
                Console.WriteLine("读取数据之前");
                SqlCommand smd = new SqlCommand(sql, conn);
                DataTable dt = new DataTable();
                smd.Parameters.Clear();
                smd.CommandType = CommandType.Text;
                smd.CommandTimeout = 999;

                var reader = smd.ExecuteReader();
                Console.WriteLine("读取数据之后");
                dt.Load(reader);
                reader.Close();

                ArrayList array = DatasetToArrList(dt);
                return array;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null;
            }
            finally { conn.Close(); }
        }
        /// <summary>
        /// dataset 转换为 ArrayList
        /// </summary>
        /// <param name="ds"></param>
        /// <returns></returns>
        public static ArrayList DatasetToArrList(DataTable ds)
        {
            if (ds.Rows.Count == 0 || ds == null)
            {
                return null;
            }
            ArrayList TList = new ArrayList();
            for (int i = 0; i < ds.Rows.Count; i++)
            {
                DataRow dr = ds.Rows[i];
                Dictionary<string, string> dic = new Dictionary<string, string>();
                for (int j = 0; j < ds.Columns.Count; j++)
                {
                    DataColumn dc = ds.Columns[j];
                    dic.Add(dc.ToString(), dr.ItemArray[j].ToString());

                }
                object Tval = Function.GetT<object>(dic);
                TList.Add(Tval);
            }
            return TList;
        }
        #endregion


        #region 存储过程的操作
        public readonly static List<OracleParameter> oracleParameters = new List<OracleParameter>();
        public readonly static OracleDynamicParameters dynamicParameters = new OracleDynamicParameters();
       public static void Add(string name,OracleDbType dbType,ParameterDirection direction,object ? obj=null,int ?size=null)
        {
            OracleParameter oracleparameter;
            if (size.HasValue)
            {
                oracleparameter = new OracleParameter(name,dbType,size.Value,obj.ToString(),direction);
            }
            else
            {
                oracleparameter = new OracleParameter(name, dbType, obj.ToString(), direction);
            }
            oracleParameters.Add(oracleparameter);
        }
        public static void Add(string name, OracleDbType oracleDbType, ParameterDirection direction)
        {
            var oracleParameter = new OracleParameter(name, oracleDbType, direction);
            oracleParameters.Add(oracleParameter);
        }

        public static void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            ((SqlMapper.IDynamicParameters)dynamicParameters).AddParameters(command, identity);

            var oracleCommand = command as OracleCommand;

            if (oracleCommand != null)
            {
                oracleCommand.Parameters.AddRange(oracleParameters.ToArray());
            }
        }
      

        #endregion

    }
}
