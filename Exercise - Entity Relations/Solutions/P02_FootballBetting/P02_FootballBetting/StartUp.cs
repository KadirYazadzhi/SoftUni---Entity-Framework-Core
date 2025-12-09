namespace P02_FootballBetting {
    using System;
    using P02_FootballBetting.Data;

    public class StartUp {
        public static void Main(string[] args) {
            using var context = new FootballBettingContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            Console.WriteLine("Database created successfully!");
        }
    }
}
