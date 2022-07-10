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
            string result = GetMostRecentBooks(db);
            Console.WriteLine(result);
        }

        public static string GetMostRecentBooks(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var query
                = context.Categories
                    .Select(c => new
                    {
                        categoryName = c.Name,
                        top3books = c.CategoryBooks
                                        .OrderByDescending(b => b.Book.ReleaseDate)
                                        .Take(3)
                                        .Select(b => new
                                        {
                                            bookTitle = b.Book.Title,
                                            releaseYear = b.Book.ReleaseDate.Value.Year
                                        })
                                        .ToList()
                    })
                    .OrderBy(c => c.categoryName)
                    .ToList();

            foreach (var c in query)
            {
                output.AppendLine($"--{c.categoryName}");

                foreach (var b in c.top3books)
                {
                    output.AppendLine($"{b.bookTitle} ({b.releaseYear})");
                }
            }

            return output.ToString().TrimEnd();
        }
    }
}
