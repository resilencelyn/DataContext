using MySqlOrm;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using UnitTestProject1;

namespace MySqlOrm.Tests
{
    [TestClass()]
    public class UnitTest1
    {
        [TestMethod()]
        public void Model2SQLTest()
        {
            DBHelper db = new DBHelper();
            Model2Sql sqlObject = db.Model2SQL<DbTable>(Model2Db.Add, new DbTable() { ID = 0 });

            Assert.IsNotNull(sqlObject);
        }
    }
}

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            Type type = typeof(DbTable);

            //type.GetCustomAttribute<KeyAttribute>();

            foreach (var item in type.GetProperties())
            {
                bool attr = Attribute.IsDefined(item, typeof(KeyAttribute));
            }

            Type type2 = typeof(Int32);
            bool valueType= type2.IsValueType;
           

            Assert.IsNull(type);
        }
    }
}
