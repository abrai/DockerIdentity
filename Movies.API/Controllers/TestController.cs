using Microsoft.AspNetCore.Mvc;
using System;
using System.Net;
using System.Threading.Tasks;


namespace Movies.API.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TestController : ControllerBase
    {

        public TestController()
        {
        }

        [HttpGet]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.OK)]
        public async Task<ActionResult> Get()
        {
            var welcomeMessage = string.Concat("Hello Abhishek :", DateTime.Now.ToString());
            return Ok(welcomeMessage);
        }
    }
}
