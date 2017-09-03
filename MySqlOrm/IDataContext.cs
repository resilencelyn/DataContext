using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlOrm
{
    /// <summary>
    /// 数据上下文
    /// </summary>
    public interface IDataContext
    {
        #region CRUD方法（CUD）

        long Add<T>(T value) where T : class, new();
        int Delete<T>(object value) where T : class, new();
        int Update<T>(T value) where T : class, new();
        int PartUpdate<T>(object value) where T : class, new();

        long Add<T>(IBussinessContext context, T value) where T : class, new();
        int Delete<T>(IBussinessContext context, object value) where T : class, new();
        int Update<T>(IBussinessContext context, T value) where T : class, new();
        int PartUpdate<T>(IBussinessContext context, object value) where T : class, new();

        #endregion

        #region 查询方法

        List<T> GetAll<T>() where T : class, new();
        List<T> GetAll<T>(IOrder order) where T : class, new();
        List<T> GetAllByQuery<T>(IQuery query) where T : class, new();
        List<T> GetAllByQuery<T>(IQuery query, IOrder order) where T : class, new();

        List<T> GetAll<T>(IBussinessContext context) where T : class, new();
        List<T> GetAllByQuery<T>(IBussinessContext context, IQuery query) where T : class, new();

        IPager<T> GetPages<T>(int pageInex, int pageSize) where T : class, new();
        IPager<T> GetPages<T>(int pageInex, int pageSize, IOrder order) where T : class, new();
        IPager<T> GetPagesByQuery<T>(int pageIndex, int pageSize, IQuery query) where T : class, new();
        IPager<T> GetPagesByQuery<T>(int pageIndex, int pageSize, IQuery query, IOrder order) where T : class, new();

        T Get<T>(object id) where T : class, new();
        T Get<T>(IBussinessContext context, object id) where T : class, new();

        #endregion

    }
}
