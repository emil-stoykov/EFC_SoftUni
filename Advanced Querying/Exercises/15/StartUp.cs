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
            IncreasePrices(db);
        }

        public static void IncreasePrices(BookShopContext context)
        {
            List<Book> books 
                = context.Books
                    .Where(x => x.ReleaseDate.Value.Year < 2010)
                    .ToList();

            foreach (var b in books)
            {
                b.Price += 5;
            }

            context.SaveChanges();
        }
    }
}
