namespace MoviesApp.Services.Interfaces {
    using MoviesApp.Data.Models;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public interface IMoviesService {
        Task<IEnumerable<Movie>> GetAllMoviesAsync();
        Task<Movie?> GetMovieByIdAsync(int id);
        Task AddMovieAsync(Movie movie);
        Task UpdateMovieAsync(Movie movie);
        Task DeleteMovieAsync(int id);
        Task<bool> MovieExistsAsync(int id);
    }
}
