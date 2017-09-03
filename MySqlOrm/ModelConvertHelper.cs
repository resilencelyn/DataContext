using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MySqlOrm
{
    /// <summary>  
    /// DataTable转化为实体类
    /// </summary>  
    public class ModelConvertHelper<T> where T :class, new()
    {
        public static IList<T> ConvertToModel(DataTable dt)
        {
            // 定义集合  
            IList<T> ts = new List<T>();

            // 获得此模型的类型  
            Type type = typeof(T);

            string tempName = "";

            foreach (DataRow dr in dt.Rows)
            {
                T t = new T();

                // 获得此模型的公共属性  
                PropertyInfo[] propertys = t.GetType().GetProperties();

                foreach (PropertyInfo pi in propertys)
                {
                    tempName = pi.Name;

                    // 检查DataTable是否包含此列  
                    if (dt.Columns.Contains(tempName))
                    {
                        Type tempType = pi.PropertyType;

                        // 判断此属性是否有Setter  
                        if (!pi.CanWrite) continue;

                        object value = dr[tempName];
                        if (value != DBNull.Value)
                        {
                            //判断是否是泛型
                            if (tempType.IsGenericType)
                            {
                                tempType = tempType.GetGenericArguments()[0];
                            }

                            try
                            {
                                value = Convert.ChangeType(value, tempType);//改变数据类型
                                pi.SetValue(t, value, null);
                            }
                            catch (InvalidCastException ex)
                            {
                                throw ex;
                            }
                        }
                        else
                        {
                            pi.SetValue(t, null, null);
                        }
                    }
                }

                ts.Add(t);
            }
            return ts;
        }
    }
}
