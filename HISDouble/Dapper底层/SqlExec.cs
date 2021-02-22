using Dapper;
using Dapper.Contrib.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace HISDouble
{
    public static class SqlExec
    {
        public interface IProxy
        {
            bool IsDirty
            {
                get;
                set;
            }
        }

        public interface ITableNameMapper
        {
            string GetTableName(Type type);
        }

        public delegate string GetDatabaseTypeDelegate(IDbConnection connection);

        public delegate string TableNameMapperDelegate(Type type);

        private static class ProxyGenerator
        {
            private static readonly Dictionary<Type, Type> TypeCache = new Dictionary<Type, Type>();

            private static AssemblyBuilder GetAsmBuilder(string name)
            {
                return AssemblyBuilder.DefineDynamicAssembly(new AssemblyName
                {
                    Name = name
                }, AssemblyBuilderAccess.Run);
            }

            public static T GetInterfaceProxy<T>()
            {
                Type typeOfT = typeof(T);
                if (TypeCache.TryGetValue(typeOfT, out Type i))
                {
                    return (T)Activator.CreateInstance(i);
                }
                AssemblyBuilder assemblyBuilder = GetAsmBuilder(typeOfT.Name);
                ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("SqlMapperExtensions." + typeOfT.Name);
                Type interfaceType = typeof(IProxy);
                TypeBuilder typeBuilder = moduleBuilder.DefineType(typeOfT.Name + "_" + Guid.NewGuid().ToString(), TypeAttributes.Public);
                typeBuilder.AddInterfaceImplementation(typeOfT);
                typeBuilder.AddInterfaceImplementation(interfaceType);
                MethodInfo setIsDirtyMethod = CreateIsDirtyProperty(typeBuilder);
                PropertyInfo[] properties = typeof(T).GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    bool isId = property.GetCustomAttributes(inherit: true).Any((object a) => a is KeyAttribute);
                    CreateProperty<T>(typeBuilder, property.Name, property.PropertyType, setIsDirtyMethod, isId);
                }
                Type generatedType = typeBuilder.CreateTypeInfo().AsType();
                TypeCache.Add(typeOfT, generatedType);
                return (T)Activator.CreateInstance(generatedType);
            }

            private static MethodInfo CreateIsDirtyProperty(TypeBuilder typeBuilder)
            {
                Type propType = typeof(bool);
                FieldBuilder field = typeBuilder.DefineField("_IsDirty", propType, FieldAttributes.Private);
                PropertyBuilder property = typeBuilder.DefineProperty("IsDirty", PropertyAttributes.None, propType, new Type[1]
                {
                propType
                });
                MethodBuilder currGetPropMthdBldr = typeBuilder.DefineMethod("get_IsDirty", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.SpecialName, propType, Type.EmptyTypes);
                ILGenerator currGetIl = currGetPropMthdBldr.GetILGenerator();
                currGetIl.Emit(OpCodes.Ldarg_0);
                currGetIl.Emit(OpCodes.Ldfld, field);
                currGetIl.Emit(OpCodes.Ret);
                MethodBuilder currSetPropMthdBldr = typeBuilder.DefineMethod("set_IsDirty", MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Final | MethodAttributes.Virtual | MethodAttributes.HideBySig | MethodAttributes.VtableLayoutMask | MethodAttributes.SpecialName, null, new Type[1]
                {
                propType
                });
                ILGenerator currSetIl = currSetPropMthdBldr.GetILGenerator();
                currSetIl.Emit(OpCodes.Ldarg_0);
                currSetIl.Emit(OpCodes.Ldarg_1);
                currSetIl.Emit(OpCodes.Stfld, field);
                currSetIl.Emit(OpCodes.Ret);
                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                MethodInfo getMethod = typeof(IProxy).GetMethod("get_IsDirty");
                MethodInfo setMethod = typeof(IProxy).GetMethod("set_IsDirty");
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);
                return currSetPropMthdBldr;
            }

            private static void CreateProperty<T>(TypeBuilder typeBuilder, string propertyName, Type propType, MethodInfo setIsDirtyMethod, bool isIdentity)
            {
                FieldBuilder field = typeBuilder.DefineField("_" + propertyName, propType, FieldAttributes.Private);
                PropertyBuilder property = typeBuilder.DefineProperty(propertyName, PropertyAttributes.None, propType, new Type[1]
                {
                propType
                });
                MethodBuilder currGetPropMthdBldr = typeBuilder.DefineMethod("get_" + propertyName, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, propType, Type.EmptyTypes);
                ILGenerator currGetIl = currGetPropMthdBldr.GetILGenerator();
                currGetIl.Emit(OpCodes.Ldarg_0);
                currGetIl.Emit(OpCodes.Ldfld, field);
                currGetIl.Emit(OpCodes.Ret);
                MethodBuilder currSetPropMthdBldr = typeBuilder.DefineMethod("set_" + propertyName, MethodAttributes.FamANDAssem | MethodAttributes.Family | MethodAttributes.Virtual | MethodAttributes.HideBySig, null, new Type[1]
                {
                propType
                });
                ILGenerator currSetIl = currSetPropMthdBldr.GetILGenerator();
                currSetIl.Emit(OpCodes.Ldarg_0);
                currSetIl.Emit(OpCodes.Ldarg_1);
                currSetIl.Emit(OpCodes.Stfld, field);
                currSetIl.Emit(OpCodes.Ldarg_0);
                currSetIl.Emit(OpCodes.Ldc_I4_1);
                currSetIl.Emit(OpCodes.Call, setIsDirtyMethod);
                currSetIl.Emit(OpCodes.Ret);
                if (isIdentity)
                {
                    Type keyAttribute = typeof(KeyAttribute);
                    ConstructorInfo myConstructorInfo = keyAttribute.GetConstructor(new Type[0]);
                    CustomAttributeBuilder attributeBuilder = new CustomAttributeBuilder(myConstructorInfo, new object[0]);
                    property.SetCustomAttribute(attributeBuilder);
                }
                property.SetGetMethod(currGetPropMthdBldr);
                property.SetSetMethod(currSetPropMthdBldr);
                MethodInfo getMethod = typeof(T).GetMethod("get_" + propertyName);
                MethodInfo setMethod = typeof(T).GetMethod("set_" + propertyName);
                typeBuilder.DefineMethodOverride(currGetPropMthdBldr, getMethod);
                typeBuilder.DefineMethodOverride(currSetPropMthdBldr, setMethod);
            }
        }

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> KeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ExplicitKeyProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> TypeProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>> ComputedProperties = new ConcurrentDictionary<RuntimeTypeHandle, IEnumerable<PropertyInfo>>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> GetQueries = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ConcurrentDictionary<RuntimeTypeHandle, string> TypeTableName = new ConcurrentDictionary<RuntimeTypeHandle, string>();

        private static readonly ISqlAdapter DefaultAdapter = new SqlServerAdapter();

        private static readonly Dictionary<string, ISqlAdapter> AdapterDictionary = new Dictionary<string, ISqlAdapter>
        {
            ["sqlconnection"] = new SqlServerAdapter(),
            ["sqlceconnection"] = new SqlCeServerAdapter(),
            ["npgsqlconnection"] = new PostgresAdapter(),
            ["sqliteconnection"] = new SQLiteAdapter(),
            ["mysqlconnection"] = new MySqlAdapter(),
            ["fbconnection"] = new FbAdapter()
        };

        public static TableNameMapperDelegate TableNameMapper;

        public static GetDatabaseTypeDelegate GetDatabaseType;

        public static async Task<T> GetAsync<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            Type type = typeof(T);
            if (!GetQueries.TryGetValue(type.TypeHandle, out string sql))
            {
                PropertyInfo key = GetSingleKey<T>("GetAsync");
                string name = GetTableName(type);
                sql = "SELECT * FROM " + name + " WHERE " + key.Name + " = @id";
                GetQueries[type.TypeHandle] = sql;
            }
            DynamicParameters dynParms = (DynamicParameters)(object)new DynamicParameters();
            dynParms.Add("@id", id);
            if (!type.IsInterface)
            {
                return (await SqlMapper.QueryAsync<T>(connection, sql, (object)dynParms, transaction, commandTimeout, (CommandType?)null).ConfigureAwait(continueOnCapturedContext: false)).FirstOrDefault();
            }
            IDictionary<string, object> res = (await SqlMapper.QueryAsync<object>(connection, sql, (object)dynParms, (IDbTransaction)null, (int?)null, (CommandType?)null).ConfigureAwait(continueOnCapturedContext: false)).FirstOrDefault() as IDictionary<string, object>;
            if (res == null)
            {
                return null;
            }
            T obj = ProxyGenerator.GetInterfaceProxy<T>();
            foreach (PropertyInfo property in TypePropertiesCache(type))
            {
                object val = res[property.Name];
                if (val != null)
                {
                    if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        Type genericType = Nullable.GetUnderlyingType(property.PropertyType);
                        if (genericType != null)
                        {
                            property.SetValue(obj, Convert.ChangeType(val, genericType), null);
                        }
                    }
                    else
                    {
                        property.SetValue(obj, Convert.ChangeType(val, property.PropertyType), null);
                    }
                }
            }
            ((IProxy)obj).IsDirty = false;
            return obj;
        }

        public static Task<IEnumerable<T>> GetAllAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            Type type = typeof(T);
            Type cacheType = typeof(List<T>);
            if (!GetQueries.TryGetValue(cacheType.TypeHandle, out string sql))
            {
                GetSingleKey<T>("GetAll");
                string name = GetTableName(type);
                sql = "SELECT * FROM " + name;
                GetQueries[cacheType.TypeHandle] = sql;
            }
            if (!type.IsInterface)
            {
                return SqlMapper.QueryAsync<T>(connection, sql, (object)null, transaction, commandTimeout, (CommandType?)null);
            }
            return GetAllAsyncImpl<T>(connection, transaction, commandTimeout, sql, type);
        }

        private static async Task<IEnumerable<T>> GetAllAsyncImpl<T>(IDbConnection connection, IDbTransaction transaction, int? commandTimeout, string sql, Type type) where T : class
        {
            IEnumerable<object> result = await SqlMapper.QueryAsync(connection, sql, (object)null, (IDbTransaction)null, (int?)null, (CommandType?)null).ConfigureAwait(continueOnCapturedContext: false);
            List<T> list = new List<T>();
            foreach (object item in result)
            {
                IDictionary<string, object> res = (IDictionary<string, object>)(dynamic)item;
                T obj = ProxyGenerator.GetInterfaceProxy<T>();
                foreach (PropertyInfo property in TypePropertiesCache(type))
                {
                    object val = res[property.Name];
                    if (val != null)
                    {
                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            Type genericType = Nullable.GetUnderlyingType(property.PropertyType);
                            if (genericType != null)
                            {
                                property.SetValue(obj, Convert.ChangeType(val, genericType), null);
                            }
                        }
                        else
                        {
                            property.SetValue(obj, Convert.ChangeType(val, property.PropertyType), null);
                        }
                    }
                }
                ((IProxy)obj).IsDirty = false;
                list.Add(obj);
            }
            return list;
        }

        public static Task<int> InsertAsync<T>(this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null, ISqlAdapter sqlAdapter = null) where T : class
        {
            Type type = typeof(T);
            sqlAdapter = (sqlAdapter ?? GetFormatter(connection));
            bool isList = false;
            if (type.IsArray)
            {
                isList = true;
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo.ImplementedInterfaces.Any((Type ti) => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    isList = true;
                    type = type.GetGenericArguments()[0];
                }
            }
            string name = GetTableName(type);
            StringBuilder sbColumnList = new StringBuilder(null);
            List<PropertyInfo> allProperties = TypePropertiesCache(type);
            List<PropertyInfo> keyProperties = KeyPropertiesCache(type).ToList();
            List<PropertyInfo> computedProperties = ComputedPropertiesCache(type);
            List<PropertyInfo> allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            for (int j = 0; j < allPropertiesExceptKeyAndComputed.Count; j++)
            {
                PropertyInfo property = allPropertiesExceptKeyAndComputed[j];
                sqlAdapter.AppendColumnName(sbColumnList, property.Name);
                if (j < allPropertiesExceptKeyAndComputed.Count - 1)
                {
                    sbColumnList.Append(", ");
                }
            }
            StringBuilder sbParameterList = new StringBuilder(null);
            for (int i = 0; i < allPropertiesExceptKeyAndComputed.Count; i++)
            {
                PropertyInfo property2 = allPropertiesExceptKeyAndComputed[i];
                sbParameterList.AppendFormat("@{0}", property2.Name);
                if (i < allPropertiesExceptKeyAndComputed.Count - 1)
                {
                    sbParameterList.Append(", ");
                }
            }
            if (!isList)
            {
                return sqlAdapter.InsertAsync(connection, transaction, commandTimeout, name, sbColumnList.ToString(), sbParameterList.ToString(), keyProperties, entityToInsert);
            }
            string cmd = $"INSERT INTO {name} ({sbColumnList}) values ({sbParameterList})";
            return SqlMapper.ExecuteAsync(connection, cmd, (object)entityToInsert, transaction, commandTimeout, (CommandType?)null);
        }

        public static async Task<bool> UpdateAsync<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            IProxy proxy = entityToUpdate as IProxy;
            if (proxy != null && !proxy.IsDirty)
            {
                return false;
            }
            Type type = typeof(T);
            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo.ImplementedInterfaces.Any((Type ti) => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    type = type.GetGenericArguments()[0];
                }
            }
            List<PropertyInfo> keyProperties = KeyPropertiesCache(type).ToList();
            List<PropertyInfo> explicitKeyProperties = ExplicitKeyPropertiesCache(type);
            if (keyProperties.Count == 0 && explicitKeyProperties.Count == 0)
            {
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");
            }
            string name = GetTableName(type);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update {0} set ", name);
            List<PropertyInfo> allProperties = TypePropertiesCache(type);
            keyProperties.AddRange(explicitKeyProperties);
            List<PropertyInfo> computedProperties = ComputedPropertiesCache(type);
            List<PropertyInfo> nonIdProps = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            ISqlAdapter adapter = GetFormatter(connection);
            for (int j = 0; j < nonIdProps.Count; j++)
            {
                PropertyInfo property = nonIdProps[j];
                adapter.AppendColumnNameEqualsValue(sb, property.Name);
                if (j < nonIdProps.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" where ");
            for (int i = 0; i < keyProperties.Count; i++)
            {
                PropertyInfo property2 = keyProperties[i];
                adapter.AppendColumnNameEqualsValue(sb, property2.Name);
                if (i < keyProperties.Count - 1)
                {
                    sb.Append(" and ");
                }
            }
            return await SqlMapper.ExecuteAsync(connection, sb.ToString(), (object)entityToUpdate, transaction, commandTimeout, (CommandType?)null).ConfigureAwait(continueOnCapturedContext: false) > 0;
        }

        public static async Task<bool> DeleteAsync<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToDelete == null)
            {
                throw new ArgumentException("Cannot Delete null Object", "entityToDelete");
            }
            Type type = typeof(T);
            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo.ImplementedInterfaces.Any((Type ti) => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    type = type.GetGenericArguments()[0];
                }
            }
            List<PropertyInfo> keyProperties = KeyPropertiesCache(type);
            List<PropertyInfo> explicitKeyProperties = ExplicitKeyPropertiesCache(type);
            if (keyProperties.Count == 0 && explicitKeyProperties.Count == 0)
            {
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");
            }
            string name = GetTableName(type);
            List<PropertyInfo> allKeyProperties = keyProperties.Concat(explicitKeyProperties).ToList();
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("DELETE FROM {0} WHERE ", name);
            ISqlAdapter adapter = GetFormatter(connection);
            for (int i = 0; i < allKeyProperties.Count; i++)
            {
                PropertyInfo property = allKeyProperties[i];
                adapter.AppendColumnNameEqualsValue(sb, property.Name);
                if (i < allKeyProperties.Count - 1)
                {
                    sb.Append(" AND ");
                }
            }
            return await SqlMapper.ExecuteAsync(connection, sb.ToString(), (object)entityToDelete, transaction, commandTimeout, (CommandType?)null).ConfigureAwait(continueOnCapturedContext: false) > 0;
        }

        public static async Task<bool> DeleteAllAsync<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            Type type = typeof(T);
            string statement = "DELETE FROM " + GetTableName(type);
            return await SqlMapper.ExecuteAsync(connection, statement, (object)null, transaction, commandTimeout, (CommandType?)null).ConfigureAwait(continueOnCapturedContext: false) > 0;
        }

        private static List<PropertyInfo> ComputedPropertiesCache(Type type)
        {
            if (ComputedProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
            {
                return pi.ToList();
            }
            List<PropertyInfo> computedProperties = (from p in TypePropertiesCache(type)
                                                     where p.GetCustomAttributes(inherit: true).Any((object a) => a is ComputedAttribute)
                                                     select p).ToList();
            ComputedProperties[type.TypeHandle] = computedProperties;
            return computedProperties;
        }

        private static List<PropertyInfo> ExplicitKeyPropertiesCache(Type type)
        {
            if (ExplicitKeyProperties.TryGetValue(type.TypeHandle, out IEnumerable<PropertyInfo> pi))
            {
                return pi.ToList();
            }
            List<PropertyInfo> explicitKeyProperties = (from p in TypePropertiesCache(type)
                                                        where p.GetCustomAttributes(inherit: true).Any((object a) => a is ExplicitKeyAttribute)
                                                        select p).ToList();
            ExplicitKeyProperties[type.TypeHandle] = explicitKeyProperties;
            return explicitKeyProperties;
        }

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

        private static PropertyInfo GetSingleKey<T>(string method)
        {
            Type type = typeof(T);
            List<PropertyInfo> keys = KeyPropertiesCache(type);
            List<PropertyInfo> explicitKeys = ExplicitKeyPropertiesCache(type);
            int keyCount = keys.Count + explicitKeys.Count;
            if (keyCount > 1)
            {
                throw new DataException($"{method}<T> only supports an entity with a single [Key] or [ExplicitKey] property. [Key] Count: {keys.Count}, [ExplicitKey] Count: {explicitKeys.Count}");
            }
            if (keyCount == 0)
            {
                throw new DataException(method + "<T> only supports an entity with a [Key] or an [ExplicitKey] property");
            }
            if (keys.Count <= 0)
            {
                return explicitKeys[0];
            }
            return keys[0];
        }

        public static T Get<T>(this IDbConnection connection, dynamic id, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            //IL_0077: Unknown result type (might be due to invalid IL or missing references)
            //IL_007d: Expected O, but got Unknown
            Type type = typeof(T);
            if (!GetQueries.TryGetValue(type.TypeHandle, out string sql))
            {
                PropertyInfo key = GetSingleKey<T>("Get");
                string name = GetTableName(type);
                sql = "select * from " + name + " where " + key.Name + " = @id";
                GetQueries[type.TypeHandle] = sql;
            }
            DynamicParameters dynParms = (DynamicParameters)(object)new DynamicParameters();
            dynParms.Add("@id", id);
            T obj;
            if (type.IsInterface)
            {
                IDictionary<string, object> res = SqlMapper.Query(connection, sql, (object)dynParms, (IDbTransaction)null, true, (int?)null, (CommandType?)null).FirstOrDefault() as IDictionary<string, object>;
                if (res == null)
                {
                    return null;
                }
                obj = ProxyGenerator.GetInterfaceProxy<T>();
                foreach (PropertyInfo property in TypePropertiesCache(type))
                {
                    object val = res[property.Name];
                    if (val != null)
                    {
                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            Type genericType = Nullable.GetUnderlyingType(property.PropertyType);
                            if (genericType != null)
                            {
                                property.SetValue(obj, Convert.ChangeType(val, genericType), null);
                            }
                        }
                        else
                        {
                            property.SetValue(obj, Convert.ChangeType(val, property.PropertyType), null);
                        }
                    }
                }
                ((IProxy)obj).IsDirty = false;
            }
            else
            {
                obj = SqlMapper.Query<T>(connection, sql, (object)dynParms, transaction, true, commandTimeout, (CommandType?)null).FirstOrDefault();
            }
            return obj;
        }

        public static IEnumerable<T> GetAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            Type type = typeof(T);
            Type cacheType = typeof(List<T>);
            if (!GetQueries.TryGetValue(cacheType.TypeHandle, out string sql))
            {
                GetSingleKey<T>("GetAll");
                string name = GetTableName(type);
                sql = "select * from " + name;
                GetQueries[cacheType.TypeHandle] = sql;
            }
            if (!type.IsInterface)
            {
                return SqlMapper.Query<T>(connection, sql, (object)null, transaction, true, commandTimeout, (CommandType?)null);
            }
            IEnumerable<object> result = SqlMapper.Query(connection, sql, (object)null, (IDbTransaction)null, true, (int?)null, (CommandType?)null);
            List<T> list = new List<T>();
            foreach (object item in result)
            {
                IDictionary<string, object> res = (IDictionary<string, object>)(dynamic)item;
                T obj = ProxyGenerator.GetInterfaceProxy<T>();
                foreach (PropertyInfo property in TypePropertiesCache(type))
                {
                    object val = res[property.Name];
                    if (val != null)
                    {
                        if (property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            Type genericType = Nullable.GetUnderlyingType(property.PropertyType);
                            if (genericType != null)
                            {
                                property.SetValue(obj, Convert.ChangeType(val, genericType), null);
                            }
                        }
                        else
                        {
                            property.SetValue(obj, Convert.ChangeType(val, property.PropertyType), null);
                        }
                    }
                }
                ((IProxy)obj).IsDirty = false;
                list.Add(obj);
            }
            return list;
        }
        /// <summary>
        /// 添加查询方法
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="connection"></param>
        /// <returns></returns>
        public static List<T> ViewGetAll<T>(this IDbConnection connection)
        {
            Type type = typeof(T);
            string TableName = GetTableName(type);
            string SqlEx = $"SELECT * FROM {TableName}";
            return connection.Query<T>(SqlEx).ToList();
        }

        public static List<T> ViewGetAll<T>(this IDbConnection connection,string sqlwhere)
        {
            Type type = typeof(T);
            string TableName = GetTableName(type);
            string SqlEx = $"SELECT * FROM {TableName} "+sqlwhere;
            return connection.Query<T>(SqlEx).ToList();
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

        public static long Insert<T>(this IDbConnection connection, T entityToInsert, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            bool isList = false;
            Type type = typeof(T);
            if (type.IsArray)
            {
                isList = true;
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo.ImplementedInterfaces.Any((Type ti) => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    isList = true;
                    type = type.GetGenericArguments()[0];
                }
            }
            string name = GetTableName(type);
            StringBuilder sbColumnList = new StringBuilder(null);
            List<PropertyInfo> allProperties = TypePropertiesCache(type);
            List<PropertyInfo> keyProperties = KeyPropertiesCache(type);
            List<PropertyInfo> computedProperties = ComputedPropertiesCache(type);
            List<PropertyInfo> allPropertiesExceptKeyAndComputed = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            ISqlAdapter adapter = GetFormatter(connection);
            allProperties = allProperties.Where(r => !r.PropertyType.FullName.ToUpper().Contains("HISDOUBLE.MODELS")).ToList();
            for (int j = 0; j < allProperties.Count; j++)
            {
                PropertyInfo property = allProperties[j];
                    adapter.AppendColumnName(sbColumnList, property.Name);
                    if (j < allProperties.Count - 1)
                    {
                        sbColumnList.Append(", ");
                    }
                
            }
            sbColumnList= sbColumnList.Replace("]", "").Replace("[", "");
            StringBuilder sbParameterList = new StringBuilder(null);
            JObject jobj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject((T)entityToInsert));
            for (int i = 0; i < allProperties.Count; i++)
            {
                PropertyInfo property2 = allProperties[i];
                Type type1 = property2.PropertyType;
                string value = "";
                if (type1 == typeof(DateTime) || type1.FullName.Contains("DateTime"))
                {
                    try
                    {
                        string time = string.IsNullOrEmpty(jobj.GetValue(property2.Name).ToString()) ? "0001-01-01 00:00:00" : DateTime.Parse(jobj.GetValue(property2.Name).ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        value = $"to_date('{time}','yyyy-mm-dd hh24:mi:ss')";
                    }
                    catch
                    {
                        value = $"to_date('0001-01-01 00:00:00','yyyy-mm-dd hh24:mi:ss')";
                    }
                }
                else if (type1 == typeof(int) || type1 == typeof(double) || type1 == typeof(decimal) || type1 == typeof(float) || type1 == typeof(long) || type1 == typeof(short))
                {
                    value = jobj.GetValue(property2.Name) == null?"": jobj.GetValue(property2.Name).ToString();

                }
                //           else if (type1.IsClass)
                //           {
                ////后期再次进行修改

                //           }
                else
                {
                    value = $"'{jobj.GetValue(property2.Name).ToString()}'";
                }
                sbParameterList.AppendFormat("{0}", value);
                if (i < allProperties.Count - 1)
                {
                    sbParameterList.Append(", ");
                }
            }
            bool wasClosed = connection.State == ConnectionState.Closed;
            if (wasClosed)
            {
                connection.Open();
            }
            int returnVal;
            if (!isList)
            {
                string cmd = $"insert into {name} ({sbColumnList}) values ({sbParameterList})";
                Console.WriteLine("执行的sql语句是：\n"+cmd);
                returnVal = connection.Execute(cmd,transaction);
            }
            else
            {
                string cmd = $"insert into {name} ({sbColumnList}) values ({sbParameterList})";
                Console.WriteLine("执行的sql语句是：\n" + cmd);
                returnVal = connection.Execute(cmd , transaction);
            }
            if (wasClosed)
            {
                connection.Close();
            }
            return returnVal;
        }

        public static bool Update<T>(this IDbConnection connection, T entityToUpdate, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            IProxy proxy = entityToUpdate as IProxy;
            if (proxy != null && !proxy.IsDirty)
            {
                return false;
            }
            Type type = typeof(T);
            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo.ImplementedInterfaces.Any((Type ti) => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    type = type.GetGenericArguments()[0];
                }
            }
            List<PropertyInfo> keyProperties = KeyPropertiesCache(type).ToList();
            List<PropertyInfo> explicitKeyProperties = ExplicitKeyPropertiesCache(type);
            if (keyProperties.Count == 0 && explicitKeyProperties.Count == 0)
            {
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");
            }
            string name = GetTableName(type);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("update {0} set ", name);
            List<PropertyInfo> allProperties = TypePropertiesCache(type);
            keyProperties.AddRange(explicitKeyProperties);
            List<PropertyInfo> computedProperties = ComputedPropertiesCache(type);
            List<PropertyInfo> nonIdProps = allProperties.Except(keyProperties.Union(computedProperties)).ToList();
            ISqlAdapter adapter = GetFormatter(connection);
            JObject jobj = JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject((T)entityToUpdate));
            for (int j = 0; j < nonIdProps.Count; j++)
            {
                PropertyInfo property = nonIdProps[j];
                Type type1 = property.PropertyType;
                string value = "";
                if (type1 == typeof(DateTime) || type1.FullName.Contains("DateTime"))
                {
                    try
                    {
                        string time = string.IsNullOrEmpty(jobj.GetValue(property.Name).ToString()) ? "0001-01-01 00:00:00" : DateTime.Parse(jobj.GetValue(property.Name).ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        value = $"to_date('{time}','yyyy-mm-dd hh24:mi:ss')";
                    }
                    catch
                    {
                        value = $"to_date('0001-01-01 00:00:00','yyyy-mm-dd hh24:mi:ss')";
                    }
                }
                else if (type1 == typeof(int) || type1 == typeof(double) || type1 == typeof(decimal) || type1 == typeof(float) || type1 == typeof(long) || type1 == typeof(short))
                {
                    value = jobj.GetValue(property.Name) == null ? "" : jobj.GetValue(property.Name).ToString();

                }
                //           else if (type1.IsClass)
                //           {
                ////后期再次进行修改

                //           }
                
                else
                {
                    value = $"'{jobj.GetValue(property.Name).ToString()}'";
                }
                sb.Replace("[", "").Replace("]", "");
                string ProName = property.Name;
                if (property.Name.ToUpper()== "AUDIT")
                {
                    ProName = "\"" + property.Name + "\"";
                }
                sb.Append(ProName + "=" + value );
                if (j < nonIdProps.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.Append(" where ");
            for (int i = 0; i < keyProperties.Count; i++)
            {
                PropertyInfo property2 = keyProperties[i];
                string value = "";
                Type type1= property2.PropertyType;
                if (type1 == typeof(DateTime) || type1.FullName.Contains("DateTime"))
                {
                    try
                    {
                        string time = string.IsNullOrEmpty(jobj.GetValue(property2.Name).ToString()) ? "0001-01-01 00:00:00" : DateTime.Parse(jobj.GetValue(property2.Name).ToString()).ToString("yyyy-MM-dd HH:mm:ss");
                        value = $"to_date('{time}','yyyy-mm-dd hh24:mi:ss')";
                    }
                    catch
                    {
                        value = $"to_date('0001-01-01 00:00:00','yyyy-mm-dd hh24:mi:ss')";
                    }
                }
                else if (type1 == typeof(int) || type1 == typeof(double) || type1 == typeof(decimal) || type1 == typeof(float) || type1 == typeof(long) || type1 == typeof(short))
                {
                    value = jobj.GetValue(property2.Name) == null ? "" : jobj.GetValue(property2.Name).ToString();

                }
                //           else if (type1.IsClass)
                //           {
                ////后期再次进行修改

                //           }
                else
                {
                    value = $"'{jobj.GetValue(property2.Name).ToString()}'";
                }
                sb.Replace("[", "").Replace("]", "");
                string ColName = property2.Name;
                if (property2.Name.ToUpper()== "AUDIT")
                {
                    ColName = "\"" + property2.Name + "\"";
                }
                sb.Append(ColName + "=" + value);

                if (i < keyProperties.Count - 1)
                {
                    sb.Append(" and ");
                }
            }
            Console.WriteLine("执行的sql语句" + sb.ToString()) ;
            int updated = SqlMapper.Execute(connection, sb.ToString().Replace("[", "").Replace("]", ""), (object)entityToUpdate, transaction, commandTimeout, (CommandType?)null);
            return updated > 0;
        }
        private static JObject GetPareterToJobj<T>(T intance) where T:class
        {
            return JsonConvert.DeserializeObject<JObject>(JsonConvert.SerializeObject(intance));
        }
        public static bool Delete<T>(this IDbConnection connection, T entityToDelete, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            if (entityToDelete == null)
            {
                throw new ArgumentException("Cannot Delete null Object", "entityToDelete");
            }
            Type type = typeof(T);
            if (type.IsArray)
            {
                type = type.GetElementType();
            }
            else if (type.IsGenericType)
            {
                TypeInfo typeInfo = type.GetTypeInfo();
                if (typeInfo.ImplementedInterfaces.Any((Type ti) => ti.IsGenericType && ti.GetGenericTypeDefinition() == typeof(IEnumerable<>)) || typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                {
                    type = type.GetGenericArguments()[0];
                }
            }
            List<PropertyInfo> keyProperties = KeyPropertiesCache(type).ToList();
            List<PropertyInfo> explicitKeyProperties = ExplicitKeyPropertiesCache(type);
            if (keyProperties.Count == 0 && explicitKeyProperties.Count == 0)
            {
                throw new ArgumentException("Entity must have at least one [Key] or [ExplicitKey] property");
            }
            string name = GetTableName(type);
            keyProperties.AddRange(explicitKeyProperties);
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("delete from {0} where ", name);
            ISqlAdapter adapter = GetFormatter(connection);
            JObject param = GetPareterToJobj<T>(entityToDelete);
            for (int i = 0; i < keyProperties.Count; i++)
            {
                PropertyInfo property = keyProperties[i];
                string value = property.PropertyType == typeof(int) ? $"  {param.GetValue(property.Name).ToString()}" : property.PropertyType == typeof(DateTime) || property.PropertyType.FullName.Contains("DateTime") ? $"to_date('{param.GetValue(property.Name).ToString()}','yyyy-mm-dd hh24:mi:ss')" : $"'{param.GetValue(property.Name).ToString()}'";
                sb.Replace("[", "").Replace("]", "").Append( $"{property.Name}={value}" );
                if (i < keyProperties.Count - 1)
                {
                    sb.Append(" and ");
                }
            }
            Console.WriteLine("删除的语句："+sb.ToString());
            int deleted = SqlMapper.Execute(connection, sb.ToString(), transaction);
            return deleted > 0;
        }

        public static bool DeleteAll<T>(this IDbConnection connection, IDbTransaction transaction = null, int? commandTimeout = null) where T : class
        {
            Type type = typeof(T);
            string name = GetTableName(type);
            string statement = "delete from " + name;
            int deleted = SqlMapper.Execute(connection, statement, (object)null, transaction, commandTimeout, (CommandType?)null);
            return deleted > 0;
        }

        private static ISqlAdapter GetFormatter(IDbConnection connection)
        {
            string name = GetDatabaseType?.Invoke(connection).ToLower() ?? connection.GetType().Name.ToLower();
            if (!AdapterDictionary.TryGetValue(name, out ISqlAdapter adapter))
            {
                return DefaultAdapter;
            }
            return adapter;
        }



    }
}
