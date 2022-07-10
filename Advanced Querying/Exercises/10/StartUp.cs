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
            string result = GetBooksByAuthor(db, input);
            Console.WriteLine(result);
        }

        public static string GetBooksByAuthor(BookShopContext context, string input)
        {
            StringBuilder output = new StringBuilder();

            var books
                = context.Books
                    .Where(b => b.Author.LastName.ToLower().StartsWith(input.ToLower()))
                    .OrderBy(b => b.BookId)
                    .Select(b => new
                    {
                        bookName = b.Title,
                        authorName = b.Author.FirstName + " " + b.Author.LastName
                    })
                    .ToArray();

            foreach(var b in books)
            {
                output.AppendLine($"{b.bookName} ({b.authorName})");
            }

            return output.ToString().TrimEnd();
        }
    }
}
