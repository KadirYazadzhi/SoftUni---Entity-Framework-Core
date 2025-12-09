namespace P01_StudentSystem {
    using System;
    using P01_StudentSystem.Data;

    public class StartUp {
        public static void Main(string[] args) {
            using var context = new StudentSystemContext();
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            
            Console.WriteLine("Database created successfully!");
        }
    }
}
