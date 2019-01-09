using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Hk.Core.Data.DbContextCore;
using SqlSugar;

namespace Hk.Core.Data.Repositories
{
    public class BaseRepository:IRepository
    {
        protected readonly IDbContextCore DbContext;

        public BaseRepository(IDbContextCore context)
        {
            DbContext = context;
        }

        #region 新增
        public bool Insert<T>(T insertObj) where T : class, new()
        {
            return DbContext.Insert(insertObj);
        }

        public int InsertReturnIdentity<T>(T insertObj) where T : class, new()
        {
            return DbContext.InsertReturnIdentity(insertObj);
        }

        public bool InsertRange<T>(List<T> insertObjs) where T : class, new()
        {
            return DbContext.InsertRange(insertObjs);
        }


        #endregion

        #region 删除
        public bool Delete<T>(T deleteObj) where T : class, new()
        {
            return DbContext.Delete(deleteObj);
        }

        public bool Delete<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return DbContext.Delete(whereExpression);
        }



        #endregion

        #region 更新
        public bool Update<T>(T updateObj) where T : class, new()
        {
            return DbContext.Update(updateObj);
        }

        public bool UpdateRange<T>(List<T> updateObjs) where T : class, new()
        {
            return DbContext.UpdateRange(updateObjs);
        }

        public bool Update<T>(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return DbContext.Update(columns, whereExpression);
        }
        #endregion

        #region 查询

        public List<T> GetList<T>() where T : class, new()
        {
            return DbContext.GetList<T>();
        }

        // ReSharper disable once MethodOverloadWithOptionalParameter
        public List<T> GetList<T>(Expression<Func<T, bool>> whereExpression = null) where T : class, new()
        {
            return DbContext.GetList(whereExpression);
        }

        public List<T> GetPageList<T>(PageModel page, Expression<Func<T, bool>> whereExpression = null,
            Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc) where T : class, new()
        {
            return DbContext.GetPageList<T>(page, whereExpression, orderByExpression, orderByType);
        }

        public T GetSingleOrDefault<T>(Expression<Func<T, bool>> whereExpression) where T : class, new()
        {
            return DbContext.GetSingleOrDefault(whereExpression);
        }

        #endregion

        #region DataTable
        public DataTable GetDataTable(string sql, object parameters = null)
        {
            return DbContext.GetDataTable(sql, parameters);
        }

        public List<T> GetListBySql<T>(string sql, object parameters = null) where T : class, new()
        {
            return DbContext.GetListBySql<T>(sql, parameters);
        }

        public T GetSingleBySql<T>(string sql, object parameters = null) where T : class, new()
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