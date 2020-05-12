using Microsoft.AspNetCore.Mvc;
using YesSql.Services;
using YesSql.Samples.Web.Models;
using System.Threading.Tasks;
using System;

namespace YesSql.Samples.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStore _store;

        public HomeController(IStore store)
        {
            _store = store;
        }

        [Route("/")]
        public async Task<IActionResult> Index()
        {
            BlogPost post;
            using (var session = _store.CreateSession())
            {
                post = await session.Query<BlogPost>().FirstOrDefaultAsync();
            }

            return View(post);
        }

        [Route("create")]
        public IActionResult Create()
        {
            // creating a blog post
            var post = new BlogPost
            {
                Title = "Hello YesSql",
                Author = "Bill",
                Content = "Hello",
                PublishedUtc = DateTime.UtcNow,
                Tags = new[] { "Hello", "YesSql" }
            };

            // saving the post to the database
            using (var session = _store.CreateSession())
            {
                session.Save(post);
            }
            return Content(post.Id.ToString());
        }
    }
}
