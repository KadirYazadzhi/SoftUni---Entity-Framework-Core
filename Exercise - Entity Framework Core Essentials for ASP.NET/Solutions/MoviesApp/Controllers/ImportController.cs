using Microsoft.AspNetCore.Mvc;
using MoviesApp.Services.Interfaces;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Http; // For IFormFile

namespace MoviesApp.Controllers {
    public class ImportController : Controller {
        private readonly IImportService _importService;

        public ImportController(IImportService importService) {
            _importService = importService;
        }

        public IActionResult Index() {
            return View(); // A view with import forms
        }

        [HttpPost]
        public async Task<IActionResult> ImportJson(IFormFile file) {
            if (file == null || file.Length == 0) {
                return BadRequest("File not selected.");
            }

            using (var reader = new StreamReader(file.OpenReadStream())) {
                var jsonContent = await reader.ReadToEndAsync();
                var importedCount = await _importService.ImportMoviesFromJsonAsync(jsonContent);
                return Ok($"Imported {importedCount} movies from JSON.");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ImportXml(IFormFile file) {
            if (file == null || file.Length == 0) {
                return BadRequest("File not selected.");
            }

            using (var reader = new StreamReader(file.OpenReadStream())) {
                var xmlContent = await reader.ReadToEndAsync();
                var importedCount = await _importService.ImportMoviesFromXmlAsync(xmlContent);
                return Ok($"Imported {importedCount} movies from XML.");
            }
        }
    }
}
