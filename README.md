# 简单数据库操作类库
已实现方法：

常用的增删改查，分页，排序

# 统一接口定义IDataContext
## 不带事务的方法接口如下

插入一条数据  long Add<T>(T value)

删除一条数据  int Delete<T>(object value)
        
修改一条记录  int Update<T>(T value)

部分更新一条记录  int PartUpdate<T>(object value)

获取一条记录 T Get<T>(object id)

获取所有记录（默认用主键降序排列）  List<T> GetAll<T>()

获取所有记录（指定排序方式） List<T> GetAll<T>(IOrder order)

获取所有记录（指定查询条件） List<T> GetAllByQuery<T>(IQuery query)

获取所有记录（指定查询条件和排序方式） List<T> GetAllByQuery<T>(IQuery query, IOrder order)

分页（默认使用主键降序排列） IPager<T> GetPages<T>(int pageInex, int pageSize)

分页（指定排序方式） IPager<T> GetPages<T>(int pageInex, int pageSize, IOrder order)

分页（指定排序方式） IPager<T> GetPagesByQuery<T>(int pageIndex, int pageSize, IQuery query)

分页（指定查询条件和排序方式） IPager<T> GetPagesByQuery<T>(int pageIndex, int pageSize, IQuery query, IOrder order)


## 带事务的方法
long Add<T>(IBussinessContext context, T value)

int Delete<T>(IBussinessContext context, object value)

int Update<T>(IBussinessContext context, T value)

int PartUpdate<T>(IBussinessContext context, object value)

List<T> GetAll<T>(IBussinessContext context)

List<T> GetAllByQuery<T>(IBussinessContext context, IQuery query)

T Get<T>(IBussinessContext context, object id)
