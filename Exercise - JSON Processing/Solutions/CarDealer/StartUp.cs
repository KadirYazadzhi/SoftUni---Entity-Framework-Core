namespace CarDealer {
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using Data;
    using DTOs.Import;
    using Models;
    using Microsoft.EntityFrameworkCore;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class StartUp {
        public static void Main() {
            using CarDealerContext dbContext = new CarDealerContext();
            
            // Example usage
            // string result = GetSalesWithAppliedDiscount(dbContext);
            // Console.WriteLine(result);
        }

        // Problem 09
        public static string ImportSuppliers(CarDealerContext context, string inputJson) {
            ICollection<Supplier> suppliersToImport = new List<Supplier>();
            IEnumerable<ImportSupplierDto>? supplierDtos = JsonConvert.DeserializeObject<ImportSupplierDto[]>(inputJson);
            
            if (supplierDtos != null) {
                foreach (ImportSupplierDto supplierDto in supplierDtos) {
                    if (!IsValid(supplierDto)) {
                        continue;
                    }

                    bool isImporterValidVal = bool.TryParse(supplierDto.IsImporter, out bool isImporter);
                    if (!isImporterValidVal) {
                        continue;
                    }

                    Supplier newSupplier = new Supplier() {
                        Name = supplierDto.Name,
                        IsImporter = isImporter
                    };
                    suppliersToImport.Add(newSupplier);
                }

                context.Suppliers.AddRange(suppliersToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {suppliersToImport.Count}.";
        }

        // Problem 10
        public static string ImportParts(CarDealerContext context, string inputJson) {
            ICollection<Part> partsToImport = new List<Part>();
            var existingSuppliers = context.Suppliers.AsNoTracking().Select(s => s.Id).ToHashSet();

            IEnumerable<ImportPartDto>? partDtos = JsonConvert.DeserializeObject<ImportPartDto[]>(inputJson);
            if (partDtos != null) {
                foreach (ImportPartDto partDto in partDtos) {
                    if (!IsValid(partDto)) {
                        continue;
                    }

                    if (!int.TryParse(partDto.SupplierId, out int supplierId) || !existingSuppliers.Contains(supplierId)) {
                        continue;
                    }

                    Part newPart = new Part() {
                        Name = partDto.Name,
                        Price = partDto.Price,
                        Quantity = partDto.Quantity,
                        SupplierId = supplierId
                    };
                    partsToImport.Add(newPart);
                }

                context.Parts.AddRange(partsToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {partsToImport.Count}.";
        }

        // Problem 11
        public static string ImportCars(CarDealerContext context, string inputJson) {
            ICollection<Car> carsToImport = new List<Car>();
            IEnumerable<ImportCarDto>? carDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);
            
            if (carDtos != null) {
                var existingParts = context.Parts.Select(p => p.Id).ToHashSet();
                
                foreach (ImportCarDto carDto in carDtos) {
                    if (!IsValid(carDto)) {
                        continue;
                    }

                    Car newCar = new Car() {
                        Make = carDto.Make,
                        Model = carDto.Model,
                        TraveledDistance = carDto.TraveledDistance
                    };

                    foreach (int partId in carDto.PartsIds.Distinct()) {
                        if (!existingParts.Contains(partId)) {
                            continue;
                        }

                        newCar.PartsCars.Add(new PartCar { PartId = partId });
                    }
                    carsToImport.Add(newCar);
                }

                context.Cars.AddRange(carsToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {carsToImport.Count}.";
        }

        // Problem 12
        public static string ImportCustomers(CarDealerContext context, string inputJson) {
            ICollection<Customer> customersToImport = new List<Customer>();
            IEnumerable<ImportCustomerDto>? customerDtos = JsonConvert.DeserializeObject<ImportCustomerDto[]>(inputJson);
            
            if (customerDtos != null) {
                foreach (ImportCustomerDto customerDto in customerDtos) {
                    if (!IsValid(customerDto)) {
                        continue;
                    }

                    if (!DateTime.TryParseExact(customerDto.Birthdate, "yyyy-MM-dd'T'HH:mm:ss",
                            CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime birthDate)) {
                        continue;
                    }

                    if (!bool.TryParse(customerDto.IsYoungDriver, out bool isYoungDriver)) {
                        continue;
                    }

                    Customer newCustomer = new Customer() {
                        Name = customerDto.Name,
                        BirthDate = birthDate,
                        IsYoungDriver = isYoungDriver
                    };
                    customersToImport.Add(newCustomer);
                }

                context.Customers.AddRange(customersToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {customersToImport.Count}.";
        }

        // Problem 13
        public static string ImportSales(CarDealerContext context, string inputJson) {
            ICollection<Sale> salesToImport = new List<Sale>();
            IEnumerable<ImportSaleDto>? saleDtos = JsonConvert.DeserializeObject<ImportSaleDto[]>(inputJson);
            
            if (saleDtos != null) {
                var carIds = context.Cars.Select(c => c.Id).ToHashSet();
                var customerIds = context.Customers.Select(c => c.Id).ToHashSet();

                foreach (ImportSaleDto saleDto in saleDtos) {
                    if (!carIds.Contains(saleDto.CarId) || !customerIds.Contains(saleDto.CustomerId)) {
                        continue;
                    }

                    Sale newSale = new Sale() {
                        CarId = saleDto.CarId,
                        CustomerId = saleDto.CustomerId,
                        Discount = saleDto.Discount
                    };
                    salesToImport.Add(newSale);
                }

                context.Sales.AddRange(salesToImport);
                context.SaveChanges();
            }

            return $"Successfully imported {salesToImport.Count}.";
        }

        // Problem 14
        public static string GetOrderedCustomers(CarDealerContext context) {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver
                })
                .ToArray();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        // Problem 15
        public static string GetCarsFromMakeToyota(CarDealerContext context) {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        // Problem 16
        public static string GetLocalSuppliers(CarDealerContext context) {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        // Problem 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context) {
            var cars = context.Cars
                .Select(c => new {
                    car = new {
                        Make = c.Make,
                        Model = c.Model,
                        TraveledDistance = c.TraveledDistance
                    },
                    parts = c.PartsCars.Select(pc => new {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("F2")
                    }).ToArray()
                })
                .ToArray();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        // Problem 18
        public static string GetTotalSalesByCustomer(CarDealerContext context) {
             var customers = context.Customers
                .Where(c => c.Sales.Any())
                .Select(c => new {
                    fullName = c.Name,
                    boughtCars = c.Sales.Count,
                    spentMoney = c.Sales.Sum(s => s.Car.PartsCars.Sum(pc => pc.Part.Price))
                })
                .OrderByDescending(c => c.spentMoney)
                .ThenByDescending(c => c.boughtCars)
                .ToArray();

            var settings = new JsonSerializerSettings {
                Formatting = Formatting.Indented,
                ContractResolver = new DefaultContractResolver {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            };

            return JsonConvert.SerializeObject(customers, settings);
        }

        // Problem 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context) {
            var top10Sales = context.Sales
                .Take(10)
                .Select(s => new {
                    car = new {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    customerName = s.Customer.Name,
                    discount = s.Discount.ToString("f2"),
                    price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("f2"),
                    priceWithDiscount = (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (1 - s.Discount / 100)).ToString("f2")
                })
                .ToArray();

            return JsonConvert.SerializeObject(top10Sales, Formatting.Indented);
        }

        private static bool IsValid(object obj) {
            ValidationContext validationContext = new ValidationContext(obj);
            ICollection<ValidationResult> validationResults = new List<ValidationResult>();
            return Validator.TryValidateObject(obj, validationContext, validationResults);
        }
    }
}