using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using SqlSugar;

namespace Hk.Core.Data.Repositories
{
    public interface IRepositoryT<T> where T:class,new ()
    {
        #region 新增

        bool Insert(T insertObj);
        int InsertReturnIdentity(T insertObj);
        bool InsertRange(List<T> insertObjs);
        #endregion
        #region 删除

        bool Delete(T deleteObj);
        bool Delete(Expression<Func<T, bool>> whereExpression);
        #endregion
        #region 更新

        bool Update(T updateObj);
        bool UpdateRange(List<T> updateObjs);
        bool Update(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression);

        #endregion
        #region 查询

        List<T> GetList();
        List<T> GetList(Expression<Func<T, bool>> whereExpression = null);
        List<T> GetPageList(PageModel page, Expression<Func<T, bool>> whereExpression = null,
            Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
            ;
        T GetSingleOrDefault(Expression<Func<T, bool>> whereExpression);
        #endregion

        #region DataTable

        DataTable GetDataTable(string sql, object parameters = null);
        List<T> GetListBySql(string sql, object parameters = null);
        T GetSingleBySql(string sql, object parameters = null);
        bool ExecuteSqlTran(List<string> sqlStringList);
        bool ExcuteSqlSugarTran(List<string> sqlStringList);

        #endregion
    }
}