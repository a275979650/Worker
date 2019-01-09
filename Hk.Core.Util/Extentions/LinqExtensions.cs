using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json;

namespace Hk.Core.Util.Extentions
{
    /// <summary>
    /// Enables the efficient, dynamic composition of query predicates.
    /// </summary>
    public static class LinqExtensions
    {
        public static Expression Property(this Expression expression, string propertyName)
        {
            return Expression.Property(expression, propertyName);
        }
        public static Expression AndAlso(this Expression left, Expression right)
        {
            return Expression.AndAlso(left, right);
        }
        public static Expression Call(this Expression instance, string methodName, params Expression[] arguments)
        {
            return Expression.Call(instance, instance.Type.GetMethod(methodName,new []{ instance.Type}), arguments);
        }
        public static Expression GreaterThan(this Expression left, Expression right)
        {
            return Expression.GreaterThan(left, right);
        }
        public static Expression<T> ToLambda<T>(this Expression body, params ParameterExpression[] parameters)
        {
            return Expression.Lambda<T>(body, parameters);
        }
        public static Expression<Func<T, bool>> True<T>() { return param => true; }
        public static Expression<Func<T, bool>> False<T>() { return param => false; }

        /// <summary>
        /// 组合And
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, object>> AndLamda<T>(this Expression<Func<T, object>> first, Expression<Func<T, object>> second)
        {
            var invokedExpr = Expression.Invoke(second, first.Parameters.Cast<Expression>());
            return Expression.Lambda<Func<T, object>>
                (Expression.And(first.Body, invokedExpr), first.Parameters);
        }
        /// <summary>
        /// 组合Or
        /// </summary>
        /// <returns></returns>
        public static Expression<Func<T, object>> OrLamda<T>(this Expression<Func<T, object>> first, Expression<Func<T, object>> second)
        {
            return first.ComposeLamda(second, Expression.OrElse);
        }

        /// <summary>
        /// Combines the first expression with the second using the specified merge function.
        /// </summary>
        static Expression<T> ComposeLamda<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            var map = first.Parameters
                .Select((f, i) => new { f, s = second.Parameters[i] })
                .ToDictionary(p => p.s, p => p.f);
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
        public static Expression Or(this Expression expression, Expression expressionRight)
        {
            return Expression.Or(expression, expressionRight);
        }

