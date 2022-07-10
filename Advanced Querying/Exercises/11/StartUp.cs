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
            int result = CountBooks(db, int.Parse(input));
            Console.WriteLine(result);
        }

        public static int CountBooks(BookShopContext context, int lengthCheck)
        {
            var books
                = context.Books
                    .Where(x => x.Title.Length > lengthCheck)
                    .ToArray();

            return books.Length;
        }
    }
}
