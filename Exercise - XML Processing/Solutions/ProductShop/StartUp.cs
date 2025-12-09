namespace ProductShop {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using DTOs.Import;
    using DTOs.Export;
    using Models;
    using Microsoft.EntityFrameworkCore;

    public class StartUp {
        public static void Main(string[] args) {
            using ProductShopContext context = new ProductShopContext();
            context.Database.EnsureCreated();
            
            // Example usage
            // string xml = File.ReadAllText("Datasets/users.xml");
            // Console.WriteLine(ImportUsers(context, xml));
        }

        // Query 1
        public static string ImportUsers(ProductShopContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDto[]), new XmlRootAttribute("Users"));
            using StringReader stringReader = new StringReader(inputXml);
            ImportUserDto[] userDtos = (ImportUserDto[])xmlSerializer.Deserialize(stringReader)!;

            ICollection<User> users = new List<User>();
            foreach (var uDto in userDtos) {
                users.Add(new User {
                    FirstName = uDto.FirstName,
                    LastName = uDto.LastName,
                    Age = uDto.Age
                });
            }

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count}";
        }

        // Query 2
        public static string ImportProducts(ProductShopContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportProductDto[]), new XmlRootAttribute("Products"));
            using StringReader stringReader = new StringReader(inputXml);
            ImportProductDto[] productDtos = (ImportProductDto[])xmlSerializer.Deserialize(stringReader)!;

            ICollection<Product> products = new List<Product>();
            foreach (var pDto in productDtos) {
                products.Add(new Product {
                    Name = pDto.Name,
                    Price = pDto.Price,
                    SellerId = pDto.SellerId,
                    BuyerId = pDto.BuyerId
                });
            }

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count}";
        }

        // Query 3
        public static string ImportCategories(ProductShopContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoryDto[]), new XmlRootAttribute("Categories"));
            using StringReader stringReader = new StringReader(inputXml);
            ImportCategoryDto[] categoryDtos = (ImportCategoryDto[])xmlSerializer.Deserialize(stringReader)!;

            ICollection<Category> categories = new List<Category>();
            foreach (var cDto in categoryDtos) {
                if (cDto.Name == null) continue;
                
                categories.Add(new Category {
                    Name = cDto.Name
                });
            }

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count}";
        }

        // Query 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoryProductDto[]), new XmlRootAttribute("CategoryProducts"));
            using StringReader stringReader = new StringReader(inputXml);
            ImportCategoryProductDto[] cpDtos = (ImportCategoryProductDto[])xmlSerializer.Deserialize(stringReader)!;

            var categoryIds = context.Categories.Select(c => c.Id).ToHashSet();
            var productIds = context.Products.Select(p => p.Id).ToHashSet();

            ICollection<CategoryProduct> categoryProducts = new List<CategoryProduct>();
            foreach (var dto in cpDtos) {
                if (categoryIds.Contains(dto.CategoryId) && productIds.Contains(dto.ProductId)) {
                    categoryProducts.Add(new CategoryProduct {
                        CategoryId = dto.CategoryId,
                        ProductId = dto.ProductId
                    });
                }
            }

            context.CategoriesProducts.AddRange(categoryProducts);
            context.SaveChanges();

            return $"Successfully imported {categoryProducts.Count}";
        }

        // Query 5
        public static string GetProductsInRange(ProductShopContext context) {
            var productsRefined = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new {
                    p.Name,
                    p.Price,
                    BuyerFirstName = p.Buyer!.FirstName,
                    BuyerLastName = p.Buyer.LastName
                })
                .Take(10)
                .ToArray()
                .Select(p => new ExportProductInRangeDto {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = $"{p.BuyerFirstName} {p.BuyerLastName}".Trim()
                })
                .ToArray();

            return SerializeObject(productsRefined, "Products");
        }

        // Query 6
        public static string GetSoldProducts(ProductShopContext context) {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select(u => new ExportUserSoldProductsDto {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Select(p => new ExportProductDto {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToArray()
                })
                .ToArray();
            
            return SerializeObject(users, "Users");
        }

        // Query 7
        public static string GetCategoriesByProductsCount(ProductShopContext context) {
            var categories = context.Categories
                .Select(c => new ExportCategoryByProductsDto {
                    Name = c.Name,
                    Count = c.CategoriesProducts.Count,
                    AveragePrice = c.CategoriesProducts.Any() ? c.CategoriesProducts.Average(cp => cp.Product.Price) : 0,
                    TotalRevenue = c.CategoriesProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();
            
            return SerializeObject(categories, "Categories");
        }

        // Query 8
        public static string GetUsersWithProducts(ProductShopContext context) {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .ToArray()
                .Select(u => new {
                    u.FirstName,
                    u.LastName,
                    u.Age,
                    SoldProducts = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Select(p => new {
                            p.Name,
                            p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                })
                .OrderByDescending(u => u.SoldProducts.Length)
                .Take(10)
                .Select(u => new ExportUserWithProductsDto {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ExportSoldProductsDto {
                        Count = u.SoldProducts.Length,
                        Products = u.SoldProducts.Select(p => new ExportProductSimpleDto {
                            Name = p.Name,
                            Price = p.Price
                        }).ToArray()
                    }
                })
                .ToArray();
            
            var root = new ExportUsersRootDto {
                Count = context.Users.Count(u => u.ProductsSold.Any(p => p.BuyerId != null)),
                Users = users
            };

            return SerializeObject(root);
        }

        private static string SerializeObject<T>(T data, string rootName) {
             XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(rootName));
             StringBuilder sb = new StringBuilder();
             using StringWriter stringWriter = new StringWriter(sb);
             XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
             namespaces.Add(string.Empty, string.Empty);
             xmlSerializer.Serialize(stringWriter, data, namespaces);
             return sb.ToString();
        }
        
        private static string SerializeObject<T>(T data) {
             XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
             StringBuilder sb = new StringBuilder();
             using StringWriter stringWriter = new StringWriter(sb);
             XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
             namespaces.Add(string.Empty, string.Empty);
             xmlSerializer.Serialize(stringWriter, data, namespaces);
             return sb.ToString();
        }
    }
}
