namespace AcademicRecordsApp {
    using System;
    using AcademicRecordsApp.Data;

    public class StartUp {
        public static void Main(string[] args) {
            using var context = new AcademicRecordsDbContext();
            // context.Database.Migrate(); // Apply migrations if needed
            
            Console.WriteLine("Context loaded successfully.");
        }
    }
}
