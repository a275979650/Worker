using System;
using System.Collections.Generic;
using Hk.Core.Data.DbContextCore;
using Hk.Core.Data.Options;
using Hk.Core.Data.Repositories;
using Hk.IServices;
using Microsoft.Extensions.Options;
using SqlSugar;

namespace Hk.Services
{
    public class TestService:BaseRepository,ITestService
    {
        public TestService(IDbContextCore context) : base(context)
        {
        }
    }
}
