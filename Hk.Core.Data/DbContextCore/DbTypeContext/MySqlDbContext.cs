using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Hk.Core.Data.Options;
using Hk.Core.Util.Aspects;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace Hk.Core.Data.DbContextCore.DbTypeContext
{
    public class MySqlDbContext:BaseDbContext
    {
        private string _dbConnectstring = "";
        public MySqlDbContext(IOptions<DbContextOption> option) : base(option)
        {
            _dbConnectstring = option.Value.ConnectionString;
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = option.Value.ConnectionString,
                DbType = DbType.MySql,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (property, column) => {
                        var attributes = property.GetCustomAttributes(true);//get all attributes     
                        if (attributes.Any(it => it is KeyAttribute))//根据自定义属性    
                        {
                            column.IsPrimarykey = true;
                        }

                        if (attributes.Any(it => it is IgnoreAttribute))
                        {
                            column.IsIgnore = true;
                        }
                    },
                    EntityNameService = (type, entity) => {
                        var attributes = type.GetCustomAttributes(true);
                        if (attributes.Any(it => it is TableAttribute))
                        {
                            entity.DbTableName = (attributes.First(it => it is TableAttribute) as TableAttribute).Name;
                        }
                    }
                }

            });
            //调式代码 用来打印SQL 
            Db.Aop.OnLogExecuting = (sql, pars) =>
            {
                Console.WriteLine(sql + "\r\n" +
                                  Db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                Console.WriteLine();
            };
        }
    }
}