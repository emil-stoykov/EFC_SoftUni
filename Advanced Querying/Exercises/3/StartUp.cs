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
            string result = GetGoldenBooks(db);
            System.Console.WriteLine(result);
        }

        public static string GetGoldenBooks(BookShopContext context)
        {
            StringBuilder output = new StringBuilder();

            var books
                = context.Books
                    .Where(x => x.Copies < 5000 && x.EditionType == EditionType.Gold)
                    .OrderBy(x => x.BookId)
                    .Select(x => new
                    {
                        bookName = x.Title
                    })
                    .ToArray();

            foreach (var book in books)
            {
                output.AppendLine($"{book.bookName}");
            }

            return output.ToString().TrimEnd();
        }
    }
}
