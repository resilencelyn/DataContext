using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlOrm
{
    /// <summary>
    /// 查询条件生成器
    /// </summary>
    public interface IQuery
    {
        IQuery Add(IQueryValue value);
        IQuery And(IQueryValue value);
        IQuery Or(IQueryValue value);

        void Clear();

        Dictionary<IQueryValue, QueryLink> All();
    }

    /// <summary>
    /// 查询条件连接
    /// </summary>
    public enum QueryLink
    {
        And=0,
        Or=1
    }

    /// <summary>
    /// 查询类型
    /// </summary>
    public enum QueryType
    {
        Equal=0,
        NotEqual = 1,
        GreaterThan=2,
        LessThan=3,
        GreaterThenEqual=4,
        LessThanEqual=5,
        In=6,
        NotIn=7,
        LikeLeft=8,
        LikeRight=9,
        LikeFull=10
    }

    /// <summary>
    /// 空接口，无任何方法
    /// </summary>
    public interface IQueryValue
    {

    }


    public class QueryValue: IQueryValue
    {
        public string FieldName { get; set; }

        public object FieldValue { get; set; }

        public QueryType Type { get; set; }
    }

    public class Query : IQueryValue, IQuery
    {
        Dictionary<IQueryValue, QueryLink> dic = new Dictionary<IQueryValue, QueryLink>();

        public IQuery Add(IQueryValue value)
        {
            if (this.Equals(value))
            {
                throw new ArgumentException("query不能添加自己到query中");
            }
            dic.Add(value, QueryLink.And);
            return this;
        }

        public Dictionary<IQueryValue, QueryLink> All()
        {
            return dic;
        }

        public IQuery And(IQueryValue value)
        {
            if (this.Equals(value))
            {
                throw new ArgumentException("query不能添加自己到query中");
            }
            dic.Add(value, QueryLink.And);

            return this;
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public IQuery Or(IQueryValue value)
        {
            if (this.Equals(value))
            {
                throw new ArgumentException("query不能添加自己到query中");
            }
            dic.Add(value, QueryLink.Or);
            return this;
        }
    }
}



