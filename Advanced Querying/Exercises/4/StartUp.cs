namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            // Solutions go below
            string result = GetBooksByPrice(db);
            System.Console.WriteLine(result);
        }

        public static string GetBooksByPrice(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var booksByPrice
                = context.Books
                    .Where(b => b.Price > 40)
                    .OrderByDescending(b => b.Price)
                    .Select(b => new
                    {
                        bookTtile = b.Title,
                        bookPrice = b.Price,
                    })
                    .ToArray();

            foreach (var b in booksByPrice)
            {
                output.AppendLine($"{b.bookTtile} - ${b.bookPrice:f2}");
            }

            return output.ToString().TrimEnd();
        }
    }
}
