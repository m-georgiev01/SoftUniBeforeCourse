using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            var usersJson = File.ReadAllText("../../../Datasets/users.json");
            var productsJson = File.ReadAllText("../../../Datasets/products.json");
            var categoriesJson = File.ReadAllText("../../../Datasets/categories.json");
            var categoriesProductJson = File.ReadAllText("../../../Datasets/categories-products.json");

            using (ProductShopContext context = new())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //Console.WriteLine(ImportUsers(context, usersJson));
                //Console.WriteLine(ImportProducts(context, productsJson));
                //Console.WriteLine(ImportCategories(context, categoriesJson));
                //Console.WriteLine(ImportCategoryProducts(context, categoriesProductJson));
                //Console.WriteLine(GetProductsInRange(context));
                //Console.WriteLine(GetSoldProducts(context));
                //Console.WriteLine(GetCategoriesByProductsCount(context));
                //Console.WriteLine(GetUsersWithProducts(context));
            }
        }


        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            var users = JsonConvert.DeserializeObject<User[]>(inputJson);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            var products = JsonConvert.DeserializeObject<Product[]>(inputJson);

            context.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            var categories = JsonConvert
            .DeserializeObject<Category[]>(inputJson)
            .Where(c => c.Name != null);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            var categoriesProducts = JsonConvert.DeserializeObject<CategoryProduct[]>(inputJson);

            context.AddRange(categoriesProducts);
            context.SaveChanges();

            return $"Successfully imported {categoriesProducts.Count()}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                    .Where(p => p.Price >= 500 && p.Price <= 1000)
                    .Select(p => new ProductInRangeDTO()
                    {
                        Name = p.Name,
                        Price = p.Price,
                        Seller = $"{p.Seller.FirstName} {p.Seller.LastName}",
                    })
                    .OrderBy(p => p.Price)
                    .ToList();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count() > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Select(u => new UserWithSoldProductsDTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new SoldProductDTO()
                    {
                        Name = p.Name,
                        Price = p.Price,
                        BuyerFirstName = p.Buyer.FirstName,
                        BuyerLastName = p.Buyer.LastName
                    })
                });

            return JsonConvert.SerializeObject(users, Formatting.Indented);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categorias = context.Categories
                .OrderByDescending(c => c.CategoriesProducts.Count)
                .Select(c => new CategoriesWithAvgPriceDTO()
                {
                    Category = c.Name,
                    ProductsCount = c.CategoriesProducts.Count,
                    AveragePrice = c.CategoriesProducts.Average(cp => cp.Product.Price).ToString("F2"),
                    TotalRevenue = c.CategoriesProducts.Sum(cp => cp.Product.Price).ToString("F2"),
                });

            return JsonConvert.SerializeObject(categorias, Formatting.Indented);
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var exportRoot = new ExportUsersRootDTO();

            var users = context.Users
                   .Include(x => x.ProductsSold)
                   .ToList()
                   .Where(u => u.ProductsSold.Any(p => p.Buyer != null))
                   .Select(u => new UserDTO()
                   {
                       FirstName = u.FirstName,
                       LastName = u.LastName,
                       Age = u.Age,
                       SoldProducts = new SoldProductExportRootDTO()
                       {
                           Count = u.ProductsSold.Count(p => p.Buyer != null),
                           Products = u.ProductsSold
                               .Where(p => p.Buyer != null)
                               .Select(p => new SoldProductExportDTO()
                               {
                                   Name = p.Name,
                                   Price = p.Price,
                               })
                       }
                   })
                   .OrderByDescending(u => u.SoldProducts.Count)
                   .ToList();

            exportRoot.UsersCount = users.Count;    
            exportRoot.Users = users;

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            string json = JsonConvert.SerializeObject(exportRoot, new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = Formatting.Indented
            });

            return json;
        }
    }
}