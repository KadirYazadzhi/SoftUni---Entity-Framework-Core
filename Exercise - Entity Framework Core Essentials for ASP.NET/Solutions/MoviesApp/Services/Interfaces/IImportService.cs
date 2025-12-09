namespace MoviesApp.Services.Interfaces {
    using System.Threading.Tasks;

    public interface IImportService {
        Task<int> ImportMoviesFromJsonAsync(string jsonContent);
        Task<int> ImportMoviesFromXmlAsync(string xmlContent);
    }
}
