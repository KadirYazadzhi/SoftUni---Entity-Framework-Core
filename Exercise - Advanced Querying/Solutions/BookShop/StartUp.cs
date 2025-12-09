namespace BookShop {
    using System;
    using System.Diagnostics;
    using System.Text;
    using System.Linq;
    using System.Globalization;
    using System.Collections.Generic;

    using Microsoft.EntityFrameworkCore;
    using Z.EntityFramework.Plus;

    using Data;
    using Initializer;
    using Models;
    using Models.Enums;

    public class StartUp {
        public static void Main() {
            using var db = new BookShopContext();
            // DbInitializer.ResetDatabase(db);

            // Test your methods here
        }

        // Problem 02
        public static string GetBooksByAgeRestriction(BookShopContext context, string command) {
            if (!Enum.TryParse(command, true, out AgeRestriction ageRestriction)) {
                return string.Empty;
            }

            var bookTitles = context.Books
                .AsNoTracking()
                .Where(b => b.AgeRestriction == ageRestriction)
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // Problem 03
        public static string GetGoldenBooks(BookShopContext context) {
            var bookTitles = context.Books
                .AsNoTracking()
                .Where(b => b.EditionType == EditionType.Gold && b.Copies < 5000)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // Problem 04
        public static string GetBooksByPrice(BookShopContext context) {
            var books = context.Books
                .AsNoTracking()
                .Where(b => b.Price > 40)
                .OrderByDescending(b => b.Price)
                .Select(b => new {
                    b.Title,
                    b.Price
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var b in books) {
                sb.AppendLine($"{b.Title} - ${b.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 05
        public static string GetBooksNotReleasedIn(BookShopContext context, int year) {
            var bookTitles = context.Books
                .AsNoTracking()
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year != year)
                .OrderBy(b => b.BookId)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // Problem 06
        public static string GetBooksByCategory(BookShopContext context, string input) {
            string[] categoriesArr = input
                .ToLower()
                .Split(' ', StringSplitOptions.RemoveEmptyEntries);

            var bookTitles = context.Books
                .AsNoTracking()
                .Where(b => b.BookCategories
                    .Any(bc => categoriesArr.Contains(bc.Category.Name.ToLower())))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // Problem 07
        public static string GetBooksReleasedBefore(BookShopContext context, string date) {
            DateTime parsedDate = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books = context.Books
                .AsNoTracking()
                .Where(b => b.ReleaseDate < parsedDate)
                .OrderByDescending(b => b.ReleaseDate)
                .Select(b => new {
                    b.Title,
                    b.EditionType,
                    b.Price
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var b in books) {
                sb.AppendLine($"{b.Title} - {b.EditionType} - ${b.Price:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 08
        public static string GetAuthorNamesEndingIn(BookShopContext context, string input) {
            var authors = context.Authors
                .AsNoTracking()
                .Where(a => a.FirstName.EndsWith(input))
                .Select(a => new {
                    FullName = a.FirstName + " " + a.LastName
                })
                .OrderBy(a => a.FullName)
                .ToArray();

            return string.Join(Environment.NewLine, authors.Select(a => a.FullName));
        }

        // Problem 09
        public static string GetBookTitlesContaining(BookShopContext context, string input) {
            var bookTitles = context.Books
                .AsNoTracking()
                .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                .OrderBy(b => b.Title)
                .Select(b => b.Title)
                .ToArray();

            return string.Join(Environment.NewLine, bookTitles);
        }

        // Problem 10
        public static string GetBooksByAuthor(BookShopContext context, string input) {
            var books = context.Books
                .AsNoTracking()
                .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                .OrderBy(b => b.BookId)
                .Select(b => new {
                    b.Title,
                    AuthorName = b.Author.FirstName + " " + b.Author.LastName
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var b in books) {
                sb.AppendLine($"{b.Title} ({b.AuthorName})");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 11
        public static int CountBooks(BookShopContext context, int lengthCheck) {
            return context.Books
                .AsNoTracking()
                .Count(b => b.Title.Length > lengthCheck);
        }

        // Problem 12
        public static string CountCopiesByAuthor(BookShopContext context) {
            var authors = context.Authors
                .AsNoTracking()
                .Select(a => new {
                    FullName = a.FirstName + " " + a.LastName,
                    TotalCopies = a.Books.Sum(b => b.Copies)
                })
                .OrderByDescending(a => a.TotalCopies)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var a in authors) {
                sb.AppendLine($"{a.FullName} - {a.TotalCopies}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 13
        public static string GetTotalProfitByCategory(BookShopContext context) {
            var categories = context.Categories
                .AsNoTracking()
                .Select(c => new {
                    c.Name,
                    TotalProfit = c.CategoryBooks.Sum(cb => cb.Book.Copies * cb.Book.Price)
                })
                .OrderByDescending(c => c.TotalProfit)
                .ThenBy(c => c.Name)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var c in categories) {
                sb.AppendLine($"{c.Name} ${c.TotalProfit:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 14
        public static string GetMostRecentBooks(BookShopContext context) {
            var categories = context.Categories
                .AsNoTracking()
                .OrderBy(c => c.Name)
                .Select(c => new {
                    c.Name,
                    RecentBooks = c.CategoryBooks
                        .OrderByDescending(cb => cb.Book.ReleaseDate)
                        .Take(3)
                        .Select(cb => new {
                            cb.Book.Title,
                            ReleaseYear = cb.Book.ReleaseDate.Value.Year
                        })
                        .ToArray()
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var c in categories) {
                sb.AppendLine($"--{c.Name}");
                foreach (var b in c.RecentBooks) {
                    sb.AppendLine($"{b.Title} ({b.ReleaseYear})");
                }
            }

            return sb.ToString().TrimEnd();
        }

        // Problem 15
        public static void IncreasePrices(BookShopContext context) {
            var books = context.Books
                .Where(b => b.ReleaseDate.HasValue && b.ReleaseDate.Value.Year < 2010);
            
            foreach (var book in books) {
                book.Price += 5;
            }

            context.SaveChanges();
        }

        // Problem 16
        public static int RemoveBooks(BookShopContext context) {
            var booksToRemove = context.Books
                .Where(b => b.Copies < 4200);
            
            int count = booksToRemove.Count();
            context.Books.RemoveRange(booksToRemove);
            context.SaveChanges();

            return count;
        }
    }
}


