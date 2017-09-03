using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MySqlOrm
{
    public class DataPager<T> : IPager<T>
    {
        public int Total { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPage { get ; set ; }
        public int PageNum { get; set; }
        public List<T> Data { get; set; }
    }
}
