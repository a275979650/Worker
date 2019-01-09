using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Hk.Core.Data.DbContextCore
{
    /// <summary>
    /// 定义上下文
    /// </summary>
    public interface IDbContextCore
    {
        #region 新增

        bool Insert<T>(T insertObj) where T : class, new();
        int InsertReturnIdentity<T>(T insertObj) where T : class, new();
        bool InsertRange<T>(List<T> insertObjs) where T : class, new();
        #endregion
        #region 删除

        bool DeleteById<T>(object id) where T : class, new();
        bool Delete<T>(T deleteObj) where T : class, new();
        bool Delete<T>(Expression<Func<T, bool>> whereExpression) where T : class, new();
        bool DeleteByIds<T>(object[] ids) where T : class, new();
        #endregion
            #region 更新

        bool Update<T>(T updateObj) where T : class, new();
        bool UpdateRange<T>(List<T> updateObjs) where T : class, new();
        bool Update<T>(Expression<Func<T, T>> columns, Expression<Func<T, bool>> whereExpression)
            where T : class, new();

        #endregion
        #region 查询

        T GetById<T>(object id) where T : class, new();

       List<T> GetList<T>() where T : class, new();
        List<T> GetList<T>(Expression<Func<T, bool>> whereExpression = null) where T : class, new();
        List<T> GetPageList<T>(PageModel page, Expression<Func<T, bool>> whereExpression = null,
            Expression<Func<T, object>> orderByExpression = null, OrderByType orderByType = OrderByType.Asc)
            where T : class, new();
        T GetSingleOrDefault<T>(Expression<Func<T, bool>> whereExpression) where T : class, new();
        #endregion

        #region DataTable

        DataTable GetDataTable(string sql, object parameters = null);
        List<T> GetListBySql<T>(string sql, object parameters = null) where T : class, new();
        T GetSingleBySql<T>(string sql, object parameters = null) where T : class, new();
        bool ExecuteSqlTran(List<string> sqlStringList);
        bool ExcuteSqlSugarTran(List<string> sqlStringList);

        #endregion
    }
}
