using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MySqlOrm
{
    public class DBHelper
    {
        public MySqlConnection CreateConnection(string connectionString)
        {
            MySqlConnection connection = new MySqlConnection(connectionString);
            return connection;
        }

        public MySqlCommand CreateCommand(MySqlConnection connnection, Model2Sql model)
        {
            MySqlCommand command = new MySqlCommand(model.Sql, connnection);
            foreach (var item in model.Param)
            {
                command.Parameters.AddWithValue(item.Key, item.Value);
            }
            return command;
        }

        public MySqlDataAdapter CreateAdapter(MySqlConnection connection, Model2Sql model)
        {
            MySqlDataAdapter adapter = new MySqlDataAdapter(CreateCommand(connection, model));
            return adapter;
        }


        /// <summary>
        /// 检查是否有主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public bool CheckKey<T>()
        {
            List<PropertyInfo> key = ReflectionHelper.GetKeys<T>();
            if (!key.Any())
            {
                throw new ArgumentException("模型未设置主键");
            }
            return true;
        }

        public Model2Sql Model2SQL<T>(Model2Db sqlType, T model)
        {
            switch (sqlType)
            {
                case Model2Db.Add:
                case Model2Db.Delete:
                case Model2Db.Update:
                    if (model == null)
                    {
                        throw new ArgumentException("模型不能为null");
                    }
                    break;
                case Model2Db.Query:
                case Model2Db.Count:
                    break;
                default:
                    break;
            }

            CheckKey<T>();
            Model2Sql sqlObject = new Model2Sql();
            sqlObject.SqlType = sqlType;
            List<PropertyInfo> keys = ReflectionHelper.GetKeys<T>();
            List<PropertyInfo> nonKeys = ReflectionHelper.GetNonKey<T>();
            string tableName = ReflectionHelper.GetTypeName<T>();
            switch (sqlType)
            {
                case Model2Db.Add:
                    sqlObject.Sql = string.Format("INSERT INTO {0} ({1}) VALUES (@{2})", tableName, string.Join(",", ReflectionHelper.GetPropertys<T>().Select(p => p.Name).ToArray()), string.Join(",@", ReflectionHelper.GetPropertys<T>().Select(p => p.Name).ToArray()));
                    ReflectionHelper.GetPropertys<T>().ToList().ForEach((p) => { sqlObject.Param.Add("@" + p.Name, p.GetValue(model)); });
                    break;
                case Model2Db.Delete:
                    sqlObject.Sql = string.Format("DELETE FROM {0} WHERE {1}", tableName, CreatWhere(keys));
                    PropertyAdd2Param<T>(model, sqlObject, keys);
                    break;
                case Model2Db.Update:
                    sqlObject.Sql = string.Format("UPDATE {0} {1} WHERE {2}", tableName, CreateUpdate(nonKeys), CreatWhere(keys));
                    PropertyAdd2Param<T>(model, sqlObject, nonKeys);
                    PropertyAdd2Param<T>(model, sqlObject, keys);
                    break;
                case Model2Db.Query:

                    IOrder order = new Order();
                    foreach (var item in keys)
                    {
                        order.Add(item.Name, OrderDirection.Desc);
                    } 

                    sqlObject.Sql = string.Format("SELECT {0} FROM {1} ORDER BY {2}", CreateQuery<T>(), tableName,GetOrderSQL<T>(order).Sql);
                    break;
                case Model2Db.Count:
                    sqlObject.Sql = string.Format("SELECT COUNT(*) FROM {0}",tableName);
                    break;
                default:
                    break;
            }

            return sqlObject;
        }


        public Model2Sql Model2SQL<T>(Model2Db sqlType, object model)
        {
            CheckKey<T>();
            Model2Sql sqlObject = new Model2Sql();
            sqlObject.SqlType = sqlType;
            string tableName = ReflectionHelper.GetTypeName<T>();
            List<PropertyInfo> keys = ReflectionHelper.GetKeys<T>();
            List<PropertyInfo> objectKeys = ReflectionHelper.GetObjectKeys(keys, model);


            if (!objectKeys.Any())
            {
                throw new ArgumentException("匿名类中不包括表主键");
            }

            switch (sqlType)
            {
                case Model2Db.Add:
                    break;
                case Model2Db.Delete:
                    sqlObject.Sql = string.Format("DELETE FROM {0} WHERE {1}", tableName, CreatWhere(keys));
                    PropertyAdd2Param(model, sqlObject, objectKeys);
                    break;
                case Model2Db.Update:
                    List<PropertyInfo> nonKeys = ReflectionHelper.GetPropertysExceptBy(ReflectionHelper.GetPropertys(model), keys);
                    sqlObject.Sql = string.Format("UPDATE {0} {1} WHERE {2}", tableName, CreateUpdate(nonKeys), CreatWhere(keys));
                    PropertyAdd2Param(model, sqlObject, nonKeys);
                    PropertyAdd2Param(model, sqlObject, objectKeys);
                    break;
                case Model2Db.Query:
                    sqlObject.Sql = string.Format("SELECT {0} FROM {1} WHERE {2}", CreateQuery<T>(), tableName, CreatWhere(objectKeys));
                    PropertyAdd2Param(model, sqlObject, objectKeys);
                    break;
                default:
                    break;
            }

            return sqlObject;
        }


        /// <summary>
        /// 将IQuery查询条件转化为sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Model2Sql Model2SQL<T>(IQuery query)
        {
            CheckKey<T>();
            List<PropertyInfo> keys = ReflectionHelper.GetKeys<T>();
            IOrder order = new Order();
            foreach (var item in keys)
            {
                order.Add(item.Name, OrderDirection.Desc);
            }

            Model2Sql sqlObject = GetQuerySQL<T>(query);
            sqlObject.Sql = String.Format("SELECT {0} FROM {1} WHERE {2} ORDER BY {3}", CreateQuery<T>(), ReflectionHelper.GetTypeName<T>(), sqlObject.Sql,GetOrderSQL<T>(order).Sql);
            return sqlObject;
        }


        /// <summary>
        /// 将IQuery查询条件转化为sql语句
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="query"></param>
        /// <returns></returns>
        public Model2Sql Model2SQL<T>(Model2Db model, IQuery query)
        {
            if (model != Model2Db.Count)
                throw new ArgumentException("不支持此类型");

            CheckKey<T>();
            List<PropertyInfo> keys = ReflectionHelper.GetKeys<T>();
            IOrder order = new Order();
            foreach (var item in keys)
            {
                order.Add(item.Name, OrderDirection.Desc);
            }
            Model2Sql sqlObject = GetQuerySQL<T>(query);
            sqlObject.Sql = String.Format("SELECT COUNT(*) FROM {0} WHERE {1} ORDER BY {2}",  ReflectionHelper.GetTypeName<T>(),sqlObject.Sql , GetOrderSQL<T>(order).Sql);
            return sqlObject;
        }

        private Model2Sql GetQuerySQL<T>(IQuery query)
        {
            if (query == null)
            {
                throw new ArgumentException("query不能为空");
            }
            
            Model2Sql sqlObject = new Model2Sql();
            sqlObject.SqlType = Model2Db.Query;

            Dictionary<IQueryValue, QueryLink> dic = query.All();
            if (dic.Count == 0)
            {
                throw new ArgumentException("query不包含查询字段");
            }
            int cur = 1;
            foreach (var item in dic)
            {
                if (cur != 1)
                {
                    sqlObject.Sql = sqlObject.Sql + string.Format(" {0} ", item.Value.ToString());
                }
                if (item.Key is QueryValue)
                {
                    QueryValue2Model2Sql<T>(sqlObject, item);
                }
                else if (item.Key is Query)
                {
                    Query2Model2Sql<T>(sqlObject, item);
                }
                else
                {
                    throw new ArgumentException("未实现指定的IQuery类型");
                }
                cur++;
            }

            return sqlObject;
        }

        private void Query2Model2Sql<T>(Model2Sql sqlObject, KeyValuePair<IQueryValue, QueryLink> item)
        {
            if (item.Key is Query)
            {
                sqlObject.Sql = sqlObject.Sql + " (";
                Query query = item.Key as Query;
                Dictionary<IQueryValue, QueryLink> dic = query.All();
                int cur = 1;
                foreach (var dicItem in dic)
                {
                    if (cur != 1)
                    {
                        sqlObject.Sql = sqlObject.Sql + string.Format(" {0} ", item.Value.ToString());
                    }
                    if (dicItem.Key is QueryValue)
                    {
                        QueryValue2Model2Sql<T>(sqlObject, dicItem);
                    }
                    else if (dicItem.Key is Query)
                    {
                        sqlObject.Sql = sqlObject.Sql + string.Format(" {0} ", item.Value.ToString()) + " (";
                        Model2SQL<T>(dicItem.Key as IQuery, sqlObject);
                        sqlObject.Sql = sqlObject.Sql + " )";
                    }
                    else
                    {
                        throw new ArgumentException("未实现指定的IQuery类型");
                    }
                    cur++;
                }
                sqlObject.Sql = sqlObject.Sql + " )";
            }
            else
            {
                throw new ArgumentException("此方法只用来解析Query");
            }
        }

        private void Model2SQL<T>(IQuery query, Model2Sql sqlObject)
        {
            if (query == null)
            {

            }
            else
            {
                sqlObject.SqlType = Model2Db.Query;

                Dictionary<IQueryValue, QueryLink> dic = query.All();
                int cur = 1;
                foreach (var item in dic)
                {
                    if (cur != 1)
                    {
                        sqlObject.Sql = sqlObject.Sql + string.Format(" {0} ", item.Value.ToString());
                    }
                    if (item.Key is QueryValue)
                    {
                        QueryValue2Model2Sql<T>(sqlObject, item);
                    }
                    else if (item.Key is Query)
                    {
                        Query2Model2Sql<T>(sqlObject, item);
                    }
                    else
                    {
                        throw new ArgumentException("未实现指定的IQuery类型");
                    }
                    cur++;
                }
            }
        }

        private void QueryValue2Model2Sql<T>(Model2Sql sqlObject, KeyValuePair<IQueryValue, QueryLink> item)
        {
            if (item.Key is QueryValue)
            {
                QueryValue value = item.Key as QueryValue;
                if (value != null)
                {
                    //检查字段是否存在于T中，如果不存在，提示异常
                    if (!ReflectionHelper.ExistField<T>(value.FieldName))
                    {
                        throw new ArgumentException(string.Format("类{0}中不包括此字段{1}", typeof(T).FullName, value.FieldName));
                    }
                    string fieldName = "@" + value.FieldName;
                    //检查同名参数是否已添加到字典(sqlObject.Param)中
                    if (sqlObject.Param.ContainsKey(fieldName))
                    {
                        KeyValuePair<string, object> param = sqlObject.Param.Last(p => p.Key.Contains(fieldName));
                        fieldName = param.Key + "1";
                    }

                    switch (value.Type)
                    {
                        case QueryType.Equal:
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0}={1} ", value.FieldName, fieldName);
                            sqlObject.Param.Add(fieldName, value.FieldValue);
                            break;
                        case QueryType.NotEqual:
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0}!={1} ", value.FieldName, fieldName);
                            sqlObject.Param.Add(fieldName, value.FieldValue);
                            break;
                        case QueryType.GreaterThan:
                            //日期格式要不要特殊处理
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0}>{1} ", value.FieldName, fieldName);
                            sqlObject.Param.Add(fieldName, value.FieldValue);
                            break;
                        case QueryType.LessThan:
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0}<{1} ", value.FieldName, fieldName);
                            sqlObject.Param.Add(fieldName, value.FieldValue);
                            break;
                        case QueryType.GreaterThenEqual:
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0}>={1} ", value.FieldName, fieldName);
                            sqlObject.Param.Add(fieldName, value.FieldValue);
                            break;
                        case QueryType.LessThanEqual:
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0}<={1} ", value.FieldName, fieldName);
                            sqlObject.Param.Add(fieldName, value.FieldValue);
                            break;
                        case QueryType.In:
                            //要不要根据数据类型做特殊处理
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0} in ( ", value.FieldName);
                            if (value.FieldValue.GetType().IsArray)
                            {
                                string[] valueArray = value.FieldValue as string[];

                                foreach (var valueItem in valueArray)
                                {
                                    if (sqlObject.Param.ContainsKey(fieldName))
                                    {
                                        KeyValuePair<string, object> param = sqlObject.Param.Last(p => p.Key.Contains(fieldName));
                                        if (param.Key.IndexOf(fieldName) > 0)
                                        {
                                            fieldName = fieldName + (Convert.ToInt32(param.Key.Substring(param.Key.IndexOf(fieldName))) + 1);
                                        }
                                        else
                                        {
                                            fieldName = fieldName + "1";
                                        }
                                    }

                                    sqlObject.Sql += fieldName + ",";
                                    sqlObject.Param.Add(fieldName, valueItem);
                                }
                                sqlObject.Sql = sqlObject.Sql.Remove(sqlObject.Sql.Length - 1) + ") ";
                            }
                            else
                            {
                                throw new ArgumentException(string.Format(" {0}的数据格式不是数组", value.FieldName));
                            }
                            break;
                        case QueryType.NotIn:
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0} not in ( ", value.FieldName);
                            if (value.FieldValue.GetType().IsArray)
                            {
                                string[] valueArray = value.FieldValue as string[];

                                foreach (var valueItem in valueArray)
                                {
                                    KeyValuePair<string, object> param = sqlObject.Param.Last(p => p.Key.Contains(fieldName));
                                    if (param.Key.IndexOf(fieldName) > 0)
                                    {
                                        fieldName = fieldName + (Convert.ToInt32(param.Key.Substring(param.Key.IndexOf(fieldName))) + 1);
                                    }
                                    else
                                    {
                                        fieldName = fieldName + "1";
                                    }

                                    sqlObject.Sql += fieldName + ",";
                                    sqlObject.Param.Add(fieldName, valueItem);
                                }
                                sqlObject.Sql = sqlObject.Sql.Remove(sqlObject.Sql.Length - 1) + ") ";
                            }
                            else
                            {
                                throw new ArgumentException(string.Format(" {0}的数据格式不是数组", value.FieldName));
                            }
                            break;
                        case QueryType.LikeLeft:
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0} like {1} ", value.FieldName, fieldName);
                            sqlObject.Param.Add(fieldName, "%" + value.FieldValue.ToString());
                            break;
                        case QueryType.LikeRight:
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0} like {1} ", value.FieldName, fieldName);
                            sqlObject.Param.Add(fieldName, value.FieldValue.ToString() + "%");
                            break;
                        case QueryType.LikeFull:
                            sqlObject.Sql = sqlObject.Sql + string.Format(" {0} like {1} ", value.FieldName, fieldName);
                            sqlObject.Param.Add(fieldName, "%" + value.FieldValue.ToString() + "%");
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {
                throw new ArgumentException("此方法只用来解析QueryValue");
            }
        }

        public Model2Sql Model2SQL<T>(IOrder order)
        {
            if (order is Order)
            {
                Model2Sql model = GetOrderSQL<T>(order);

                model.Sql = string.Format("SELECT {0} FROM {1} ORDER BY {2}", CreateQuery<T>(), ReflectionHelper.GetTypeName<T>(), model.Sql);

                return model;
            }
            else
            {
                throw new ArgumentException("不支持此类型" + order.GetType().FullName);
            }
        }

        private static Model2Sql GetOrderSQL<T>(IOrder order)
        {
            if (order == null)
            {
                throw new ArgumentException("order不能为空");
            }

            if (order.All().Count <= 0)
            {
                throw new ArgumentException("order中没有可排序字段");
            }

            int n = 1;
            string orderSql = string.Empty;
            foreach (var item in order.All())
            {
                if (!ReflectionHelper.ExistField<T>(item.Key))
                {
                    throw new ArgumentException(string.Format("此字段{0}不存在类中", item.Key));
                }

                if (n != 1)
                {
                    orderSql = orderSql+ " , " ;
                }
                orderSql += " " + item.Key + " " + item.Value.ToString();
                n++;
            }

            Model2Sql model = new Model2Sql() { Sql = orderSql, SqlType = Model2Db.Query };
            return model;
        }

        public Model2Sql Model2SQL<T>(IQuery query, IOrder order)
        {
            Model2Sql querySql = GetQuerySQL<T>(query);
            Model2Sql orderSql = GetOrderSQL<T>(order);

            Model2Sql model = new Model2Sql() {  SqlType= Model2Db.Query};
            foreach (var item in querySql.Param)
            {
                model.Param.Add(item.Key, item.Value);
            }

            
            model.Sql = string.Format("SELECT {0} FROM {1} WHERE {2} ORDER BY {3}", CreateQuery<T>(), ReflectionHelper.GetTypeName<T>(),querySql.Sql,orderSql.Sql);
            return model;
        }

        public Model2Sql Model2SQL<T>(IPager<T> pager)
        {
            if (pager == null)
            {
                throw new ArgumentException("pager不能为空");
            }
            CheckKey<T>();
            List<PropertyInfo> keys = ReflectionHelper.GetKeys<T>();
            IOrder order = new Order();
            foreach (var item in keys)
            {
                order.Add(item.Name, OrderDirection.Desc);
            }

            Model2Sql model = new Model2Sql() { SqlType= Model2Db.Query };
            model.Sql = string.Format("SELECT {0} FROM {1} ORDER BY {2} LIMIT {3},{4}", CreateQuery<T>(), ReflectionHelper.GetTypeName<T>(),GetOrderSQL<T>(order).Sql,(pager.CurrentPage-1)*pager.PageNum,pager.PageNum);

            return model;
        }

        public Model2Sql Model2SQL<T>(IPager<T> pager,IOrder order)
        {
            if (pager == null)
            {
                throw new ArgumentException("pager不能为空");
            }
            if (order == null)
            {
                throw new ArgumentException("order不能为空");
            }
            CheckKey<T>();
            Model2Sql model = new Model2Sql() { SqlType = Model2Db.Query };
            model.Sql = string.Format("SELECT {0} FROM {1} ORDER BY {2} LIMIT {3},{4}", CreateQuery<T>(), ReflectionHelper.GetTypeName<T>(), GetOrderSQL<T>(order).Sql, (pager.CurrentPage - 1) * pager.PageNum, pager.PageNum);

            return model;
        }

        public Model2Sql Model2SQL<T>(IPager<T> pager,IQuery query)
        {
            if (pager == null)
            {
                throw new ArgumentException("pager不能为空");
            }
            CheckKey<T>();
            List<PropertyInfo> keys = ReflectionHelper.GetKeys<T>();
            IOrder order = new Order();
            foreach (var item in keys)
            {
                order.Add(item.Name, OrderDirection.Desc);
            }

            Model2Sql model = GetQuerySQL<T>(query);

            model.Sql = string.Format("SELECT {0} FROM {1} WHERE {2} ORDER BY {3} LIMIT {4},{5}", CreateQuery<T>(), ReflectionHelper.GetTypeName<T>(),model.Sql, GetOrderSQL<T>(order).Sql, (pager.CurrentPage - 1) * pager.PageNum, pager.PageNum);

            return model;
        }

        public Model2Sql Model2SQL<T>(IPager<T> pager, IQuery query,IOrder order)
        {
            if (pager == null)
            {
                throw new ArgumentException("pager不能为空");
            }
            if (order == null)
            {
                throw new ArgumentException("order不能为空");
            }
            CheckKey<T>();
            Model2Sql model = GetQuerySQL<T>(query);

            model.Sql = string.Format("SELECT {0} FROM {1} WHERE {2} ORDER BY {3} LIMIT {4},{5}", CreateQuery<T>(), ReflectionHelper.GetTypeName<T>(), model.Sql, GetOrderSQL<T>(order).Sql, (pager.CurrentPage - 1) * pager.PageNum, pager.PageNum);

            return model;
        }


        private static void PropertyAdd2Param<T>(T model, Model2Sql sqlObject, List<PropertyInfo> keys)
        {
            keys.ToList().ForEach((p) => { sqlObject.Param.Add("@" + p.Name, p.GetValue(model)); });
        }

        private static void PropertyAdd2Param(object model, Model2Sql sqlObject, List<PropertyInfo> keys)
        {
            keys.ToList().ForEach((p) => { sqlObject.Param.Add("@" + p.Name, p.GetValue(model)); });
        }

        private static string CreatWhere(List<PropertyInfo> keys)
        {
            string sql = "";
            foreach (var item in keys)
            {
                sql = sql + string.Format(" {0}=@{0} AND", item.Name);
            }
            sql = sql.Remove(sql.Length - 3);
            return sql;
        }

        private static string CreateUpdate(List<PropertyInfo> nonKeys)
        {
            string sql = " SET ";
            foreach (var item in nonKeys)
            {
                sql = sql + string.Format(" {0}=@{0},", item.Name);
            }
            sql = sql.Remove(sql.Length - 1);
            return sql;
        }

        public static string CreateQuery<T>()
        {
            return string.Join(",", ReflectionHelper.GetPropertys<T>().Select(p => p.Name).ToArray());
        }
    }

    public class ReflectionHelper
    {
        public static PropertyInfo[] GetPropertys<T>()
        {
            Type type = typeof(T);
            return type.GetProperties().OrderBy(p => p.Name).ToArray();
        }

        /// <summary>
        /// 查找主键(未处理Key是具体类的异常)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<PropertyInfo> GetKeys<T>()
        {
            Type type = typeof(T);

            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            foreach (var item in type.GetProperties())
            {
                if (Attribute.IsDefined(item, typeof(KeyAttribute)))
                {
                    propertyInfos.Add(item);
                }
            }

            return propertyInfos;
        }

        /// <summary>
        /// 查找非主键
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static List<PropertyInfo> GetNonKey<T>()
        {
            Type type = typeof(T);

            List<PropertyInfo> propertyInfos = new List<PropertyInfo>();
            foreach (var item in type.GetProperties())
            {
                if (!Attribute.IsDefined(item, typeof(KeyAttribute)))
                {
                    propertyInfos.Add(item);
                }
            }

            return propertyInfos;
        }

        public static string GetTypeName<T>()
        {
            Type type = typeof(T);
            return type.Name;
        }

        public static List<PropertyInfo> GetPropertys(object model)
        {
            if (model == null)
            {
                throw new ArgumentException("model不能为null");
            }
            Type type = model.GetType();
            return type.GetProperties().ToList();
        }

        public static List<PropertyInfo> GetPropertysExceptBy(List<PropertyInfo> propertys, List<PropertyInfo> keyPropertys)
        {
            if (propertys == null)
            {
                throw new ArgumentException("propertys不能为null");
            }
            if (keyPropertys == null)
            {
                throw new ArgumentException("keyPropertys不能为null");
            }

            List<PropertyInfo> list = new List<PropertyInfo>();
            foreach (var item in propertys)
            {
                keyPropertys.ForEach(
                    (p) =>
                    {
                        if (p.Name.ToString() == item.Name.ToString())
                        {
                            list.Add(item);
                        }
                    }
                    );
            }

            List<PropertyInfo> listProperty = new List<PropertyInfo>();
            foreach (var item in propertys)
            {
                if (list.Contains(item))
                {
                    continue;
                }
                listProperty.Add(item);
            }

            return listProperty;
        }

        public static List<PropertyInfo> GetObjectKeys(List<PropertyInfo> keys, object model)
        {
            Type type = model.GetType();
            List<PropertyInfo> objectKeys = new List<PropertyInfo>();

            foreach (var item in type.GetProperties())
            {
                keys.ForEach(
                    (p) =>
                    {
                        if (item.Name.ToString() == p.Name.ToString())
                        {
                            objectKeys.Add(item);
                        }
                    }
                    );
            }

            return objectKeys;
        }

        public static bool ExistField<T>(string fieldName)
        {
            Type type = typeof(T);
            PropertyInfo property = type.GetProperty(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            return property != null;
        }
    }
}
