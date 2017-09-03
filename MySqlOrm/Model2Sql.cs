using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlOrm
{
    /// <summary>
    /// 模型转化为sql
    /// </summary>
    public class Model2Sql
    {
        public Model2Sql()
        {
            this.Param = new Dictionary<string, object>();
        }

        public string Sql { get; set; }

        public Dictionary<string, object> Param { get; set; }

        public Model2Db SqlType { get; set; }
    }

    /// <summary>
    /// 模型
    /// </summary>
    public enum Model2Db
    {
        Add = 0,
        Delete = 1,
        Update = 2,
        Query = 3,
        Count = 4
    }
}
