using BoardGamesStore.Data;
using BoardGamesStore.Models;
using BoardGamesStore.ViewModels;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace BoardGamesStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly BoardGamesStoreContext _context;

        public HomeController(BoardGamesStoreContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var recentArrivals = await _context.BoardGames
                .Include(bg => bg.BoardGameImages)
                .OrderByDescending(b => b.CreatedAt)
                .Take(4)
                .ToListAsync();

            var bestSellingGames = await _context.BoardGames
                .Include(bg => bg.BoardGameImages)
                .Include(bg => bg.OrderDetails)
                .OrderByDescending(b => b.OrderDetails.Sum(od => od.Quantity))
                .Take(4)
                .ToListAsync();

            var viewModel = new HomeViewModel
            {
                RecentArrivals = recentArrivals,
                BestSellingGames = bestSellingGames
            };

            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl = null)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return returnUrl != null ? LocalRedirect(returnUrl) : RedirectToAction("Index");
        }
    }
}
