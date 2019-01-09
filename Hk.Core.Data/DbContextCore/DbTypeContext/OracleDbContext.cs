using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Hk.Core.Data.Options;
using Hk.Core.Util.Aspects;
using Microsoft.Extensions.Options;
using Oracle.ManagedDataAccess.Client;
using SqlSugar;

namespace Hk.Core.Data.DbContextCore.DbTypeContext
{
    public class OracleDbContext:BaseDbContext
    {
        private string _dbConnectstring = "";
        public OracleDbContext(IOptions<DbContextOption> option) : base(option)
        {
            _dbConnectstring = option.Value.ConnectionString;
            Db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = option.Value.ConnectionString,
                DbType = DbType.Oracle,
                InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样我就不多解释了
                ConfigureExternalServices = new ConfigureExternalServices()
                {
                    EntityService = (property, column) => {
                        //if (property.Name == "xxx")
                        //{//根据列名    
                        //    column.IsIgnore = true;
                        //}
        
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

        public override bool ExecuteSqlTran(List<string> sqlStringList)
        {
            bool result = false;
            using (OracleConnection oracleConnection = new OracleConnection(_dbConnectstring))
            {
                oracleConnection.Open();
                OracleCommand oracleCommand = new OracleCommand();
                oracleCommand.Connection = oracleConnection;
                OracleTransaction oracleTransaction = oracleConnection.BeginTransaction();
                oracleCommand.Transaction = oracleTransaction;
                try
                {
                    for (int i = 0; i < sqlStringList.Count; i++)
                    {
                        string text = sqlStringList[i].ToString();
                        if (text.Substring(0, 1) == ":")
                        {
                            text = sqlStringList[Convert.ToInt32(text.Substring(1))];
                        }
                        if (text.Trim().Length > 1)
                        {
                            oracleCommand.CommandText = text;
                            oracleCommand.ExecuteNonQuery();
                        }
                    }
                    oracleTransaction.Commit();
                    oracleTransaction.Dispose();
                    result = true;
                }
                catch (OracleException ex)
                {
                    oracleTransaction.Rollback();
                    result = false;
                    throw new Exception(ex.Message);
                }
                finally
                {
                    oracleConnection.Close();
                }
            }
            return result;
        }


    }
}