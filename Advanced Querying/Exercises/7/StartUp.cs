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
            string date = Console.ReadLine();
            string result = GetBooksReleasedBefore(db, date);
            System.Console.WriteLine(result);
        }

        public static string GetBooksReleasedBefore(BookShopContext context, string date)
        {
            DateTime dateTimeInput = DateTime.ParseExact(date, "dd-MM-yyyy", CultureInfo.InvariantCulture);

            var books
                = context.Books
                    .Where(x => x.ReleaseDate.Value < dateTimeInput)
                    .OrderByDescending(b => b.ReleaseDate.Value)
                    .Select(b => $"{b.Title} - {b.EditionType} - ${b.Price:f2}")
                    .ToList();

            return string.Join(Environment.NewLine, books);
        }
    }
}
