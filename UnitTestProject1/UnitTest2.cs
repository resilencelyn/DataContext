using Microsoft.VisualStudio.TestTools.UnitTesting;
using MySqlOrm;
using System;
using System.Collections.Generic;

namespace UnitTestProject1
{
    [TestClass]
    public class DataContextTest2
    {
        [TestMethod()]
        public void TestAdd()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            long n = context.Add<DbTable>(new DbTable() { Name = "add", Value = "add" });
            Assert.AreNotEqual(0, n);
        }

        [TestMethod()]
        public void TestDelete()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            int n = context.Delete<DbTable>(new{ ID=5 });
            Assert.AreEqual(1, n);
        }

        [TestMethod()]
        public void TestUpdate()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            int n = context.Update<DbTable>(new DbTable() { ID = 3,Name="Update",Value="Update2" });
            Assert.AreEqual(1, n);
        }

        [TestMethod()]
        public void TestPartUpdate()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            int n = context.PartUpdate<DbTable>(new { ID = 2, Name = "PartUpdate3" });
            Assert.AreEqual(1, n);
        }

        [TestMethod()]
        public void TestGet()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            DbTable n = context.Get<DbTable>(new { ID = 2});
            Assert.AreNotEqual(null, n);
        }


        [TestMethod()]
        public void TestGetAll()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            List<DbTable> n = context.GetAll<DbTable>();
            Assert.AreNotEqual(0, n.Count);
        }

        [TestMethod()]
        public void TestGetAllByQuery()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");

            QueryValue value1 = new QueryValue() { FieldName = "Name", FieldValue =new string[] { "add" , "Update" }, Type = QueryType.In };
            QueryValue value2 = new QueryValue() { FieldName = "Value", FieldValue = "2", Type = QueryType.LikeFull };

            Query query = new Query();
            query.Add(value1).Or(value2);
            //.Or(value2);

            //为了测试多个条件
            //QueryValue value3 = new QueryValue() { FieldName = "Name", FieldValue = 1, Type = QueryType.Equal };
            //QueryValue value4 = new QueryValue() { FieldName = "Value", FieldValue = 1, Type = QueryType.Equal };
            //Query query2 = new Query();
            //query2.Add(value3).Or(value4);

            //Query query3 = new Query();
            //query3.Add(value3).And(value4);

            //query.Add(query2).Add(query3);

            List<DbTable> n = context.GetAllByQuery<DbTable>(query);
            Assert.AreNotEqual(0, n.Count);
        }

        [TestMethod()]
        public void TestGetAllByOrder()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            Order order = new Order();
            order.Add("ID", OrderDirection.Desc);
            order.Add("Name", OrderDirection.Asc);
            List<DbTable> n = context.GetAll<DbTable>(order);
            Assert.AreNotEqual(0, n.Count);
        }

        [TestMethod()]
        public void TestGetAllByQueryOrder()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            Order order = new Order();
            order.Add("ID", OrderDirection.Desc);
            order.Add("Name", OrderDirection.Asc);

            QueryValue value1 = new QueryValue() { FieldName = "Name", FieldValue = new string[] { "add", "Update" }, Type = QueryType.In };
            QueryValue value2 = new QueryValue() { FieldName = "Value", FieldValue = "2", Type = QueryType.LikeFull };
            Query query = new Query();
            query.Add(value1).Or(value2);

            List<DbTable> n = context.GetAllByQuery<DbTable>(query, order);
            Assert.AreNotEqual(0, n.Count);
        }

        [TestMethod()]
        public void TestGetPages()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            IPager<DbTable> pager = context.GetPages<DbTable>(1, 1);

            Assert.AreNotEqual(0, pager.Total);
            Assert.AreEqual(1, pager.CurrentPage);
            Assert.AreEqual(1, pager.PageNum);
            Assert.AreEqual(3, pager.TotalPage);
        }

        [TestMethod()]
        public void TestGetPagesByOrder()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            IOrder order = new Order();
            order.Add("ID", OrderDirection.Desc);

            IPager<DbTable> pager = context.GetPages<DbTable>(1, 1,order);

            Assert.AreNotEqual(0, pager.Total);
            Assert.AreEqual(1, pager.CurrentPage);
            Assert.AreEqual(1, pager.PageNum);
            Assert.AreEqual(3, pager.TotalPage);
        }

        [TestMethod()]
        public void TestGetPagesByQuery()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            QueryValue value1 = new QueryValue() { FieldName = "Name", FieldValue = new string[] { "add", "Update" }, Type = QueryType.In };
            QueryValue value2 = new QueryValue() { FieldName = "Value", FieldValue = "2", Type = QueryType.LikeFull };

            Query query = new Query();
            query.Add(value1).Or(value2);

            IPager<DbTable> pager = context.GetPagesByQuery<DbTable>(1, 1, query);

            Assert.AreNotEqual(0, pager.Total);
            Assert.AreEqual(1, pager.CurrentPage);
            Assert.AreEqual(1, pager.PageNum);
            Assert.AreEqual(3, pager.TotalPage);
        }

        [TestMethod()]
        public void TestGetPagesByQueryOrder()
        {
            DataContext context = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
            QueryValue value1 = new QueryValue() { FieldName = "name", FieldValue = new string[] { "add", "Update" }, Type = QueryType.In };
            QueryValue value2 = new QueryValue() { FieldName = "value", FieldValue = "2", Type = QueryType.LikeFull };

            Query query = new Query();
            query.Add(value1).Or(value2);

            IOrder order = new Order();
            order.Add("name", OrderDirection.Desc);

            IPager<DbTable> pager = context.GetPagesByQuery<DbTable>(1, 1, query,order);

            Assert.AreNotEqual(0, pager.Total);
            Assert.AreEqual(1, pager.CurrentPage);
            Assert.AreEqual(1, pager.PageNum);
            Assert.AreEqual(3, pager.TotalPage);
        }

        [TestMethod()]
        public void TestAddGetAllByBussinessContext()
        {
            using (BussinessContext context = new BussinessContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;"))
            {
                try
                {
                    DataContext dataContext = new DataContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;");
                    dataContext.Add<DbTable>(context, new DbTable() { Name = "add20170903", Value = "add20170903" });
                    List<DbTable> n = dataContext.GetAll<DbTable>(context);
                    Assert.AreEqual(4, n.Count);
                    context.Commit();
                }
                catch (Exception)
                {

                    context.Rollback();
                }
            }
        }

        [TestMethod()]
        public void TestAddGetAllByBussinessContext2()
        {
            using (BussinessContext context = new BussinessContext("Database=test;Data Source=127.0.0.1;User Id=root;Password=1;pooling=true;CharSet=utf8;"))
            {
                try
                {
                    DataContext dataContext = new DataContext(context);
                    dataContext.Add<DbTable>(new DbTable() { Name = "add20170903", Value = "add20170903" });
                    List<DbTable> n = dataContext.GetAll<DbTable>();
                    Assert.AreEqual(6, n.Count);
                    context.Commit();
                }
                catch (Exception)
                {

                    context.Rollback();
                }
            }
        }

    }
}
