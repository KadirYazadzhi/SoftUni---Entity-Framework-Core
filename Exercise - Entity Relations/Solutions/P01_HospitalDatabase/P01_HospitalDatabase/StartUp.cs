namespace P01_HospitalDatabase {
    using System;
    using P01_HospitalDatabase.Data;

    public class StartUp {
        public static void Main(string[] args) {
            using var context = new HospitalContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            Console.WriteLine("Database created successfully!");
        }
    }
}
