using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Hk.Core.Data.DbContextCore;
using SqlSugar;

namespace Hk.Core.Data.Repositories
{
    public class BaseRepositoryT<T>:IRepositoryT<T> where T: class ,new ()
    {
        private readonly IDbContextCore DbContext;

        public BaseRepositoryT(IDbContextCore context)
        {
            DbContext = context;
        }
        #region 新增
        public bool Insert(T insertObj)
        {
            return DbContext.Insert(insertObj);
        }

        public int InsertReturnIdentity(T insertObj)
        {
            return DbContext.InsertReturnIdentity(insertObj);
        }

        public bool InsertRange(List<T> insertObjs)
        {
            return DbContext.InsertRange(insertObjs);
        }


        #endregion

        #region 删除
        public bool Delete(T deleteObj)
        {
            return DbContext.Delete(deleteObj);
        }

        public bool Delete(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Delete(whereExpression);
        }



        #endregion

        #region 更新
        public bool Update(T updateObj)
        {
            return DbContext.Update(updateObj);
        }

        public bool UpdateRange(List<T> updateObjs)
        {
            return DbContext.UpdateRange(updateObjs);
        }

        public bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.Update(columns, whereExpression);
        }
        #endregion

        #region 查询

        public List<T> GetList()
        {
            return DbContext.GetList<T>();
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public List<T> GetList(Expression<Func<T, bool>> whereExpression = null) 
        {
            return DbContext.GetList(whereExpression);
        }

        public List<T> GetPageList(PageModel page, Expression<Func<T, bool>> whereExpression = null,
            Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
        {
            return DbContext.GetPageList<T>(page, whereExpression, orderByExpression, orderByType);
        }

        public T GetSingleOrDefault(Expression<Func<T, bool>> whereExpression)
        {
            return DbContext.GetSingleOrDefault(whereExpression);
        }

        #endregion

        #region DataTable
        public DataTable GetDataTable(string sql, object parameters = null)
        {
            return DbContext.GetDataTable(sql, parameters);
        }

        public List<T> GetListBySql(string sql, object parameters = null)
        {
            return DbContext.GetListBySql<T>(sql, parameters);
        }

        public T GetSingleBySql(string sql, object parameters = null) 
        {
            return DbContext.GetSingleBySql<T>(sql, parameters);
        }

        public bool ExecuteSqlTran(List<string> sqlStringList)
        {
            return DbContext.ExecuteSqlTran(sqlStringList);
        }

        public bool ExcuteSqlSugarTran(List<string> sqlStringList)
        {
            return DbContext.ExcuteSqlSugarTran(sqlStringList);
        }

        #endregion
    }
}