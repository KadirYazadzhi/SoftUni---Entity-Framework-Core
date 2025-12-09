namespace ProductShop {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using ProductShop.Data;
    using ProductShop.Models;
    using Microsoft.EntityFrameworkCore;

    public class StartUp {
        public static void Main(string[] args) {
            using ProductShopContext context = new ProductShopContext();
            context.Database.EnsureCreated();
            
            // Example usage
            // string inputJson = File.ReadAllText("Datasets/users.json");
            // Console.WriteLine(ImportUsers(context, inputJson));
        }

        // Query 1
        public static string ImportUsers(ProductShopContext context, string inputJson) {
            var users = JsonConvert.DeserializeObject<List<User>>(inputJson);
            context.Users.AddRange(users);
            context.SaveChanges();
            return $"Successfully imported {users.Count}";
        }

        // Query 2
        public static string ImportProducts(ProductShopContext context, string inputJson) {
            var products = JsonConvert.DeserializeObject<List<Product>>(inputJson);
            context.Products.AddRange(products);
            context.SaveChanges();
            return $"Successfully imported {products.Count}";
        }

        // Query 3
        public static string ImportCategories(ProductShopContext context, string inputJson) {
            var categories = JsonConvert.DeserializeObject<List<Category>>(inputJson)
                .Where(c => c.Name != null)
                .ToList();
            context.Categories.AddRange(categories);
            context.SaveChanges();
            return $"Successfully imported {categories.Count}";
        }

        // Query 4
        public static string ImportCategoryProducts(ProductShopContext context, string inputJson) {
            var categoryProducts = JsonConvert.DeserializeObject<List<CategoryProduct>>(inputJson);
            context.CategoriesProducts.AddRange(categoryProducts);
            context.SaveChanges();
            return $"Successfully imported {categoryProducts.Count}";
        }

        // Query 5
        public static string GetProductsInRange(ProductShopContext context) {
            var products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new {
                    name = p.Name,
                    price = p.Price,
                    seller = $"{p.Seller.FirstName} {p.Seller.LastName}"
                })
                .ToArray();

            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            return JsonConvert.SerializeObject(products, settings);
        }

        // Query 6
        public static string GetSoldProducts(ProductShopContext context) {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    soldProducts = u.ProductsSold
                        .Where(p => p.BuyerId != null)
                        .Select(p => new {
                            name = p.Name,
                            price = p.Price,
                            buyerFirstName = p.Buyer.FirstName,
                            buyerLastName = p.Buyer.LastName
                        })
                        .ToArray()
                })
                .ToArray();

            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            return JsonConvert.SerializeObject(users, settings);
        }

        // Query 7
        public static string GetCategoriesByProductsCount(ProductShopContext context) {
             var categories = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new {
                    category = c.Name,
                    productsCount = c.CategoriesProducts.Count,
                    averagePrice = c.CategoriesProducts.Any() 
                        ? c.CategoriesProducts.Average(cp => cp.Product.Price).ToString("F2") 
                        : "0.00",
                    totalRevenue = c.CategoriesProducts.Sum(cp => cp.Product.Price).ToString("F2")
                })
                .ToArray();

            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };
            return JsonConvert.SerializeObject(categories, settings);
        }

        // Query 8
        public static string GetUsersWithProducts(ProductShopContext context) {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .OrderByDescending(u => u.ProductsSold.Count(p => p.BuyerId != null))
                .Select(u => new {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new {
                        count = u.ProductsSold.Count(p => p.BuyerId != null),
                        products = u.ProductsSold
                            .Where(p => p.BuyerId != null)
                            .Select(p => new {
                                name = p.Name,
                                price = p.Price
                            })
                            .ToArray()
                    }
                })
                .ToArray();

            var result = new {
                usersCount = users.Length,
                users = users
            };

            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver {
                    NamingStrategy = new CamelCaseNamingStrategy()
                },
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(result, settings);
        }
    }
}
