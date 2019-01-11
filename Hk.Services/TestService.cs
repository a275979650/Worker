using System;
using System.Collections.Generic;
using Hk.Core.Data.DbContextCore;
using Hk.Core.Data.Options;
using Hk.Core.Data.Repositories;
using Hk.IServices;
using Hk.Models;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace Hk.Services
{
    public class TestService:BaseRepositoryT<Student>,ITestService<Student>
    {
        public TestService(IDbContextCore context) : base(context)
        {
        }
    }
}
