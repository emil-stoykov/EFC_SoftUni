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
            string input = Console.ReadLine();
            string result = GetBookTitlesContaining(db, input);
            Console.WriteLine(result);
        }

        public static string GetBookTitlesContaining(BookShopContext context, string input)
        {
            //StringBuilder output = new StringBuilder();

            var books
                = context.Books
                    .Where(b => b.Title.ToLower().Contains(input.ToLower()))
                    .OrderBy(b => b.Title)
                    .Select(b => b.Title)
                    .ToArray();

            return string.Join(Environment.NewLine, books);
        }
    }
}
