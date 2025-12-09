namespace MoviesApp.Services {
    using Microsoft.EntityFrameworkCore;
    using MoviesApp.Data;
    using MoviesApp.Data.Models;
    using MoviesApp.Services.Interfaces;
    using Newtonsoft.Json; // For JSON import
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;
    using System.Xml.Serialization; // For XML import

    public class ImportService : IImportService {
        private readonly AppDbContext _context;

        public ImportService(AppDbContext context) {
            _context = context;
        }

        public async Task<int> ImportMoviesFromJsonAsync(string jsonContent) {
            var movies = JsonConvert.DeserializeObject<List<Movie>>(jsonContent);
            if (movies != null) {
                await _context.Movies.AddRangeAsync(movies);
                await _context.SaveChangesAsync();
                return movies.Count;
            }
            return 0;
        }

        public async Task<int> ImportMoviesFromXmlAsync(string xmlContent) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(List<Movie>), new XmlRootAttribute("Movies"));
            using StringReader stringReader = new StringReader(xmlContent);
            var movies = (List<Movie>?)xmlSerializer.Deserialize(stringReader);

            if (movies != null) {
                await _context.Movies.AddRangeAsync(movies);
                await _context.SaveChangesAsync();
                return movies.Count;
            }
            return 0;
        }
    }
}
