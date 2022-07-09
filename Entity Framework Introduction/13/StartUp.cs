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

            string result = GetEmployeesByFirstNameStartingWithSa(context);
            Console.WriteLine(result);
        }

        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {

            StringBuilder result = new StringBuilder();

            var query = context
                .Employees
                .Where(d => d.FirstName.StartsWith("Sa"))
                .OrderBy(d => d.FirstName)
                .ThenBy(d => d.LastName)
                .Select(d => new
                {
                    employeeName = d.FirstName + " " + d.LastName,
                    d.JobTitle,
                    d.Salary
                })
                .ToArray();

            foreach (var q in query)
            {
                result.AppendLine($"{q.employeeName} - {q.JobTitle} - (${q.Salary:f2})");
            }

            return result.ToString().TrimEnd();
        }
    }
}
