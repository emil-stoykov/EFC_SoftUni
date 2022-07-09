using System;
using System.Text;
using SoftUni.Data;
using SoftUni.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();

            string result = GetAddressesByTown(context);
            Console.WriteLine(result);
        }

        public static string GetAddressesByTown(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var query = context
                .Addresses
                .Select(a => new
                {
                    a.AddressText,
                    townName = a.Town.Name,
                    employeeCount = a.Employees.Count()
                })
                .OrderByDescending(a => a.employeeCount).ThenBy(a => a.townName).ThenBy(a => a.AddressText)
                .Take(10)
                .ToArray();

            foreach (var a in query)
            {
                result.AppendLine($"{a.AddressText}, {a.townName} - {a.employeeCount} employees");
            }

            return result.ToString().TrimEnd();
        }
    }
}
