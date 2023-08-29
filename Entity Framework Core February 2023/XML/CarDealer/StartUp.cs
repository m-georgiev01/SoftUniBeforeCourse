using CarDealer.Data;
using CarDealer.DTOs.Export;
using CarDealer.DTOs.Import;
using CarDealer.Models;
using System.Text;
using System.Xml.Serialization;

namespace CarDealer
{
    public class StartUp
    {
        public static void Main()
        {
            var suppliersXml = File.ReadAllText("../../../Datasets/suppliers.xml");
            var partsXml = File.ReadAllText("../../../Datasets/parts.xml");
            var carsXml = File.ReadAllText("../../../Datasets/cars.xml");
            var customersXml = File.ReadAllText("../../../Datasets/customers.xml");
            var salesXml = File.ReadAllText("../../../Datasets/sales.xml");

            using (CarDealerContext context = new())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //Console.WriteLine(ImportSuppliers(context, suppliersXml));
                //Console.WriteLine(ImportParts(context, partsXml));
                //Console.WriteLine(ImportCars(context, carsXml));
                //Console.WriteLine(ImportCustomers(context, customersXml));
                //Console.WriteLine(ImportSales(context, salesXml));

                //Console.WriteLine(GetCarsWithDistance(context));
                //Console.WriteLine(GetCarsFromMakeBmw(context));
                //Console.WriteLine(GetLocalSuppliers(context));
                //Console.WriteLine(GetCarsWithTheirListOfParts(context));
                //Console.WriteLine(GetTotalSalesByCustomer(context));
                Console.WriteLine(GetSalesWithAppliedDiscount(context));
            }
        }

