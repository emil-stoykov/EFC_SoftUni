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

            string result = GetEmployeesInPeriod(context);
            Console.WriteLine(result);
        }

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            var query = context
                .Employees
                .Take(10)
                .Where(e => e.EmployeesProjects.Any(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003))
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    managerFirstName = e.Manager.FirstName,
                    managerLastName = e.Manager.LastName,
                    allProject = e.EmployeesProjects
                        .Select(ep => new
                        {
                            projectName = ep.Project.Name,
                            startDate = ep.Project.StartDate.ToString("M/d/yyyy h:mm:ss tt"),
                            endDate = ep.Project.EndDate.HasValue ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt") : "not finished"
                        })
                        .ToArray()
                })
                .ToArray();

            foreach (var e in query)
            {
                result.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.managerFirstName} {e.managerLastName}");
                
                foreach (var project in e.allProject)
                {
                    result.AppendLine($"--{project.projectName} - {project.startDate} - {project.endDate}");
                }
            }

            return result.ToString().TrimEnd();
        }
    }
}
