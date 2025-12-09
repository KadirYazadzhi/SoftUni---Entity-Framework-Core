namespace MoviesApp.Services {
    using Microsoft.EntityFrameworkCore;
    using MoviesApp.Data;
    using MoviesApp.Data.Models;
    using MoviesApp.Services.Interfaces;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class WatchlistService : IWatchlistService {
        private readonly AppDbContext _context;

        public WatchlistService(AppDbContext context) {
            _context = context;
        }

        public async Task AddToWatchlistAsync(int userId, int movieId) {
            if (!await IsMovieInWatchlistAsync(userId, movieId)) {
                _context.Watchlists.Add(new Watchlist { UserId = userId, MovieId = movieId });
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromWatchlistAsync(int userId, int movieId) {
            var entry = await _context.Watchlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.MovieId == movieId);
            if (entry != null) {
                _context.Watchlists.Remove(entry);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<Movie>> GetUserWatchlistAsync(int userId) {
            return await _context.Watchlists
                .Where(w => w.UserId == userId)
                .Select(w => w.Movie)
                .ToListAsync();
        }

        public async Task<bool> IsMovieInWatchlistAsync(int userId, int movieId) {
            return await _context.Watchlists.AnyAsync(w => w.UserId == userId && w.MovieId == movieId);
        }
    }
}
