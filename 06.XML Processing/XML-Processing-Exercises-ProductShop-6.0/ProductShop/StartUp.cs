using ProductShop.Data;
using ProductShop.DTOs.Export;
using ProductShop.DTOs.Import;
using ProductShop.Models;
using System.Globalization;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main()
        {
            ProductShopContext context = new ProductShopContext();

            //01

            //string usersxml = File.ReadAllText("../../../Datasets/users.xml");
            //Console.WriteLine(ImportUsers(context, usersxml));

            //02

            //string productsxml = File.ReadAllText("../../../Datasets/products.xml");
            //Console.WriteLine(ImportProducts(context, productsxml));


            //03
            //string categoriesXml = File.ReadAllText("../../../Datasets/categories.xml");
            //Console.WriteLine(ImportCategories(context, categoriesXml));

            //04
            //string categoryProduct = File.ReadAllText("../../../Datasets/categories-products.xml");
            //Console.WriteLine(ImportCategoryProducts(context, categoryProduct));

            Console.WriteLine(GetCategoriesByProductsCount(context));
        }

        //01. Import Users
        public static string ImportUsers(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportUserDTO[]),
                new XmlRootAttribute("Users"));
            ImportUserDTO[] importDtos;
            using (var reader = new StringReader(inputXml))
            {
                importDtos = (ImportUserDTO[])xmlSerializer.Deserialize(reader);
            };

            User[] suppliers = importDtos
                .Select(dto => new User()
                {
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    Age = dto.Age,
                }).ToArray();

            context.Users.AddRange(suppliers);
            context.SaveChanges();

            return $"Successfully imported {suppliers.Length}";
        }

        //02. Import Products
        public static string ImportProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ProductsDto[]),
                new XmlRootAttribute("Products"));
            ProductsDto[] importDtos;
            using (var reader = new StringReader(inputXml))
            {
                importDtos = (ProductsDto[])xmlSerializer.Deserialize(reader);
            };

            Product[] products = importDtos
                .Select(dto => new Product()
                {
                    Name = dto.Name,
                    Price = dto.Price,
                    SellerId = dto.SellerId,
                    BuyerId = dto.BuyerId,
                }).ToArray();

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Length}";
        }

        //03. Import Categories
        public static string ImportCategories(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(ImportCategoryDTO[]),
               new XmlRootAttribute("Categories"));
            ImportCategoryDTO[] importDtos;
            using (var reader = new StringReader(inputXml))
            {
                importDtos = (ImportCategoryDTO[])xmlSerializer.Deserialize(reader);
            };

            Category[] categories = importDtos
                .Select(dto => new Category()
                {
                    Name = dto.Name
                }).ToArray();

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Length}";
        }

        //04. Import Categories and Products
        public static string ImportCategoryProducts(ProductShopContext context, string inputXml)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(CategoryProductDto[]),
                new XmlRootAttribute("CategoryProducts"));
            CategoryProductDto[] importDtos;
            using (var reader = new StringReader(inputXml))
            {
                importDtos = (CategoryProductDto[])xmlSerializer.Deserialize(reader);
            };

            CategoryProduct[] categoryProduct = importDtos
                .Select(dto => new CategoryProduct()
                {
                    CategoryId = dto.CategoryId,
                    ProductId = dto.ProductId
                }).ToArray();

            context.CategoryProducts.AddRange(categoryProduct);
            context.SaveChanges();

            return $"Successfully imported {categoryProduct.Length}";

        }

        //05. Export Products In Range
        public static string GetProductsInRange(ProductShopContext context)
        {
            ExportProductsDto[] products = context.Products
                .Where(p => p.Price >= 500 && p.Price <= 1000)
                .Select(p => new ExportProductsDto()
                {
                    Name = p.Name,
                    Price = p.Price,
                    BuyerName = $"{p.Buyer.FirstName} {p.Buyer.LastName}"
                })
                .OrderBy(p => p.Price)
                .Take(10)
                .ToArray();

            return SerializeToXml(products, "Products");
        }

        //06. Export Sold Products
        public static string GetSoldProducts(ProductShopContext context)
        {
            UserSoldProductsDto[] users = context.Users
                .Where(u => u.ProductsSold.Count > 0)
                .OrderBy(u => u.LastName)
                .ThenBy(u => u.FirstName)
                .Take(5)
                .Select (u => new UserSoldProductsDto()
                {
                    FirstName = u.FirstName,
                    LastName = u.LastName,
                    SoldProducts = u.ProductsSold.Select(p => new ProductDto()
                    {
                        Name = p.Name,
                        Price = p.Price,
                    })
                    .ToArray()
                })
                .ToArray();

            return SerializeToXml(users, "Users");
        }

        //07. Export Categories By Products Count
        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            ExportCategoryDto[] categories = context.Categories
                .Select(c => new ExportCategoryDto()
                {
                    Name = c.Name,
                    Count = c.CategoryProducts.Count,
                    AveragePrice = c.CategoryProducts.Average(cp => cp.Product.Price),
                    TotalRevenue = c.CategoryProducts.Sum(cp => cp.Product.Price)
                })
                .OrderByDescending(c => c.Count)
                .ThenBy(c => c.TotalRevenue)
                .ToArray();

            return SerializeToXml(categories, "Categories");
        }

        //08. Export Users and Products
        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var usersInfo = context
               .Users
               .Where(u => u.ProductsSold.Any())
               .OrderByDescending(u => u.ProductsSold.Count)
               .Select(u => new UserInfo()
               {
                   FirstName = u.FirstName,
                   LastName = u.LastName,
                   Age = u.Age,
                   SoldProducts = new SoldProductsCount()
                   {
                       Count = u.ProductsSold.Count,
                       Products = u.ProductsSold.Select(p => new SoldProduct()
                       {
                           Name = p.Name,
                           Price = p.Price
                       })
                       .OrderByDescending(p => p.Price)
                       .ToArray()
                   }
               })
               .Take(10)
               .ToArray();

            ExportUserCountDto exportUserCountDto = new ExportUserCountDto()
            {
                Count = context.Users.Count(u => u.ProductsSold.Any()),
                Users = usersInfo
            };

            return SerializeToXml(exportUserCountDto, "Users");
        }


        private static string SerializeToXml<T>(T dto, string xmlRootAttribute, bool omitDeclaration = false)
        {
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(T), new XmlRootAttribute(xmlRootAttribute));
            StringBuilder stringBuilder = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings
            {
                OmitXmlDeclaration = omitDeclaration,
                Encoding = new UTF8Encoding(false),
                Indent = true
            };

            using (StringWriter stringWriter = new StringWriter(stringBuilder, CultureInfo.InvariantCulture))
            using (XmlWriter xmlWriter = XmlWriter.Create(stringWriter, settings))
            {
                XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
                xmlSerializerNamespaces.Add(string.Empty, string.Empty);

                try
                {
                    xmlSerializer.Serialize(xmlWriter, dto, xmlSerializerNamespaces);
                }
                catch (Exception)
                {
                    throw;
                }
            }

            return stringBuilder.ToString();
        }
    }
}