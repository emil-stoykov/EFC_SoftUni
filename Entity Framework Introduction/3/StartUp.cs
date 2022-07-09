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

            string result = GetEmployeesFullInformation(context);
            Console.WriteLine(result);
        }

        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var employees = context
                .Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new
                {
                    e.FirstName,
                    e.MiddleName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .ToArray();

            foreach (var e in employees)
            {
                result.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return result.ToString().TrimEnd();
        }
    }
}
