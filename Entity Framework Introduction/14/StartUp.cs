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

            string result = DeleteProjectById(context);
            Console.WriteLine(result);
        }

        public static string DeleteProjectById(SoftUniContext context)
        {
            StringBuilder result = new StringBuilder();

            foreach (var ep in context.EmployeesProjects.Where(ep => ep.ProjectId == 2))
            {
                context.EmployeesProjects.Remove(ep);
            }

            var project = context.Projects.Find(2);
            context.Projects.Remove(project);
            context.SaveChanges();

            var query = context
                .Projects
                .Select(p => new
                {
                    p.Name
                })
                .Take(10);

            foreach (var q in query)
            {
                result.AppendLine(q.Name);
            }

            return result.ToString().TrimEnd();
        }
    }
}
