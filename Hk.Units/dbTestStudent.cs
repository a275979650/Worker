using System.Collections;
using System.Collections.Generic;
using Hk.Core.Util.Extentions;
using Hk.Core.Util.Helper;
using Hk.IServices;
using Hk.Models;
using Xunit;
using Xunit.Abstractions;

namespace Hk.Units
{
    [Collection("GlobalConfig")]
    public class dbTestStudent
    {
        private readonly ITestOutputHelper _output;
        private readonly ITestService<Student> _testService;

        public dbTestStudent(ITestOutputHelper output)
        {
            _output = output;
            _testService = Ioc.DefaultContainer.GetService<ITestService<Student>>();
        }
        [Fact]
        public void TestFind()
        {
            var result = _testService.GetList();
            _output.WriteLine(result.ToJson());
        }
        [Theory]
        [ClassData(typeof(CalculatorTestData))]
        public void TestInsert(Student student)
        {
            var a = _testService.Insert(student);
            _output.WriteLine(a.ToString());
        }



    }
    public class CalculatorTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new Student(){id = GuidHelper.GenerateKey(),name = "test1",sex="1"}};
            yield return new object[] { new Student() { id = GuidHelper.GenerateKey(), name = "test2", sex = "2" } };
            yield return new object[] { new Student() { id = GuidHelper.GenerateKey(), name = "test3", sex = "3" } };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }


    //public class CalculatorTestData : IEnumerable<object[]>
    //{
    //    public IEnumerator<object[]> GetEnumerator()
    //    {
    //        yield return new object[] { 1, 2, 3 };
    //        yield return new object[] { -4, -6, -10 };
    //        yield return new object[] { -2, 2, 0 };
    //        yield return new object[] { int.MinValue, -1, int.MaxValue };
    //    }

    //    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    //}
}