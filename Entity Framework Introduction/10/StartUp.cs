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

            string result = GetDepartmentsWithMoreThan5Employees(context);
            Console.WriteLine(result);
        }

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var query = context
                .Departments
                    .Where(d => d.Employees.Count > 5)
                    .OrderBy(d => d.Employees.Count)
                    .ThenBy(d => d.Name)
                    .Select(d => new
                    {
                        d.Name,
                        managerName = d.Manager.FirstName + " " + d.Manager.LastName,
                        employeesInDep = d
                        .Employees
                            .OrderBy(e => e.FirstName)
                            .ThenBy(e => e.LastName)
                            .Select(e => new
                            {
                                employeeName = e.FirstName + " " + e.LastName,
                                jobTitle = e.JobTitle
                            })
                            .ToArray()
                    })
                    .ToArray();

            foreach (var d in query)
            {
                result.AppendLine($"{d.Name} - {d.managerName}");

                foreach (var e in d.employeesInDep)
                {
                    result.AppendLine($"{e.employeeName} - {e.jobTitle}");
                }
            }

            return result.ToString().TrimEnd();
        }
    }
}
