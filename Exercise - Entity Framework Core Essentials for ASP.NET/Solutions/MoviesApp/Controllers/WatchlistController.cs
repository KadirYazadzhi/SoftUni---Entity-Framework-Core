using Microsoft.AspNetCore.Mvc;
using MoviesApp.Services.Interfaces;
using System.Threading.Tasks;

namespace MoviesApp.Controllers {
    public class WatchlistController : Controller {
        private readonly IWatchlistService _watchlistService;

        public WatchlistController(IWatchlistService watchlistService) {
            _watchlistService = watchlistService;
        }

        public async Task<IActionResult> Index(int userId) { // userId would come from authentication usually
            var watchlistMovies = await _watchlistService.GetUserWatchlistAsync(userId);
            return View(watchlistMovies);
        }

        // Add, Remove actions
    }
}
