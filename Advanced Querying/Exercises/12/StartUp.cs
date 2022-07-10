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
            string result = CountCopiesByAuthor(db);
            Console.WriteLine(result);
        }

        public static string CountCopiesByAuthor(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var authorsCopies 
                = context.Authors 
                    .Select(a => new
                    {
                        authorName = a.FirstName + " " + a.LastName,
                        totalCopies = a.Books.Sum(b => b.Copies)
                    })
                    .OrderByDescending(b => b.totalCopies)
                    .ToArray();

            foreach (var a in authorsCopies)
            {
                output.AppendLine($"{a.authorName} - {a.totalCopies}");
            }

            return output.ToString().TrimEnd();
        }
    }
}
