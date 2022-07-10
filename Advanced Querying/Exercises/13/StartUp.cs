namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            // Solutions go below
            //string input = Console.ReadLine();
            string result = GetTotalProfitByCategory(db);
            Console.WriteLine(result);
        }

        public static string GetTotalProfitByCategory(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var categories
                = context.Categories
                    .Select(c => new
                    {
                        categoryName = c.Name,
                        total = c.CategoryBooks.Sum(x => x.Book.Price * x.Book.Copies)
                    })
                    .OrderByDescending(c => c.total)
                    .ThenBy(c => c.categoryName)
                    .ToArray();

            foreach(var c in categories)
            {
                output.AppendLine($"{c.categoryName} ${c.total:f2}");
            }

            return output.ToString().TrimEnd();
        }
    }
}
