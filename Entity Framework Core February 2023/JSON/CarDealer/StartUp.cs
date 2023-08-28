using CarDealer.Data;
using CarDealer.DTOs;
using CarDealer.Models;
using Newtonsoft.Json;
using System.Globalization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var suppliersJson = File.ReadAllText("../../../Datasets/suppliers.json");
            var partsJson = File.ReadAllText("../../../Datasets/parts.json");
            var carsJson = File.ReadAllText("../../../Datasets/cars.json");
            var customersJson = File.ReadAllText("../../../Datasets/customers.json");
            var salesJson = File.ReadAllText("../../../Datasets/sales.json");

            using (CarDealerContext context = new())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //Console.WriteLine(ImportSuppliers(context, suppliersJson));
                //Console.WriteLine(ImportParts(context, partsJson));
                //Console.WriteLine(ImportCars(context, carsJson));
                //Console.WriteLine(ImportCustomers(context, customersJson));
                //Console.WriteLine(ImportSales(context, salesJson));
                //Console.WriteLine(GetOrderedCustomers(context));
                //Console.WriteLine(GetCarsFromMakeToyota(context));
                //Console.WriteLine(GetLocalSuppliers(context));
                //Console.WriteLine(GetCarsWithTheirListOfParts(context));
                //Console.WriteLine(GetTotalSalesByCustomer(context));
                //Console.WriteLine(GetSalesWithAppliedDiscount(context));
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputJson)
        {
            var suppliers = JsonConvert.DeserializeObject<Supplier[]>(inputJson);

            context.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}.";
        }

        public static string ImportParts(CarDealerContext context, string inputJson)
        {
            var minSupplierId = context.Suppliers.Min(s => s.Id);
            var maxSupplierId = context.Suppliers.Max(s => s.Id);

            var parts = JsonConvert.DeserializeObject<Part[]>(inputJson)
                    .Where(p => p.SupplierId >= minSupplierId && p.SupplierId <= maxSupplierId);

            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}.";
        }

        public static string ImportCars(CarDealerContext context, string inputJson)
        {
            var carDtos = JsonConvert.DeserializeObject<ImportCarDto[]>(inputJson);

            foreach (var carDto in carDtos)
            {
                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TraveledDistance
                };

                context.Cars.Add(car);
                context.SaveChanges();

                var carId = context.Cars.ToList().Last().Id;

                foreach (var partId in carDto.PartsId)
                {
                    var partCar = new PartCar
                    {
                        CarId = carId,
                        PartId = partId
                    };

                    if (car.PartsCars.FirstOrDefault(pc => pc.PartId == partId) == null)
                    {
                        context.PartsCars.Add(partCar);
                    }
                }
            }

            context.SaveChanges();

            return $"Successfully imported {carDtos.Count()}.";
        }

        public static string ImportCustomers(CarDealerContext context, string inputJson)
        {
            var customers = JsonConvert.DeserializeObject<Customer[]>(inputJson);

            context.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}.";
        }

        public static string ImportSales(CarDealerContext context, string inputJson)
        {
            var sales = JsonConvert.DeserializeObject<Sale[]>(inputJson);

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}.";
        }

        public static string GetOrderedCustomers(CarDealerContext context)
        {
            var customers = context.Customers
                .OrderBy(c => c.BirthDate)
                .ThenBy(c => c.IsYoungDriver)
                .Select(c => new OrderedCustomersDTO()
                {
                    Name = c.Name,
                    BirthDate = c.BirthDate.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
                    IsYoungDriver = c.IsYoungDriver,
                })
                .ToList();

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        public static string GetCarsFromMakeToyota(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "Toyota")
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .Select(c => new CarsFilteredByMakeDTO()
                {
                    Id = c.Id,
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                })
                .ToList();

            return JsonConvert.SerializeObject(cars, Formatting.Indented);
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new LocalSupplierDTO()
                {
                    Id= s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                });

            return JsonConvert.SerializeObject(suppliers, Formatting.Indented);
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new CarAndItsPartsDTO()
                {
                    Car = new CarDTO()
                    {
                        Model = c.Model,
                        Make = c.Make,
                        TraveledDistance = c.TraveledDistance
                    },
                    Parts = c.PartsCars.Select(pc => new PartDTO()
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price.ToString("F2") 
                    })
                })
                .ToList();

            return JsonConvert.SerializeObject (cars, Formatting.Indented);
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                .Where(c => c.Sales.Count > 0)
                .Select(c => new CustomerTotalSellsDTO()
                {
                    FullName = c.Name,
                    BoughtCars = c.Sales.Count,
                    SpentMoney = c.Sales.SelectMany(s => s.Car.PartsCars).Sum(pc => pc.Part.Price)
                })
                .OrderByDescending(c => c.SpentMoney)
                .ThenByDescending(c => c.BoughtCars);

            return JsonConvert.SerializeObject(customers, Formatting.Indented);
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Take(10)
                .Select(s => new CarCustomerPriceDTO()
                {
                    Car = new CarDTO()
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    CustomerName = s.Customer.Name,
                    Discount = s.Discount.ToString("F2"),
                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price).ToString("F2"),
                    PriceWithDiscount = $"{s.Car.PartsCars.Sum(pc => pc.Part.Price) - s.Car.PartsCars.Sum(pc => pc.Part.Price) * s.Discount / 100:F2}"
                });

            return JsonConvert.SerializeObject(sales, Formatting.Indented);
        }
    }
}