        public static string ImportSuppliers(CarDealerContext context, string inputXml)
        {
            SupplierImportDTO[] dtos = Deserializer<SupplierImportDTO[]>(inputXml, "Suppliers");

            var suppliers = dtos.Select(d => new Supplier()
            {
                Name = d.Name,
                IsImporter = d.IsImporter,
            });

            context.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Count()}";
        }

        public static string ImportParts(CarDealerContext context, string inputXml)
        {
            var supplierIds = context.Suppliers.Select(s => s.Id);
            PartsImportDTO[] dtos = Deserializer<PartsImportDTO[]>(inputXml, "Parts");
            dtos = dtos.Where(s => supplierIds.Any(sid => sid == s.SupplierId)).ToArray();

            var parts = dtos.Select(d => new Part()
            {
                Name = d.Name,
                Price = d.Price,
                Quantity = d.Quantity,
                SupplierId = d.SupplierId,
            });

            context.AddRange(parts);
            context.SaveChanges();

            return $"Successfully imported {parts.Count()}";
        }

        public static string ImportCars(CarDealerContext context, string inputXml)
        {
            var dtos = Deserializer<CarImportDTO[]>(inputXml, "Cars");
            var cars = new List<Car>();
            var parts = context.Parts.ToArray();

            foreach (var carDto in dtos)
            {
                var car = new Car
                {
                    Make = carDto.Make,
                    Model = carDto.Model,
                    TraveledDistance = carDto.TraveledDistance
                };

                var currCarParts = new List<PartCar>();
                foreach (var partId in carDto.Parts.Select(p => p.Id).Distinct())
                {
                    if (!parts.Any(p => p.Id == partId))
                    {
                        continue;
                    }

                    var partCar = new PartCar
                    {
                        Car = car,
                        PartId = partId
                    };

                    currCarParts.Add(partCar);
                }

                car.PartsCars = currCarParts;
                cars.Add(car);
            }

            context.Cars.AddRange(cars);
            context.SaveChanges();

            return $"Successfully imported {dtos.Count()}";
        }

        public static string ImportCustomers(CarDealerContext context, string inputXml)
        {
            var dtos = Deserializer<CustomerImportDTO[]>(inputXml, "Customers");

            var customers = dtos.Select(d => new Customer()
            {
                Name = d.Name,
                BirthDate = d.BirthDate,
                IsYoungDriver = d.IsYoungDriver,
            });

            context.Customers.AddRange(customers);
            context.SaveChanges();

            return $"Successfully imported {customers.Count()}";
        }

        public static string ImportSales(CarDealerContext context, string inputXml)
        {
            var carIds = context.Cars.Select(c => c.Id);

            var dtos = Deserializer<SaleImportDTO[]>(inputXml, "Sales");
            dtos = dtos.Where(d => carIds.Any(cid => cid == d.CarId)).ToArray();

            var sales = dtos.Select(d => new Sale()
            {
                Discount = d.Discount,
                CarId = d.CarId,
                CustomerId = d.CustomerId
            });

            context.Sales.AddRange(sales);
            context.SaveChanges();

            return $"Successfully imported {sales.Count()}";
        }

        public static string GetCarsWithDistance(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.TraveledDistance > 2_000_000)
                .Select(c => new CarWithDistanceDTO()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                })
                .OrderBy(c => c.Make)
                .ThenBy(c => c.Model)
                .Take(10)
                .ToArray();

            return Serializer(cars, "cars");
        }

        public static string GetCarsFromMakeBmw(CarDealerContext context)
        {
            var cars = context.Cars
                .Where(c => c.Make == "BMW")
                .Select(c => new CarsFromBmwDTO()
                {
                    Id = c.Id,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance
                })
                .OrderBy(c => c.Model)
                .ThenByDescending(c => c.TraveledDistance)
                .ToArray();

            return Serializer<CarsFromBmwDTO[]>(cars, "cars");
        }

        public static string GetLocalSuppliers(CarDealerContext context)
        {
            var suppliers = context.Suppliers
                .Where(s => s.IsImporter == false)
                .Select(s => new LocalSupplierDTO()
                {
                    Id = s.Id,
                    Name = s.Name,
                    PartsCount = s.Parts.Count
                })
                .ToArray();

            return Serializer<LocalSupplierDTO[]>(suppliers, "suppliers");
        }

        public static string GetCarsWithTheirListOfParts(CarDealerContext context)
        {
            var cars = context.Cars
                .Select(c => new CarWithPartsDTO()
                {
                    Make = c.Make,
                    Model = c.Model,
                    TraveledDistance = c.TraveledDistance,
                    Parts = c.PartsCars.Select(pc => new PartDTO()
                    {
                        Name = pc.Part.Name,
                        Price = pc.Part.Price
                    })
                    .OrderByDescending(p => p.Price)
                    .ToArray()
                })
                .OrderByDescending(c => c.TraveledDistance)
                .ThenBy(c => c.Model)
                .Take(5)
                .ToArray();

            return Serializer<CarWithPartsDTO[]>(cars, "cars");
        }

        public static string GetTotalSalesByCustomer(CarDealerContext context)
        {
            var customers = context.Customers
                   .Where(c => c.Sales.Any())
                   .Select(c => new TotalSaleByCustomerDTO()
                   {
                       FullName = c.Name,
                       BoughtCars = c.Sales.Count(),
                       SpentMoney = c.Sales.SelectMany(s => s.Car.PartsCars).Sum(pc => pc.Part.Price)
                   })
                   .OrderByDescending(c => c.SpentMoney)
                   .ToArray();

            return Serializer<TotalSaleByCustomerDTO[]>(customers, "customers");
        }

        public static string GetSalesWithAppliedDiscount(CarDealerContext context)
        {
            var sales = context.Sales
                .Select(s => new SaleWithDiscountDTO()
                {
                    Car = new CarSaleDTO
                    {
                        Make = s.Car.Make,
                        Model = s.Car.Model,
                        TraveledDistance = s.Car.TraveledDistance
                    },
                    Discount = s.Discount,
                    CustomerName = s.Customer.Name,
                    Price = s.Car.PartsCars.Sum(pc => pc.Part.Price),
                    PriceWithDiscount = s.Car.PartsCars.Sum(pc => pc.Part.Price) - (s.Car.PartsCars.Sum(pc => pc.Part.Price) * (s.Discount / 100)),

                })
                .ToArray();

            return Serializer<SaleWithDiscountDTO[]>(sales, "sales");
        }

        private static T Deserializer<T>(string inputXml, string rootName)
        {
            var xmlRoot = new XmlRootAttribute(rootName);
            var serializer = new XmlSerializer(typeof(T), xmlRoot);

            using (StringReader reader = new(inputXml))
            {
                T dtos = (T)serializer.Deserialize(reader);

                return dtos;
            }          
            
        }

        private static string Serializer<T>(T dto, string rootName)
        {
            var sb = new StringBuilder();

            var xmlRoot = new XmlRootAttribute(rootName);
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            var serializer = new XmlSerializer(typeof(T), xmlRoot);

            using (StringWriter writer = new(sb))
            {
                serializer.Serialize(writer, dto, namespaces);
            }          

            return sb.ToString().TrimEnd();
        }
    }
}