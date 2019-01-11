using System;
using Hk.Core.Data.DbContextCore;
using Hk.Core.Data.Repositories;
using Hk.Core.Util.Dependency;

namespace Hk.IServices
{
    public interface ITestService<Student>:IRepositoryT<Student>,ITransientDependency where Student : class, new()
    {
    }
}
