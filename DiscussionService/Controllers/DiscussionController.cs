using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace DiscussionService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DiscussionController : ControllerBase
    {
        // GET: api/<DiscussionController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<DiscussionController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<DiscussionController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<DiscussionController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<DiscussionController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
