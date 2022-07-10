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
            int year = int.Parse(System.Console.ReadLine());
            string result = GetBooksNotReleasedIn(db, year);
            System.Console.WriteLine(result);
        }

        public static string GetBooksNotReleasedIn(BookShopContext context, int year)
        {
            StringBuilder output = new StringBuilder();

            var books 
                = context.Books
                    .Where(b => b.ReleaseDate.Value.Year != year)
                    .OrderBy(b => b.BookId)
                    .Select(b => new
                    {
                        bookTitle = b.Title
                    })
                    .ToArray();

            foreach (var b in books)
            {
                output.AppendLine($"{b.bookTitle}");
            }

            return output.ToString().TrimEnd();
        }
    }
}