        public static Expression And(this Expression expression, Expression expressionRight)
        {
            return Expression.And(expression, expressionRight);
        }
        /// <summary>
        /// 以 Expression.AndAlso 组合两个Expression表达式
        /// </summary>
        /// <typeparam name="T">表达式的主实体类型</typeparam>
        /// <param name="first">第一个Expression表达式</param>
        /// <param name="second">要组合的Expression表达式</param>
        /// <returns>组合后的表达式</returns>
        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        /// 以 Expression.OrElse 组合两个Expression表达式
        /// </summary>
        /// <typeparam name="T">表达式的主实体类型</typeparam>
        /// <param name="first">第一个Expression表达式</param>
        /// <param name="second">要组合的Expression表达式</param>
        /// <returns>组合后的表达式</returns>
        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }
        /// <summary>
        /// 以特定的条件运行组合两个Expression表达式
        /// </summary>
        /// <typeparam name="T">表达式的主实体类型</typeparam>
        /// <param name="first">第一个Expression表达式</param>
        /// <param name="second">要组合的Expression表达式</param>
        /// <param name="merge">组合条件运算方式</param>
        /// <returns>组合后的表达式</returns>
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            Dictionary<ParameterExpression, ParameterExpression> map =
                first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            Expression secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression 
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }
        public static IQueryable<T> GenerateFilter<T>(this IQueryable<T> query, string filterjson)
        {
            if (!string.IsNullOrEmpty(filterjson))
            {
                var filters = JsonConvert.DeserializeObject<IEnumerable<Filter>>(filterjson);
                var param = CreateLambdaParam<T>("c");

                Expression result = Expression.Constant(true);
                foreach (var filter in filters)
                {
                    result = result.AndAlso(param.GenerateBody<T>(filter));
                }

                query = query.Where(param.GenerateTypeLambda<T>(result));
            }

            return query;
        }

        /// <summary>
        /// 创建完整的lambda
        /// </summary>
        public static LambdaExpression GenerateLambda(this ParameterExpression param, Expression body)
        {
            //c=>c.XXX=="XXX"
            return Expression.Lambda(body, param);
        }

        public static Expression<Func<T, bool>> GenerateTypeLambda<T>(this ParameterExpression param, Expression body)
        {
            return (Expression<Func<T, bool>>)(param.GenerateLambda(body));
        }

        public static Expression<Func<T, bool>> GenerateTypeBody<T>(this ParameterExpression param, Filter filterObj)
        {
            return (Expression<Func<T, bool>>)(param.GenerateBody<T>(filterObj));
        }
        /// <summary>
        ///     取的  Expression<Func<T,TProperty>> predicate 表达式对应的属性名称
        ///         例如：c=>c.Value.Year 侧返回：Value.Year
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public static string GetPropertyName<T, TProperty>(this Expression<Func<T, TProperty>> predicate)
        {
            var expression = predicate.Body as MemberExpression;
            //return expression.Member.Name; //该属性只返回最后一个属性，因此采用下面方法返回。
            return expression.ToString().Substring(2);
        }

        public static ParameterExpression CreateLambdaParam<T>(string name)
        {
            return Expression.Parameter(typeof(T), name);
        }

        /// <summary>
        /// 创建linq表达示的body部分
        /// </summary>
        public static Expression GenerateBody<T>(this ParameterExpression param, Filter filterObj)
        {
            PropertyInfo property = typeof(T).GetProperty(filterObj.Key);

            //组装左边
            Expression left = Expression.Property(param, property);
            //组装右边
            Expression right = null;

            if (property.PropertyType == typeof(int))
            {
                right = Expression.Constant(int.Parse(filterObj.Value));
            }
            else if (property.PropertyType == typeof(DateTime))
            {
                right = Expression.Constant(DateTime.Parse(filterObj.Value));
            }
            else if (property.PropertyType == typeof(string))
            {
                right = Expression.Constant((filterObj.Value));
            }
            else if (property.PropertyType == typeof(decimal))
            {
                right = Expression.Constant(decimal.Parse(filterObj.Value));
            }
            else if (property.PropertyType == typeof(Guid))
            {
                right = Expression.Constant(Guid.Parse(filterObj.Value));
            }
            else if (property.PropertyType == typeof(bool))
            {
                right = Expression.Constant(filterObj.Value.Equals("1"));
            }
            else if (property.PropertyType == typeof(Guid?))
            {
                left = Expression.Property(left, "Value");
                right = Expression.Constant(Guid.Parse(filterObj.Value));
            }
            else
            {
                throw new Exception("暂不能解析该Key的类型");
            }

            //c.XXX=="XXX"
            Expression filter = Expression.Equal(left, right);
            switch (filterObj.Contrast)
            {
                case "<=":
                    filter = Expression.LessThanOrEqual(left, right);
                    break;

                case "<":
                    filter = Expression.LessThan(left, right);
                    break;

                case ">":
                    filter = Expression.GreaterThan(left, right);
                    break;

                case ">=":
                    filter = Expression.GreaterThanOrEqual(left, right);
                    break;
                case "!=":
                    filter = Expression.NotEqual(left, right);
                    break;

                case "like":
                    filter = Expression.Call(left, typeof(string).GetMethod("Contains", new Type[] { typeof(string) }),
                        Expression.Constant(filterObj.Value));
                    break;
                case "not in":
                    var listExpression = Expression.Constant(filterObj.Value.Split(',').ToList()); //数组
                    var method = typeof(List<string>).GetMethod("Contains", new Type[] { typeof(string) }); //Contains语句
                    filter = Expression.Not(Expression.Call(listExpression, method, left));
                    break;
                case "in":
                    var lExp = Expression.Constant(filterObj.Value.Split(',').ToList()); //数组
                    var methodInfo =
                        typeof(List<string>).GetMethod("Contains", new Type[] { typeof(string) }); //Contains语句
                    filter = Expression.Call(lExp, methodInfo, left);
                    break;
            }

            return filter;
        }
        //系统已经有该函数的实现
        //public static IQueryable<T> Where<T>(this IQueryable<T> query, Expression expression)
        //{
        //    Expression expr = Expression.Call(typeof(Queryable), "Where", new[] { typeof(T) },
        //       Expression.Constant(query), expression);
        //    //生成动态查询
        //    IQueryable<T> result = query.Provider.CreateQuery<T>(expr);
        //    return result;
        //}
        #region 拓展BuildExtendSelectExpre方法

        /// <summary>
        /// 组合继承属性选择表达式树,无拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, TResult>> BuildExtendSelectExpre<TBase, TResult>(this Expression<Func<TBase, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, TResult>>(expression);
        }

        /// <summary>
        /// 组合继承属性选择表达式树,1个拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="T1">拓展类型1</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, T1, TResult>> BuildExtendSelectExpre<TBase, T1, TResult>(this Expression<Func<TBase, T1, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, TResult>>(expression);
        }

        /// <summary>
        /// 组合继承属性选择表达式树,2个拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="T1">拓展类型1</typeparam>
        /// <typeparam name="T2">拓展类型2</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, T1, T2, TResult>> BuildExtendSelectExpre<TBase, T1, T2, TResult>(this Expression<Func<TBase, T1, T2, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, TResult>>(expression);
        }

        /// <summary>
        /// 组合继承属性选择表达式树,3个拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="T1">拓展类型1</typeparam>
        /// <typeparam name="T2">拓展类型2</typeparam>
        /// <typeparam name="T3">拓展类型3</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, T1, T2, T3, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, TResult>(this Expression<Func<TBase, T1, T2, T3, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, TResult>>(expression);
        }

        /// <summary>
        /// 组合继承属性选择表达式树,4个拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="T1">拓展类型1</typeparam>
        /// <typeparam name="T2">拓展类型2</typeparam>
        /// <typeparam name="T3">拓展类型3</typeparam>
        /// <typeparam name="T4">拓展类型4</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, T1, T2, T3, T4, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, T4, TResult>(this Expression<Func<TBase, T1, T2, T3, T4, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, T4, TResult>>(expression);
        }

        /// <summary>
        /// 组合继承属性选择表达式树,5个拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="T1">拓展类型1</typeparam>
        /// <typeparam name="T2">拓展类型2</typeparam>
        /// <typeparam name="T3">拓展类型3</typeparam>
        /// <typeparam name="T4">拓展类型4</typeparam>
        /// <typeparam name="T5">拓展类型5</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, T1, T2, T3, T4, T5, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, T4, T5, TResult>(this Expression<Func<TBase, T1, T2, T3, T4, T5, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, T4, T5, TResult>>(expression);
        }

        /// <summary>
        /// 组合继承属性选择表达式树,6个拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="T1">拓展类型1</typeparam>
        /// <typeparam name="T2">拓展类型2</typeparam>
        /// <typeparam name="T3">拓展类型3</typeparam>
        /// <typeparam name="T4">拓展类型4</typeparam>
        /// <typeparam name="T5">拓展类型5</typeparam>
        /// <typeparam name="T6">拓展类型6</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, T1, T2, T3, T4, T5, T6, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, T4, T5, T6, TResult>(this Expression<Func<TBase, T1, T2, T3, T4, T5, T6, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, T4, T5, T6, TResult>>(expression);
        }

        /// <summary>
        /// 组合继承属性选择表达式树,7个拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="T1">拓展类型1</typeparam>
        /// <typeparam name="T2">拓展类型2</typeparam>
        /// <typeparam name="T3">拓展类型3</typeparam>
        /// <typeparam name="T4">拓展类型4</typeparam>
        /// <typeparam name="T5">拓展类型5</typeparam>
        /// <typeparam name="T6">拓展类型6</typeparam>
        /// <typeparam name="T7">拓展类型7</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, T1, T2, T3, T4, T5, T6, T7, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, T4, T5, T6, T7, TResult>(this Expression<Func<TBase, T1, T2, T3, T4, T5, T6, T7, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, T4, T5, T6, T7, TResult>>(expression);
        }

        /// <summary>
        /// 组合继承属性选择表达式树,8个拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="T1">拓展类型1</typeparam>
        /// <typeparam name="T2">拓展类型2</typeparam>
        /// <typeparam name="T3">拓展类型3</typeparam>
        /// <typeparam name="T4">拓展类型4</typeparam>
        /// <typeparam name="T5">拓展类型5</typeparam>
        /// <typeparam name="T6">拓展类型6</typeparam>
        /// <typeparam name="T7">拓展类型7</typeparam>
        /// <typeparam name="T8">拓展类型8</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, T1, T2, T3, T4, T5, T6, T7, T8, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Expression<Func<TBase, T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, T4, T5, T6, T7, T8, TResult>>(expression);
        }

        /// <summary>
        /// 组合继承属性选择表达式树,9个拓展参数
        /// TResult将继承TBase的所有属性
        /// </summary>
        /// <typeparam name="TBase">原数据类型</typeparam>
        /// <typeparam name="T1">拓展类型1</typeparam>
        /// <typeparam name="T2">拓展类型2</typeparam>
        /// <typeparam name="T3">拓展类型3</typeparam>
        /// <typeparam name="T4">拓展类型4</typeparam>
        /// <typeparam name="T5">拓展类型5</typeparam>
        /// <typeparam name="T6">拓展类型6</typeparam>
        /// <typeparam name="T7">拓展类型7</typeparam>
        /// <typeparam name="T8">拓展类型8</typeparam>
        /// <typeparam name="T9">拓展类型9</typeparam>
        /// <typeparam name="TResult">返回类型</typeparam>
        /// <param name="expression">拓展表达式</param>
        /// <returns></returns>
        public static Expression<Func<TBase, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> BuildExtendSelectExpre<TBase, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Expression<Func<TBase, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression) where TResult : TBase
        {
            return GetExtendSelectExpre<TBase, TResult, Func<TBase, T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>>(expression);
        }

        #endregion

        //#region 拓展And和Or方法

        ///// <summary>
        ///// 连接表达式与运算
        ///// </summary>
        ///// <typeparam name="T">参数</typeparam>
        ///// <param name="one">原表达式</param>
        ///// <param name="another">新的表达式</param>
        ///// <returns></returns>
        //public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> one, Expression<Func<T, bool>> another)
        //{
        //    //创建新参数
        //    var newParameter = Expression.Parameter(typeof(T), "parameter");

        //    var parameterReplacer = new ParameterReplacer(newParameter);
        //    var left = parameterReplacer.Replace(one.Body);
        //    var right = parameterReplacer.Replace(another.Body);
        //    var body = Expression.And(left, right);

        //    return Expression.Lambda<Func<T, bool>>(body, newParameter);
        //}

        ///// <summary>
        ///// 连接表达式或运算
        ///// </summary>
        ///// <typeparam name="T">参数</typeparam>
        ///// <param name="one">原表达式</param>
        ///// <param name="another">新表达式</param>
        ///// <returns></returns>
        //public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> one, Expression<Func<T, bool>> another)
        //{
        //    //创建新参数
        //    var newParameter = Expression.Parameter(typeof(T), "parameter");

        //    var parameterReplacer = new ParameterReplacer(newParameter);
        //    var left = parameterReplacer.Replace(one.Body);
        //    var right = parameterReplacer.Replace(another.Body);
        //    var body = Expression.Or(left, right);

        //    return Expression.Lambda<Func<T, bool>>(body, newParameter);
        //}

        //#endregion

        #region 拓展Expression的Invoke方法

        public static TResult Invoke<TResult>(this Expression<Func<TResult>> expression)
        {
            return expression.Compile().Invoke();
        }

        public static TResult Invoke<T1, TResult>(this Expression<Func<T1, TResult>> expression, T1 arg1)
        {
            return expression.Compile().Invoke(arg1);
        }

        public static TResult Invoke<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T1 arg1, T2 arg2)
        {
            return expression.Compile().Invoke(arg1, arg2);
        }

        public static TResult Invoke<T1, T2, T3, TResult>(this Expression<Func<T1, T2, T3, TResult>> expression, T1 arg1, T2 arg2, T3 arg3)
        {
            return expression.Compile().Invoke(arg1, arg2, arg3);
        }

        public static TResult Invoke<T1, T2, T3, T4, TResult>(this Expression<Func<T1, T2, T3, T4, TResult>> expression, T1 arg1, T2 arg2, T3 arg3, T4 arg4)
        {
            return expression.Compile().Invoke(arg1, arg2, arg3, arg4);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, TResult>(this Expression<Func<T1, T2, T3, T4, T5, TResult>> expression, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5)
        {
            return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, TResult>> expression, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6)
        {
            return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, TResult>> expression, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7)
        {
            return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, TResult>> expression, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8)
        {
            return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, TResult>> expression, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9)
        {
            return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9);
        }

        public static TResult Invoke<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>(this Expression<Func<T1, T2, T3, T4, T5, T6, T7, T8, T9, T10, TResult>> expression, T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6, T7 arg7, T8 arg8, T9 arg9, T10 arg10)
        {
            return expression.Compile().Invoke(arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10);
        }

        #endregion

        #region 私有成员

        private static Expression<TDelegate> GetExtendSelectExpre<TBase, TResult, TDelegate>(Expression<TDelegate> expression)
        {
            NewExpression newBody = Expression.New(typeof(TResult));
            MemberInitExpression oldExpression = (MemberInitExpression)expression.Body;

            ParameterExpression[] oldParamters = expression.Parameters.ToArray();
            List<string> existsProperties = new List<string>();
            oldExpression.Bindings.ForEach(aBinding =>
            {
                existsProperties.Add(aBinding.Member.Name);
            });

            List<MemberBinding> bindings = oldExpression.Bindings.ToList();
            typeof(TBase).GetProperties().Where(x => !existsProperties.Contains(x.Name)).ForEach(aProperty =>
            {
                MemberInfo newMember = typeof(TResult).GetMember(aProperty.Name)[0];
                MemberBinding newMemberBinding = Expression.Bind(newMember, Expression.PropertyOrField(oldParamters[0], aProperty.Name));
                bindings.Add(newMemberBinding);
            });
            var body = Expression.MemberInit(newBody, bindings.ToArray());
            var resExpression = Expression.Lambda<TDelegate>(body, oldParamters);

            return resExpression;
        }

        #endregion
        /// <summary>
        /// ParameterRebinder
        /// </summary>
        private class ParameterRebinder : ExpressionVisitor
        {
            /// <summary>
            /// The ParameterExpression map
            /// </summary>
            readonly Dictionary<ParameterExpression, ParameterExpression> map;
            /// <summary>
            /// Initializes a new instance of the <see cref="ParameterRebinder"/> class.
            /// </summary>
            /// <param name="map">The map.</param>
            ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
            {
                this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
            }
            /// <summary>
            /// Replaces the parameters.
            /// </summary>
            /// <param name="map">The map.</param>
            /// <param name="exp">The exp.</param>
            /// <returns>Expression</returns>
            public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
            {
                return new ParameterRebinder(map).Visit(exp);
            }
            /// <summary>
            /// Visits the parameter.
            /// </summary>
            /// <param name="p">The p.</param>
            /// <returns>Expression</returns>
            protected override Expression VisitParameter(ParameterExpression p)
            {
                ParameterExpression replacement;

                if (map.TryGetValue(p, out replacement))
                {
                    p = replacement;
                }
                return base.VisitParameter(p);
            }
        }
        public class Filter
        {
            public string Key { get; set; }
            public string Value { get; set; }
            public string Contrast { get; set; }
        }
    }
}