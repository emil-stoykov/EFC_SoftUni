namespace TeisterMask.DataProcessor
{
    using System;
    using Newtonsoft.Json;
    using System.Linq;
    using Data;

    using Formatting = Newtonsoft.Json.Formatting;
    using System.Globalization;
    using System.Text;
    using System.IO;
    using System.Xml.Serialization;
    using TeisterMask.DataProcessor.ExportDto;
    using System.Collections.Generic;

    public class Serializer
    {
        public static string ExportProjectWithTheirTasks(TeisterMaskContext context)
        {
            var sb = new StringBuilder();
            using StringWriter writer = new StringWriter();

            List<ProjectExportModel> projectDTOs
                = context.Projects
                .Where(x => x.Tasks.Any())
                .ToList()
                .Select(x => new ProjectExportModel
                {
                    ProjectName = x.Name,
                    HasEndDate = x.DueDate.HasValue ? "Yes" : "No",
                    TaskCount = x.Tasks.Count(),
                    Tasks 
                        = x.Tasks.Select(t => new TaskExportModel
                        {
                            Name = t.Name,
                            Label = t.LabelType.ToString()
                        })
                        .OrderBy(t => t.Name)
                        .ToList()
                })
                .OrderByDescending(x => x.TaskCount)
                .ThenBy(x => x.ProjectName)
                .ToList();

            var serializer = new XmlSerializer(typeof(List<ProjectExportModel>), new XmlRootAttribute("Projects"));

            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "");

            serializer.Serialize(writer, projectDTOs, namespaces);

            return writer.ToString().TrimEnd();
        }

        public static string ExportMostBusiestEmployees(TeisterMaskContext context, DateTime date)
        {
            var employees
                = context.Employees
                .Where(e => e.EmployeesTasks.Any(t => t.Task.OpenDate >= date))
                .ToList()
                .Select(e => new
                {
                    Username = e.Username,
                    Tasks
                        = e.EmployeesTasks
                        .Where(et => et.Task.OpenDate >= date)
                        .OrderByDescending(et => et.Task.DueDate)
                        .ThenBy(et => et.Task.Name)
                        .Select(et => new
                        {
                            TaskName = et.Task.Name,
                            OpenDate = et.Task.OpenDate.ToString("d", CultureInfo.InvariantCulture),
                            DueDate = et.Task.DueDate.ToString("d", CultureInfo.InvariantCulture),
                            LabelType = et.Task.LabelType.ToString(),
                            ExecutionType = et.Task.ExecutionType.ToString()
                        })
                        .ToList()
                })
                .OrderByDescending(e => e.Tasks.Count)
                .ThenBy(e => e.Username)
                .Take(10)
                .ToList();


            return JsonConvert.SerializeObject(employees, Formatting.Indented);
        }
    }
}