using Hk.IServices;
using Hk.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using Hk.Core.Data.DbContextCore;
using Hk.Core.Data.Options;
using Hk.Core.Util.Extentions;
using Hk.Core.Util.Helper;
using Microsoft.Extensions.Options;
using SqlSugar;
using Xunit;
using Xunit.Abstractions;

namespace Hk.Units
{
    [Collection("GlobalConfig")]
    public class dbTest
    {
        private readonly ITestOutputHelper _output;

        private readonly DbContextOption _contextOption;
        private readonly IDbContextCore _dbContext;

        public dbTest(ITestOutputHelper output)
        {
            _output = output;
            _dbContext = Ioc.DefaultContainer.GetService<IDbContextCore>();
            _contextOption = Ioc.DefaultContainer.GetService<IOptions<DbContextOption>>().Value;
        }

        [Fact]
        public void TestInit()
        {
            _output.WriteLine("测试中...");
            _output.WriteLine(_contextOption.ConnectionString);
 ;
        }
        [Fact]
        public void TestDataTable()
        {
            var result = _dbContext.GetListBySql<Student>("SELECT * FROM STUDENT");
            _output.WriteLine(result.ToJson());
            DataTable data = _dbContext.GetDataTable("SELECT * FROM STUDENT");
            data.Rows.Cast<DataRow>().ForEach(x =>
            {
                _output.WriteLine(x["id"].ToString());
                _output.WriteLine(x["name"].ToString());
                _output.WriteLine(x["sex"].ToString());
            });
        }
        [Fact]
        public void TestInsert()
        {
            Student student = new Student()
            {
                id = GuidHelper.GenerateKey(),
                name = "test1",
                sex = "1"
            };
            Assert.True(_dbContext.Insert(student));
            Student student1 = new Student()
            {
                id = GuidHelper.GenerateKey(),
                name = "test2",
                sex = "2"
            };
            Student student2 = new Student()
            {
                id = GuidHelper.GenerateKey(),
                name = "test3",
                sex = "1"
            };
            List<Student> students = new List<Student>();
            students.Add(student1);
            students.Add(student2);
            Assert.True(_dbContext.InsertRange(students));
        }
        [Fact]
        public void TestDelete()
        {
            var s = _dbContext.GetSingleOrDefault<Student>(student => student.name=="test1");
            _output.WriteLine("找到的数据为:"+s.ToJson());
            Assert.True(_dbContext.Delete(s));
            _output.WriteLine("已删除的数据为："+s.ToJson());
            var students = _dbContext.GetList<Student>().Select(x=>x.id);
            Assert.True(_dbContext.DeleteByIds<Student>(students.ToArray()));
            _output.WriteLine("已删除所有记录");
        }
        [Fact]
        public void TestMysql()
        {
            DataTable data =  _dbContext.GetDataTable("SELECT * FROM activity_info ");
            data.Rows.Cast<DataRow>().ForEach(x =>
            {
                _output.WriteLine(x["activity_id_no"].ToString());
                _output.WriteLine(x["activity_name"].ToString());
                _output.WriteLine(x["summary"].ToString());
            });
        }
    }
}
