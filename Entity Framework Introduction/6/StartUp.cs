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

            string result = AddNewAddressToEmployee(context);
            Console.WriteLine(result);
        }

        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var newAddress = new Address()
            {
                TownId = 4,
                AddressText = "Vitoshka 15"
            };

            context.Addresses.Add(newAddress);

            Employee employeesNakov = context.Employees.First(e => e.LastName == "Nakov");
            employeesNakov.Address = newAddress;
            context.SaveChanges();

            var employees = context
                .Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => new
                {
                    e.AddressId,
                    AddressText = e.Address.AddressText
                })
                .ToArray();

            foreach (var e in employees)
            {
                result.AppendLine(e.AddressText);
            }

            return result.ToString().TrimEnd();
        }
    }
}
