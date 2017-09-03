using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MySqlOrm
{
    public class DataContext : IDataContext,IDisposable
    {
        string connectionString = string.Empty;
        DBHelper dbHelper = new DBHelper();

        MySqlConnection connection = null;

        private DataContext() { }

        /// <summary>
        /// 连接字符串
        /// </summary>
        /// <param name="connectionString">Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;</param>
        public DataContext(string connectionString)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("数据库连接字符串不能为空");
            }

            connection = dbHelper.CreateConnection(connectionString);
            this.connectionString = connectionString;
        }

        public DataContext(IBussinessContext context)
        {
            connection = context.Connection as MySqlConnection;
            this.connectionString = connection.ConnectionString;
        }

        public long Add<T>(T value) where T : class, new()
        {
            long result = 0;
            CheckConnectionOpen();
            using (MySqlCommand command = dbHelper.CreateCommand(connection, dbHelper.Model2SQL<T>(Model2Db.Add, value)))
            {
                result = command.ExecuteNonQuery();
                result = command.LastInsertedId;
            }
            return result;
        }

        public long Add<T>(IBussinessContext context, T value) where T : class, new()
        {
            long result = 0;
            
            using (MySqlCommand command = dbHelper.CreateCommand(context.Connection as MySqlConnection, dbHelper.Model2SQL<T>(Model2Db.Add, value)))
            {
                result = command.ExecuteNonQuery();
                result = command.LastInsertedId;
            }
            return result;
        }

        public int Delete<T>(object value) where T : class, new()
        {
            int result = 0;
            CheckConnectionOpen();
            using (MySqlCommand command = dbHelper.CreateCommand(connection, dbHelper.Model2SQL<T>(Model2Db.Delete, value)))
            {
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        public int Delete<T>(IBussinessContext context, object value) where T : class, new()
        {
            int result = 0;
            using (MySqlCommand command = dbHelper.CreateCommand(context.Connection as MySqlConnection, dbHelper.Model2SQL<T>(Model2Db.Delete, value)))
            {
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        public int PartUpdate<T>(object value) where T : class, new()
        {
            int result = 0;
            CheckConnectionOpen();
            using (MySqlCommand command = dbHelper.CreateCommand(connection, dbHelper.Model2SQL<T>(Model2Db.Update, value)))
            {
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        public int PartUpdate<T>(IBussinessContext context, object value) where T : class, new()
        {
            int result = 0;
            using (MySqlCommand command = dbHelper.CreateCommand(context.Connection as MySqlConnection, dbHelper.Model2SQL<T>(Model2Db.Update, value)))
            {
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        public int Update<T>(T value) where T : class, new()
        {
            int result = 0;
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            using (MySqlCommand command = dbHelper.CreateCommand(connection, dbHelper.Model2SQL<T>(Model2Db.Update, value)))
            {
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        public int Update<T>(IBussinessContext context, T value) where T : class, new()
        {
            int result = 0;
            dbHelper.CheckKey<T>();
            using (MySqlCommand command = dbHelper.CreateCommand(context.Connection as MySqlConnection, dbHelper.Model2SQL<T>(Model2Db.Update, value)))
            {
                result = command.ExecuteNonQuery();
            }
            return result;
        }

        public T Get<T>(object id) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(connection, dbHelper.Model2SQL<T>(Model2Db.Query, id)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                if (models.Any())
                {
                    return models.First();
                }
            }
            return default(T);
        }

        public T Get<T>(IBussinessContext context, object id) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(context.Connection as MySqlConnection, dbHelper.Model2SQL<T>(Model2Db.Query, id)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                if (models.Any())
                {
                    return models.First();
                }
            }
            return default(T);
        }

        public List<T> GetAll<T>() where T : class, new()
        {
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(connection, dbHelper.Model2SQL<T>(Model2Db.Query,default(T))))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                return models.ToList();
            }
        }

        public List<T> GetAll<T>(IBussinessContext context) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(context.Connection as MySqlConnection, dbHelper.Model2SQL<T>(Model2Db.Query, default(T))))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                return models.ToList();
            }
        }

        public List<T> GetAllByQuery<T>(IQuery query) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(connection, dbHelper.Model2SQL<T>(query)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                return models.ToList();
            }
        }

        public List<T> GetAllByQuery<T>(IBussinessContext context, IQuery query) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(context.Connection as MySqlConnection, dbHelper.Model2SQL<T>(query)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                return models.ToList();
            }
        }

        public IPager<T> GetPages<T>(int pageInex, int pageSize) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            DataPager<T> pager = new DataPager<T>();

            using (MySqlCommand command = dbHelper.CreateCommand(connection, dbHelper.Model2SQL<T>(Model2Db.Count,default(T))))
            {
                pager.Total = Convert.ToInt32(command.ExecuteScalar());
                pager.TotalPage = pager.Total/pageSize;
                pager.PageNum = pageSize;
                if (pageInex <= 0)
                {
                    pager.CurrentPage = 1;
                }
                else if (pageInex >= pager.TotalPage)
                {
                    pager.CurrentPage = pager.TotalPage;
                }
                else
                {
                    pager.CurrentPage = pageInex;
                }
            }

            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(connection, dbHelper.Model2SQL<T>(pager)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                pager.Data = models.ToList();
            }

            return pager;
        }

        public IPager<T> GetPagesByQuery<T>(int pageIndex, int pageSize, IQuery query) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            DataPager<T> pager = new DataPager<T>();

            using (MySqlCommand command = dbHelper.CreateCommand(connection, dbHelper.Model2SQL<T>(Model2Db.Count,query)))
            {
                pager.Total = Convert.ToInt32(command.ExecuteScalar());
                pager.TotalPage = pager.Total / pageSize;
                pager.PageNum = pageSize;
                if (pageIndex <= 0)
                {
                    pager.CurrentPage = 1;
                }
                else if (pageIndex >= pager.TotalPage)
                {
                    pager.CurrentPage = pager.TotalPage;
                }
                else
                {
                    pager.CurrentPage = pageIndex;
                }
            }

            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(connection, dbHelper.Model2SQL<T>(pager,query)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                pager.Data = models.ToList();
            }

            return pager;
        }

        public List<T> GetAll<T>(IOrder order) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(connection, dbHelper.Model2SQL<T>(order)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                return models.ToList();
            }
        }

        public List<T> GetAllByQuery<T>(IQuery query, IOrder order) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(connection, dbHelper.Model2SQL<T>(query,order)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                return models.ToList();
            }
        }

        public IPager<T> GetPages<T>(int pageInex, int pageSize, IOrder order) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            DataPager<T> pager = new DataPager<T>();

            using (MySqlCommand command = dbHelper.CreateCommand(connection, dbHelper.Model2SQL<T>(Model2Db.Count, default(T))))
            {
                pager.Total = Convert.ToInt32(command.ExecuteScalar());
                pager.TotalPage = pager.Total / pageSize;
                pager.PageNum = pageSize;
                if (pageInex <= 0)
                {
                    pager.CurrentPage = 1;
                }
                else if (pageInex >= pager.TotalPage)
                {
                    pager.CurrentPage = pager.TotalPage;
                }
                else
                {
                    pager.CurrentPage = pageInex;
                }
            }

            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(connection, dbHelper.Model2SQL<T>(pager, order)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                pager.Data = models.ToList();
            }

            return pager;
        }

        public IPager<T> GetPagesByQuery<T>(int pageIndex, int pageSize, IQuery query, IOrder order) where T : class, new()
        {
            dbHelper.CheckKey<T>();
            CheckConnectionOpen();
            DataPager<T> pager = new DataPager<T>();

            using (MySqlCommand command = dbHelper.CreateCommand(connection, dbHelper.Model2SQL<T>(Model2Db.Count, query)))
            {
                pager.Total = Convert.ToInt32(command.ExecuteScalar());
                pager.TotalPage = pager.Total / pageSize;
                pager.PageNum = pageSize;
                if (pageIndex <= 0)
                {
                    pager.CurrentPage = 1;
                }
                else if (pageIndex >= pager.TotalPage)
                {
                    pager.CurrentPage = pager.TotalPage;
                }
                else
                {
                    pager.CurrentPage = pageIndex;
                }
            }

            using (MySqlDataAdapter adapter = dbHelper.CreateAdapter(connection, dbHelper.Model2SQL<T>(pager, query,order)))
            {
                DataTable table = new DataTable();
                adapter.Fill(table);
                IList<T> models = ModelConvertHelper<T>.ConvertToModel(table);
                pager.Data = models.ToList();
            }

            return pager;
        }


        #region 私有方法

        /// <summary>
        /// 检查数据库连接是否打开
        /// </summary>
        private void CheckConnectionOpen()
        {
            if (connection.State == ConnectionState.Closed)
            {
                connection.Open();
            }
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
            }
        }
        #endregion
    }
}
