using System;
using System.Text;
using SoftUni.Data;
using SoftUni.Models;
using System.Linq;
using System.Collections.Generic;

namespace SoftUni
{
    public class StartUp
    {
        static void Main(string[] args)
        {
            var context = new SoftUniContext();

            string result = GetEmployeesWithSalaryOver50000(context);
            Console.WriteLine(result);
        }

        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var employees = context
                .Employees
                .OrderBy(e => e.FirstName)
                .Select(e => new
                {
                    e.FirstName,
                    e.MiddleName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .Where(e => e.Salary > 50000)
                .ToArray();

            foreach (var e in employees)
            {
                result.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return result.ToString().TrimEnd();
        }
    }
}
