using Hk.Core.Data.Options;
using Microsoft.Extensions.Options;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Hk.Core.Data.DbContextCore
{
    public class BaseDbContext : IDbContextCore
    {
        //注意：不能写成静态的
        public SqlSugarClient Db;//用来处理事务多表查询和复杂的操作
        /// <summary>
        /// 用来处理T表的常用操作
        /// </summary>
        public SimpleClient CurrentDb => new SimpleClient(Db);
        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="option"></param>
        public BaseDbContext(IOptions<DbContextOption> option)
        {

            if (option == null)
                throw new ArgumentNullException(nameof(option));
            if (string.IsNullOrEmpty(option.Value.ConnectionString))
                throw new ArgumentNullException(nameof(option.Value.ConnectionString));
        }

        #region 新增
        public bool Insert<T>(T insertObj) where T : class, new()
        {
            return CurrentDb.Insert(insertObj);
        }

        public int InsertReturnIdentity<T>(T insertObj) where T : class, new()
        {
            return CurrentDb.InsertReturnIdentity(insertObj);
        }

        public bool InsertRange<T>(List<T> insertObjs) where T : class, new()
        {
            return CurrentDb.InsertRange(insertObjs);
        }

        public bool DeleteById<T>(object id) where T : class, new()
        {
            return CurrentDb.DeleteById<T>(id);
        }

        #endregion

        #region 删除
        public bool Delete<T>(T deleteObj) where T : class, new()
        {
            return CurrentDb.Delete(deleteObj);
        }

        public bool Delete<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return CurrentDb.Delete(whereExpression);
        }

        public bool DeleteByIds<T>(object[] ids) where T : class, new()
        {
            return CurrentDb.DeleteByIds<T>(ids);
        }

        #endregion

        #region 更新
        public bool Update<T>(T updateObj) where T : class, new()
        {
            return CurrentDb.Update(updateObj);
        }

        public bool UpdateRange<T>(List<T> updateObjs) where T : class, new()
        {
            return CurrentDb.UpdateRange(updateObjs);
        }

        public bool Update<T>(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return CurrentDb.Update(columns, whereExpression);
        }

        public T GetById<T>(object id) where T : class, new()
        {
            return CurrentDb.GetById<T>(id);
        }

        #endregion

        #region 查询

        public List<T> GetList<T>() where T : class, new()
        {
            return CurrentDb.GetList<T>();
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public List<T> GetList<T>(Expression<Func<T, bool>> whereExpression = null) where T : class, new()
        {
            return CurrentDb.GetList(whereExpression);
        }

        public List<T> GetPageList<T>(PageModel page, Expression<Func<T, bool>> whereExpression = null,
            Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where T : class, new()
        {
            return CurrentDb.GetPageList(whereExpression, page, orderByExpression, orderByType);
        }

        public T GetSingleOrDefault<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return Db.Queryable<T>().Where(whereExpression).First();
        }

        //public T GetSingleOrDefault<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        //{
        //    return Db.Queryable<T>().First();
        //}
        #endregion

        #region DataTable
        public DataTable GetDataTable(string sql, object parameters = null)
        {
            return Db.Ado.GetDataTable(sql, parameters);
        }

        public List<T> GetListBySql<T>(string sql, object parameters = null) where T:class ,new ()
        {
            return Db.Ado.SqlQuery<T>(sql, parameters);
        }

        public T GetSingleBySql<T>(string sql, object parameters = null) where T : class, new()
        {
            return Db.Ado.SqlQuerySingle<T>(sql, parameters);
        }

        public virtual bool ExecuteSqlTran(List<string> sqlStringList)
        {
            throw  new  Exception("没有实现当前方法:ExecuteSqlTran");
        }

        public bool ExcuteSqlSugarTran(List<string> sqlStringList)
        {
            bool result = false;
            try
            {
                Db.Ado.BeginTran();
                sqlStringList.ForEach(x =>
                {
                    Db.Ado.ExecuteCommand(x);
                });
                Db.Ado.CommitTran();
                result = true;
            }
            catch (Exception ex)
            {
                Db.Ado.RollbackTran();
                result = false;
                throw new Exception(ex.Message);
            }

            return result;
        }

        #endregion


    }
}