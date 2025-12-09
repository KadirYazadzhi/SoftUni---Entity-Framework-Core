namespace MoviesApp.Services.Interfaces {
    using MoviesApp.Data.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IWatchlistService {
        Task AddToWatchlistAsync(int userId, int movieId);
        Task RemoveFromWatchlistAsync(int userId, int movieId);
        Task<IEnumerable<Movie>> GetUserWatchlistAsync(int userId);
        Task<bool> IsMovieInWatchlistAsync(int userId, int movieId);
    }
}
