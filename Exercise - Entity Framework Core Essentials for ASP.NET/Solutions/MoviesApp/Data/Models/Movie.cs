namespace MoviesApp.Data.Models {
    using System.ComponentModel.DataAnnotations;
    using System.Collections.Generic;

    public class Movie {
        public Movie() {
            this.Watchlists = new HashSet<Watchlist>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Title { get; set; } = null!;

        public int Year { get; set; }

        [MaxLength(50)]
        public string? Genre { get; set; }

        public decimal? Rating { get; set; }

        [MaxLength(100)]
        public string? Director { get; set; }

        public virtual ICollection<Watchlist> Watchlists { get; set; }
    }
}
