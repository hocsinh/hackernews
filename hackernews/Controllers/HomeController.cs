using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using hackernews.Models;
using hackernews.Services;

namespace hackernews.Controllers
{
    public class HomeController : Controller
    {
        private IHackerNewsDataService dataService;
        public HomeController(IHackerNewsDataService dataService)
        {
            this.dataService = dataService;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public async Task<ActionResult> Index(string searchString)
        {
            var news = await dataService.GetNewsListAsync();

            if (!string.IsNullOrEmpty(searchString))
            {
                var search = searchString.ToLower();
                // do not clear out user's input in the Search box
                ViewData["Filter"] = searchString;
                news = news.Where(n => n.Title.ToLower().IndexOf(search) > -1 || n.By.ToLower().IndexOf(search) > -1).ToList();
            }

            return View(news);
        }

    }
}
