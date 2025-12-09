namespace MoviesApp.Data.Models {
    public class Watchlist {
        public int UserId { get; set; }
        public int MovieId { get; set; }
        
        public virtual Movie Movie { get; set; } = null!;
    }
}
