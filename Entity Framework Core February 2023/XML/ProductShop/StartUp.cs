using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.Models;
using System.Text;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            string usersURI = "../../../Datasets/users.xml";
            string productsURI = "../../../Datasets/products.xml";
            string categoriesURI = "../../../Datasets/categories.xml";
            string categoryProductsURI = "../../../Datasets/categories-products.xml";

            using (ProductShopContext context = new())
            {
                //context.Database.EnsureDeleted();
                //context.Database.EnsureCreated();

                //Console.WriteLine(ImportUsers(context, usersURI));
                //Console.WriteLine(ImportProducts(context, productsURI));
                //Console.WriteLine(ImportCategories(context, categoriesURI));
                //Console.WriteLine(ImportCategoryProducts(context, categoryProductsURI));

                //Console.WriteLine(GetProductsInRange(context));
                //Console.WriteLine(GetSoldProducts(context));
                //Console.WriteLine(GetCategoriesByProductsCount(context));
                //Console.WriteLine(GetUsersWithProducts(context));
            }
        }

        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XDocument doc = XDocument.Load(inputXml);
            var users = doc.Root.Elements();

            List<User> usersList = new();
            foreach ( var user in users )
            {
                usersList.Add(new User()
                {
                    FirstName = user.Element("firstName").Value,
                    LastName = user.Element("lastName").Value,
                    Age = int.Parse(user.Element("age").Value)
                }) ;
            }

            context.AddRange(usersList);
            context.SaveChanges();

            return $"Successfully imported {usersList.Count()}"; ;
        }

        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XDocument doc = XDocument.Load(inputXml);
            var products = doc.Root.Elements();

            List<Product> productsList = new();
            foreach (var product in products)
            {  
                var productEl = new Product()
                {
                    Name = product.Element("name").Value,
                    Price = decimal.Parse(product.Element("price").Value),
                    SellerId = int.Parse(product.Element("sellerId").Value),
                };

                if (int.TryParse(product.Element("buyerId")?.Value, out int buyerId))
                {
                    productEl.BuyerId = buyerId;
                }

                productsList.Add(productEl);
            }

            context.AddRange(productsList);
            context.SaveChanges();

            return $"Successfully imported {productsList.Count}";
        }

        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XDocument doc = XDocument.Load(inputXml);
            var categories = doc.Root.Elements();

            List<Category> categoriesList = new();
            foreach (var category in categories)
            {
                categoriesList.Add(new Category()
                {
                    Name = category.Element("name").Value
                });
            }

            context.AddRange(categoriesList);
            context.SaveChanges();

            return $"Successfully imported {categoriesList.Count}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XDocument doc = XDocument.Load(inputXml);
            var catProducts = doc.Root.Elements();

            List<CategoryProduct> categoryProductsList = new();
            foreach (var categoryProduct in catProducts)
            {
                categoryProductsList.Add(new CategoryProduct()
                {
                    CategoryId = int.Parse(categoryProduct.Element("CategoryId").Value),
                    ProductId = int.Parse(categoryProduct.Element("ProductId").Value),
                });
            }

            context.AddRange(categoryProductsList);
            context.SaveChanges();

            return $"Successfully imported {categoryProductsList.Count}";
        }

        public static string GetProductsInRange(ProductShopContext context)
        {
            var products = context.Products
                .Where(p => p.Price >= 500 & p.Price <= 1000)
                .OrderBy(p => p.Price)
                .Select(p => new ProductsInRangeDTO()
                {
                    Name = p.Name,
                    Price = p.Price,
                    Buyer = p.Buyer != null ? $"{p.Buyer.FirstName} {p.Buyer.LastName}" : null
                })
                .Take(10)
                .ToArray();

            StringBuilder sb = new();
            var serializer = new XmlSerializer(typeof(ProductsInRangeDTO[]), new XmlRootAttribute("Products"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (StringWriter writer = new(sb))
            {
                serializer.Serialize(writer, products, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(10)
                .Select (u => new UserWithSoldProductsDTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold
                        .Select(p => new ProductDTO()
                        {
                            Name = p.Name,
                            Price = p.Price,
                        })
                        .Take(10)
                        .ToArray()
                })
                .ToArray();

            StringBuilder sb = new();
            var serializer = new XmlSerializer(typeof(UserWithSoldProductsDTO[]), new XmlRootAttribute("Users"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (StringWriter writer = new(sb))
            {
                serializer.Serialize(writer, users, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories = context.Categories
                .Select(c => new CategoriesByProductsDTO()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Select(cp => cp.Product).Count(),
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price),
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(CategoriesByProductsDTO[]), new XmlRootAttribute("Categories"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (StringWriter writer = new(sb))
            {
                serializer.Serialize(writer, categories, namespaces);
            }

            return sb.ToString().TrimEnd();
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderByDescending(u => u.ProductsSold.Count)
                .Select(u => new UserElementDTO()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    Age = u.Age,
                    SoldProducts = new ProductElementDTO()
                    {
                        Count = u.ProductsSold.Count,
                        Products = u.ProductsSold.Select(p => new ProductDTO()
                        {
                            Name = p.Name,
                            Price = p.Price
                        })
                        .OrderByDescending(p => p.Price)
                        .ToArray()
                    }
                })
                .OrderByDescending(u => u.SoldProducts.Count)
                .ToArray();

            var root = new UsersAndProductsDTO()
            {
                Count = users.Count(),
                Users = users.Take(10).ToArray(),
            };

            var sb = new StringBuilder();
            var serializer = new XmlSerializer(typeof(UsersAndProductsDTO), new XmlRootAttribute("Users"));
            var namespaces = new XmlSerializerNamespaces();
            namespaces.Add(string.Empty, string.Empty);

            using (StringWriter writer = new(sb))
            {
                serializer.Serialize(writer, root, namespaces);
            }

            return sb.ToString().TrimEnd();
        }
    }
}