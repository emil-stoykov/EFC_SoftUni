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

            string result = GetEmployee147(context);
            Console.WriteLine(result);
        }

        public static string GetEmployee147(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            Employee e147 = context.Employees
                .First(e => e.EmployeeId == 147);

            result.AppendLine($"{e147.FirstName} {e147.LastName} - {e147.JobTitle}");
            
            var projects = context.Projects
                .Where(p => p.EmployeesProjects.Any(ep => ep.EmployeeId == 147))
                .OrderBy(p => p.Name)
                .Select(p => p.Name)
                .ToArray();

            result.AppendLine(string.Join("\n", projects));

            return result.ToString().TrimEnd();
        }
    }
}
