using Microsoft.AspNetCore.Mvc;
using MoviesApp.Data.Models;
using MoviesApp.Services.Interfaces;
using System.Threading.Tasks;

namespace MoviesApp.Controllers {
    public class MoviesController : Controller {
        private readonly IMoviesService _moviesService;

        public MoviesController(IMoviesService moviesService) {
            _moviesService = moviesService;
        }

        public async Task<IActionResult> Index() {
            var movies = await _moviesService.GetAllMoviesAsync();
            return View(movies);
        }

        // Other actions like Details, Create, Edit, Delete would go here
        // For simplicity, I'm just adding Index for now.
    }
}
