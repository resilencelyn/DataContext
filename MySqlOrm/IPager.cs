using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlOrm
{
    /// <summary>
    /// 分页类
    /// </summary>
    public interface IPager<T>
    {
        /// <summary>
        /// 总行数
        /// </summary>
        int Total { set; get; }

        /// <summary>
        /// 当前页
        /// </summary>
        int CurrentPage { get; set; }

        /// <summary>
        /// 总页数
        /// </summary>
        int TotalPage { get; set; }

        /// <summary>
        /// 单页数据行数
        /// </summary>
        int PageNum { get; set; }

        /// <summary>
        /// 当前数据集
        /// </summary>
        List<T> Data { get; set; }
    }
}
