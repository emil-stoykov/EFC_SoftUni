namespace BookShop
{
    using BookShop.Models;
    using BookShop.Models.Enums;
    using Data;
    using Initializer;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;
    using System.Text;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            // Solutions go below
            string command = System.Console.ReadLine();
            string result = GetBooksByAgeRestriction(db, command);
            System.Console.WriteLine(result);
        }

        public static string GetBooksByAgeRestriction(BookShopContext context, string command)
        {
            StringBuilder output = new StringBuilder();

            string input = command.ToLower();

            AgeRestriction ageRec = AgeRestriction.Minor;
            if (input == "minor")
            {
                ageRec = AgeRestriction.Minor;
            } else if (input == "teen")
            {
                ageRec = AgeRestriction.Teen;
            } else
            {
                ageRec = AgeRestriction.Adult;
            }

            // WHERE AgeRestriction = 0 
            var books
                = context.Books
                    .Where(x => x.AgeRestriction == ageRec)
                    .Select(b => new
                    {
                        bookName = b.Title
                    })
                    .OrderBy(b => b.bookName)
                    .ToArray();
                    

            foreach (var b in books)
            {
                output.AppendLine(b.bookName);
            }

            return output.ToString().TrimEnd();
        }
    }
}
