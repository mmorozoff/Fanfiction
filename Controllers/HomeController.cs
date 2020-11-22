using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Fanfiction.Models;
using Fanfiction.Data;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Fanfiction.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db;
        public HomeController(ApplicationDbContext context)
        {
            db = context;
        }

        [HttpPost]
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Index(SortState sortOrder = SortState.TimeAsc)
        {
            if (User.IsInRole("blocked"))
            {
                return RedirectToAction("Account", "Identity", new { id = "Lockout" });
            }
            IQueryable<Fanfic> fanfics = db.Fanfics;
            ViewData["PopSort"] = sortOrder == SortState.ScoreAsc ? SortState.ScoreDesc : SortState.ScoreAsc;
            ViewData["TimeSort"] = sortOrder == SortState.TimeAsc ? SortState.TimeDesc : SortState.TimeAsc;
            ViewData["SizeSort"] = sortOrder == SortState.SizeAsc ? SortState.SizeDesc : SortState.SizeAsc;

            fanfics = sortOrder switch
            {
                SortState.ScoreDesc => fanfics.OrderByDescending(s => s.Score),
                SortState.ScoreAsc => fanfics.OrderBy(s => s.Score),
                SortState.TimeDesc => fanfics.OrderByDescending(s => s.CreateTime),
                SortState.TimeAsc => fanfics.OrderBy(s => s.CreateTime),
                SortState.SizeDesc => fanfics.OrderByDescending(s => s.Size),
                SortState.SizeAsc => fanfics.OrderBy(s => s.Size),
            };
            return View(await fanfics.AsNoTracking().Include(t => t.Tags).Include(g => g.Genres).ToListAsync());
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
