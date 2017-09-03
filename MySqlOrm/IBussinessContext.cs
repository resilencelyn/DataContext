using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlOrm
{
    /// <summary>
    /// 业务上下文
    /// </summary>
    public interface IBussinessContext : IDbTransaction, IDisposable
    {
    }

    public class BussinessContext : IBussinessContext
    {
        private MySqlConnection connection = null;

        private MySqlTransaction transaction = null;

        private BussinessContext()
        { }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionString">mysql连接字符串</param>
        public BussinessContext(string connectionString)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            transaction = connection.BeginTransaction();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="connectionString"></param>
        /// <param name="level"></param>
        public BussinessContext(string connectionString, IsolationLevel level)
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            transaction = connection.BeginTransaction(level);
        }

        public IDbConnection Connection => GetConnection();

        private IDbConnection GetConnection()
        {
            return connection;
        }

        public IsolationLevel IsolationLevel => GetIsolationLevel();

        private IsolationLevel GetIsolationLevel()
        {
            return transaction.IsolationLevel;
        }

        public void Commit()
        {
            transaction.Commit();
        }

        public void Dispose()
        {
            connection.Close();
            connection.Dispose();
        }

        public void Rollback()
        {
            transaction.Rollback();
        }
    }
}
