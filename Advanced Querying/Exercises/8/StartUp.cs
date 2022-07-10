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
            string result = GetAuthorNamesEndingIn(db, input);
            Console.WriteLine(result);
        }

        public static string GetAuthorNamesEndingIn(BookShopContext context, string input)
        {
            StringBuilder output = new StringBuilder();

            var authors = 
                context.Authors
                    .Where(a => a.FirstName.EndsWith(input))
                    .Select(a => new
                    {
                        fullName = a.FirstName + " " + a.LastName,
                    })
                    .OrderBy(a => a.fullName)
                    .ToList();

            foreach(var a in authors)
            {
                output.AppendLine($"{a.fullName}");
            }

            return output.ToString().TrimEnd();
        }
    }
}
