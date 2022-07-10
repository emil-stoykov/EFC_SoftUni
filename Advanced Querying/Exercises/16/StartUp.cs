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
    using Z.EntityFramework.Plus;

    public class StartUp
    {
        public static void Main()
        {
            using var db = new BookShopContext();
            DbInitializer.ResetDatabase(db);

            // Solutions go below
            //string input = Console.ReadLine();
            int result = RemoveBooks(db);
            Console.WriteLine(result);
        }

        public static int RemoveBooks(BookShopContext context)
        {
            return context.Books.Where(b => b.Copies < 4200).Delete();
        }
    }
}
