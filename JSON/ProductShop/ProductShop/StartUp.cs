using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using AutoMapper;
using Newtonsoft.Json;
using ProductShop.Data;
using ProductShop.DTOs;
using ProductShop.Models;

namespace ProductShop
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            ProductShopContext productShopContext = new ProductShopContext();
            productShopContext.Database.EnsureDeleted();
            productShopContext.Database.EnsureCreated();

            string inputJson = File.ReadAllText(@"...\Datasets\users.json");
            string result = ImportUsers(productShopContext, inputJson);

            inputJson = File.ReadAllText(@"...\Datasets\products.json");
            result = ImportProducts(productShopContext, inputJson);

            inputJson = File.ReadAllText(@"...\Datasets\categories.json");
            result = ImportCategories(productShopContext, inputJson);

            inputJson = File.ReadAllText(@"...\Datasets\categories-products.json");
            result = ImportCategoryProducts(productShopContext, inputJson);



            //Console.WriteLine(GetProductsInRange(productShopContext));

            //Console.WriteLine(GetSoldProducts(productShopContext));

            //Console.WriteLine(GetCategoriesByProductsCount(productShopContext));

            //Console.WriteLine(GetUsersWithProducts(productShopContext));
        }

        // Import Data

        public static string ImportUsers(ProductShopContext context, string inputJson)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.AddProfile<ProductShopProfile>());

            var mapper = config.CreateMapper();

            IEnumerable<UserInputModel> dtoUsers = JsonConvert.DeserializeObject<IEnumerable<UserInputModel>>(inputJson);

            IEnumerable<User> users = mapper.Map<IEnumerable<User>>(dtoUsers);

            context.Users.AddRange(users);
            context.SaveChanges();

            return $"Successfully imported {users.Count()}";
        }

        public static string ImportProducts(ProductShopContext context, string inputJson)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.AddProfile<ProductShopProfile>());

            var mapper = config.CreateMapper();

            IEnumerable<ProductInputModel> dtoProducts = JsonConvert.DeserializeObject<IEnumerable<ProductInputModel>>(inputJson);

            IEnumerable<Product> products = mapper.Map<IEnumerable<Product>>(dtoProducts);

            context.Products.AddRange(products);
            context.SaveChanges();

            return $"Successfully imported {products.Count()}";
        }

        public static string ImportCategories(ProductShopContext context, string inputJson)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.AddProfile<ProductShopProfile>());

            var mapper = config.CreateMapper();

            IEnumerable<CategoryInputModel> dtoProducts
                = JsonConvert.DeserializeObject<IEnumerable<CategoryInputModel>>(inputJson).Where(x => x.Name != null);

            IEnumerable<Category> categories = mapper.Map<IEnumerable<Category>>(dtoProducts);

            context.Categories.AddRange(categories);
            context.SaveChanges();

            return $"Successfully imported {categories.Count()}";
        }

        public static string ImportCategoryProducts(ProductShopContext context, string inputJson)
        {
            MapperConfiguration config = new MapperConfiguration(cfg =>
            cfg.AddProfile<ProductShopProfile>());

            var mapper = config.CreateMapper();

            IEnumerable<CategoryProductsInputModel> dtoCP
                = JsonConvert.DeserializeObject <IEnumerable<CategoryProductsInputModel>> (inputJson);

            IEnumerable<CategoryProduct> cp = mapper.Map<IEnumerable<CategoryProduct>>(dtoCP);

            context.CategoryProducts.AddRange(cp);
            context.SaveChanges();

            return $"Successfully imported {cp.Count()}";
        }

        // Export Data
        public static string GetProductsInRange(ProductShopContext context)
        {
            var products
                = context.Products
                    .Where(p => p.Price >= 500 && p.Price <= 1000)
                    .OrderBy(p => p.Price)
                    .Select(p => new
                    {
                        name = p.Name,
                        price = p.Price,
                        seller = p.Seller.FirstName + " " + p.Seller.LastName,
                    })
                    .ToList();

            return JsonConvert.SerializeObject(products, Formatting.Indented);
        }

        public static string GetSoldProducts(ProductShopContext context)
        {
            var usersWithProductsSold
                = context.Users
                    .Where(u => u.ProductsSold.Any(y => y.BuyerId != null))
                    .OrderBy(u => u.LastName)
                    .ThenBy(u => u.FirstName)
                    .Select(u => new
                    {
                        firstName = u.FirstName,
                        lastName = u.LastName,
                        soldProducts = u.ProductsSold
                                       .Where(p => p.Buyer != null)
                                       .Select(p => new
                                       {
                                           name = p.Name,
                                           price = p.Price,
                                           buyerFirstName = p.Buyer.FirstName,
                                           buyerLastName = p.Buyer.LastName
                                       }
                                       ).ToList()
                    })
                    .ToList();

            return JsonConvert.SerializeObject(usersWithProductsSold, Formatting.Indented);
        }

        public static string GetCategoriesByProductsCount(ProductShopContext context)
        {
            var categories
                = context.Categories
                    .Select(c => new
                    {
                        category = c.Name,
                        productsCount = c.CategoryProducts.Count(),
                        averagePrice = c.CategoryProducts.Average(p => p.Product.Price).ToString("F2"),
                        totalRevenue = c.CategoryProducts.Sum(p => p.Product.Price).ToString("F2")
                    })
                    .OrderByDescending(u => u.productsCount)
                    .ToList();

            return JsonConvert.SerializeObject(categories, Formatting.Indented);
        }

        public static string GetUsersWithProducts(ProductShopContext context)
        {
            var users = context.Users
                .Where(u => u.ProductsSold.Any(p => p.BuyerId != null))
                .ToList()
                .Select(u => new
                {
                    firstName = u.FirstName,
                    lastName = u.LastName,
                    age = u.Age,
                    soldProducts = new
                    {
                        count = u.ProductsSold.Where(p => p.BuyerId != null).Count(),
                        products = u.ProductsSold.Where(p => p.BuyerId != null)
                        .Select(p => new
                        {
                            name = p.Name,
                            price = p.Price
                        })
                    }
                })
                .OrderByDescending(u => u.soldProducts.count)
                .ToList();

            var newObjectOfUsers = new
            {
                usersCount = users.Count,
                users = users

            };

            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore,
            };

            return JsonConvert.SerializeObject(newObjectOfUsers, settings);
        }
    }
}