using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlOrm
{
    /// <summary>
    /// 排序条件
    /// </summary>
    public interface IOrder
    {
        Dictionary<string,OrderDirection> Add(string field, OrderDirection direction);

        Dictionary<string, OrderDirection> Remove(string field);

        void Clear();

        Dictionary<string, OrderDirection> All();
    }


    public enum OrderDirection
    {
        Asc=0,
        Desc=1
    }


    public class Order : IOrder
    {
        private Dictionary<string, OrderDirection> fieldList = new Dictionary<string, OrderDirection>();

        public Dictionary<string, OrderDirection> Add(string field, OrderDirection direction)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new ArgumentException("字段不能为空");
            }

            fieldList.Add(field, direction);
            return fieldList;
        }

        public Dictionary<string, OrderDirection> Remove(string field)
        {
            if (string.IsNullOrEmpty(field))
            {
                throw new ArgumentException("字段不能为空");
            }

            fieldList.Remove(field);
            return fieldList;
        }

        public void Clear()
        {
            fieldList.Clear();
        }

        public Dictionary<string, OrderDirection> All()
        {
            return fieldList;
        }
    }

}
