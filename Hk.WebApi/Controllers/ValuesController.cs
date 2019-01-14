using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Hk.Core.Util.Extentions;
using Hk.IServices;
using Hk.Models;
using Microsoft.AspNetCore.Mvc;
using Remotion.Linq.Clauses;

namespace Hk.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private ITestService<Student> _testService;
        public ValuesController(ITestService<Student> testService)
        {
            _testService = testService;
        }
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public ActionResult<string> Get(int id)
        {
            //return "value";

            //var table= _testService.GetDataTable(
            //    "SELECT * FROM PEIS_PERSON_ASSESS WHERE PERSON_HOS_NO =\'1901070001\' AND ASSESS_KEY=\'0000009\'");
            //return Ok(table.Rows.Count);
            var s = _testService.GetList();
            return Ok(s.ToJson());
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
