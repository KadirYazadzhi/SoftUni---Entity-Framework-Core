namespace P03_SalesDatabase {
    using System;
    using P03_SalesDatabase.Data;
    using P03_SalesDatabase.Data.Models;

    public class StartUp {
        public static void Main(string[] args) {
            using var context = new SalesContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            // Seed(context); // Optional seeding
            Console.WriteLine("Database created successfully!");
        }

        public static void Seed(SalesContext context) {
            // Seed logic here...
        }
    }
}
