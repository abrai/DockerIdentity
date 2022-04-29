using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net.Http;
using System.Threading.Tasks;

namespace Movies.Client.Controllers
{
    public class TestController : Controller
    {
        private readonly ILogger<TestController> _logger;
        private readonly HttpClient _httpClient;

        public TestController(IHttpClientFactory httpClientFactory, ILogger<TestController> logger)
        {
            _logger = logger;
            _httpClient = httpClientFactory.CreateClient("MovieAPIClient");
        }

        public async Task<IActionResult> Index()
        {
            var response = await _httpClient.GetAsync("/Test");
            var content = await response.Content.ReadAsStringAsync();
            string myString = content.ToString();
            return View((object)myString);
            //return View(content);
        }
    }
}
