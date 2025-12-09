namespace CarDealer {
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
             using CarDealerContext context = new CarDealerContext();
             context.Database.EnsureCreated();
        }

        // Query 9
        public static string ImportSuppliers(CarDealerContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSupplierDto[]), new XmlRootAttribute("Suppliers"));
            using StringReader stringReader = new StringReader(inputXml);
            ImportSupplierDto[] supplierDtos = (ImportSupplierDto[])xmlSerializer.Deserialize(stringReader)!;

            ICollection<Supplier> suppliers = new List<Supplier>();
            foreach (var dto in supplierDtos) {
                suppliers.Add(new Supplier {
                    Name = dto.Name,
                    IsImporter = dto.IsImporter
                });
            }

            context.Suppliers.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count}";
        }

        // Query 10
        public static string ImportParts(CarDealerContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportPartDto[]), new XmlRootAttribute("Parts"));
            using StringReader stringReader = new StringReader(inputXml);
            ImportPartDto[] partDtos = (ImportPartDto[])xmlSerializer.Deserialize(stringReader)!;

            var supplierIds = context.Suppliers.Select(s => s.Id).ToHashSet();
            ICollection<Part> parts = new List<Part>();

            foreach (var dto in partDtos) {
                if (!supplierIds.Contains(dto.SupplierId)) continue;

                parts.Add(new Part {
                    Name = dto.Name,
                    Price = dto.Price,
                    Quantity = dto.Quantity,
                    SupplierId = dto.SupplierId
                });
            }

            context.Parts.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count}";
        }

        // Query 11
        public static string ImportCars(CarDealerContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCarDto[]), new XmlRootAttribute("Cars"));
            using StringReader stringReader = new StringReader(inputXml);
            ImportCarDto[] carDtos = (ImportCarDto[])xmlSerializer.Deserialize(stringReader)!;

            var partIds = context.Parts.Select(p => p.Id).ToHashSet();
            ICollection<Car> cars = new List<Car>();

            foreach (var dto in carDtos) {
                Car car = new Car {
                    Make = dto.Make,
                    Model = dto.Model,
                    TraveledDistance = dto.TraveledDistance
                };
                
                var distinctPartIds = dto.Parts.Select(p => p.Id).Distinct();
                foreach (var id in distinctPartIds) {
                    if (!partIds.Contains(id)) continue;
                    car.PartsCars.Add(new PartCar { PartId = id });
                }
                
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {cars.Count}";
        }

        // Query 12
        public static string ImportCustomers(CarDealerContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCustomerDto[]), new XmlRootAttribute("Customers"));
            using StringReader stringReader = new StringReader(inputXml);
            ImportCustomerDto[] customerDtos = (ImportCustomerDto[])xmlSerializer.Deserialize(stringReader)!;

            ICollection<Customer> customers = new List<Customer>();
            foreach (var dto in customerDtos) {
                customers.Add(new Customer {
                    Name = dto.Name,
                    BirthDate = dto.BirthDate,
                    IsYoungDriver = dto.IsYoungDriver
                });
            }

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count}";
        }

        // Query 13
        public static string ImportSales(CarDealerContext context, string inputXml) {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportSaleDto[]), new XmlRootAttribute("Sales"));
            using StringReader stringReader = new StringReader(inputXml);
            ImportSaleDto[] saleDtos = (ImportSaleDto[])xmlSerializer.Deserialize(stringReader)!;

            var carIds = context.Cars.Select(c => c.Id).ToHashSet();
            ICollection<Sale> sales = new List<Sale>();

            foreach (var dto in saleDtos) {
                if (!carIds.Contains(dto.CarId)) continue;

                sales.Add(new Sale {
                    CarId = dto.CarId,
                    CustomerId = dto.CustomerId,
                    Discount = dto.Discount
                });
            }

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count}";
        }

        // Query 14
        public static string GetCarsWithDistance(CarDealerContext context) {
            var cars = context.Cars
                .Where(c => c.TraveledDistance > 2000000)
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .Select(c => new ExportCarWithDistanceDto {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ToArray();

            return SerializeObject(cars, "cars");
        }

        // Query 15
        public static string GetCarsFromMakeBmw(CarDealerContext context) {
            var cars = context.Cars
                .Where(c => c.Make == "BMW")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new ExportCarBmwDto {
                    Id = c.Id,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .ToArray();

            return SerializeObject(cars, "cars");
        }

        // Query 16
        public static string GetLocalSuppliers(CarDealerContext context) {
            var suppliers = context.Suppliers
                .Where(s => !s.IsImporter)
                .Select(s => new ExportLocalSupplierDto {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            return SerializeObject(suppliers, "suppliers");
        }

        // Query 17
        public static string GetCarsWithTheirListOfParts(CarDealerContext context) {
            var cars = context.Cars
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .Select(c => new ExportCarWithPartsDto {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                    Parts = c.PartsCars
                        .Select(pc => pc.Part)
                        .OrderByDescending(p => p.Price)
                        .Select(p => new ExportPartDto {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .ToArray()
                })
                .ToArray();

            return SerializeObject(cars, "cars");
        }

        // Query 18
        public static string GetTotalSalesByCustomer(CarDealerContext context) {
            var customerDtos = context.Customers
                 .Where(c => c.Sales.Any())
                 .Select(c => new {
                     FullName = c.Name,
                     BoughtCars = c.Sales.Count,
                     SalesInfo = c.Sales.Select(s => new {
                         CarPrice = s.Car.PartsCars.Sum(pc => pc.Part.Price),
                         Discount = s.Discount,
                         IsYoung = c.IsYoungDriver
                     })
                 })
                 .ToArray()
                 .Select(c => new ExportCustomerSalesDto {
                     FullName = c.FullName,
                     BoughtCars = c.BoughtCars,
                     SpentMoney = c.SalesInfo.Sum(s => {
                         decimal totalDiscount = s.Discount + (s.IsYoung ? 5 : 0);
                         return Math.Round(s.CarPrice * (1 - totalDiscount / 100), 2);
                     })
                 })
                 .OrderByDescending(x => x.SpentMoney)
                 .ToArray();

            return SerializeObject(customerDtos, "customers");
        }

        // Query 19
        public static string GetSalesWithAppliedDiscount(CarDealerContext context) {
            var sales = context.Sales
                .Select(s => new {
                    s.Car,
                    s.Discount,
                    CustomerName = s.Customer.Name,
                    CarPrice = s.Car.PartsCars.Sum(pc => pc.Part.Price)
                })
                .ToArray()
                .Select(s => new ExportSaleDiscountDto {
                    Car = new ExportCarAttributeDto {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.CustomerName,
                    Price = s.CarPrice,
                    PriceWithDiscount = Math.Round(s.CarPrice * (1 - s.Discount / 100), 4)
                })
                .ToArray();

            return SerializeObject(sales, "sales");
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
    }
}
