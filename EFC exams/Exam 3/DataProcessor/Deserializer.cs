namespace TeisterMask.DataProcessor
{
    using System;
    using System.Collections.Generic;

    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;
    using Data;
    using Newtonsoft.Json;
    using TeisterMask.Data.Models;
    using TeisterMask.Data.Models.Enums;
    using TeisterMask.DataProcessor.ImportDto;
    using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

    public class Deserializer
    {
        private const string ErrorMessage = "Invalid data!";

        private const string SuccessfullyImportedProject
            = "Successfully imported project - {0} with {1} tasks.";

        private const string SuccessfullyImportedEmployee
            = "Successfully imported employee - {0} with {1} tasks.";

        public static string ImportProjects(TeisterMaskContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var serializer = new XmlSerializer(typeof(List<ProjectImportModel>), new XmlRootAttribute("Projects"));
            List<ProjectImportModel> projectsDTOs = (List<ProjectImportModel>)serializer.Deserialize(new StringReader(xmlString));

            List<Project> validProjects = new List<Project>();

            foreach (var projectDTO in projectsDTOs)
            {
                if (!IsValid(projectDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                if (!projectDTO.Tasks.Any())
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                bool isProjectOpenDateValid
                    = DateTime.TryParseExact(projectDTO.OpenDate,
                                             "dd/MM/yyyy",
                                             CultureInfo.InvariantCulture,
                                             DateTimeStyles.None,
                                             out DateTime pOpenDateValue);
                if (!isProjectOpenDateValid)
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                DateTime? pDueDateValue = null;
                if (!String.IsNullOrWhiteSpace(projectDTO.DueDate))
                {
                    bool isProjectDueDateValid
                       = DateTime.TryParseExact(projectDTO.DueDate,
                                                "dd/MM/yyyy",
                                                CultureInfo.InvariantCulture,
                                                DateTimeStyles.None,
                                                out DateTime pDueDate);
                    if (!isProjectDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    pDueDateValue = pDueDate;
                }

                
                // do a check for both project dates?

                Project projectOBJ = new Project
                {
                    Name = projectDTO.Name,
                    OpenDate = pOpenDateValue,
                    DueDate = pDueDateValue
                };

                foreach (var task in projectDTO.Tasks)
                {
                    if (!IsValid(task))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isTaskOpenDateValid = DateTime.TryParseExact(task.OpenDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                                     DateTimeStyles.None, out DateTime taskOpenDate);
                    if (!isTaskOpenDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    bool isTaskDueDateValid
                        = DateTime.TryParseExact(task.DueDate, "dd/MM/yyyy", CultureInfo.InvariantCulture,
                                                 DateTimeStyles.None, out DateTime taskDueDate);

                    if (!isTaskDueDateValid)
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    if ((taskOpenDate < projectOBJ.OpenDate) || (pDueDateValue.HasValue && (taskDueDate > projectOBJ.DueDate)))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    Task taskObj = new Task
                    {
                        Name = task.Name,
                        OpenDate = taskOpenDate,
                        DueDate = taskDueDate,
                        ExecutionType = (ExecutionType)task.ExecutionType,
                        LabelType = (LabelType)task.LabelType
                    };

                    projectOBJ.Tasks.Add(taskObj);
                }

                sb.AppendLine(string.Format(SuccessfullyImportedProject, projectOBJ.Name, projectOBJ.Tasks.Count()));
                validProjects.Add(projectOBJ); 
            }

            context.Projects.AddRange(validProjects);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportEmployees(TeisterMaskContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            List<EmployeeImportModel> employeesDTO = JsonConvert.DeserializeObject<List<EmployeeImportModel>>(jsonString);

            List<int> validTaskIds = context.Tasks.Select(x => x.Id).ToList();
            var validEmployees = new List<Employee>();

            foreach (var employeeDTO in employeesDTO)
            {
                if (!IsValid(employeeDTO))
                {
                    sb.AppendLine(ErrorMessage);
                    continue;
                }

                Employee employeeObj = new Employee
                {
                    Username = employeeDTO.Username,
                    Email = employeeDTO.Email,
                    Phone = employeeDTO.Phone
                };

                foreach (var task in employeeDTO.Tasks.Distinct())
                {
                    if (!validTaskIds.Contains(task))
                    {
                        sb.AppendLine(ErrorMessage);
                        continue;
                    }

                    employeeObj.EmployeesTasks.Add(new EmployeeTask { Employee = employeeObj, TaskId = task });
                }

                sb.AppendLine(string.Format(SuccessfullyImportedEmployee, employeeObj.Username, employeeObj.EmployeesTasks.Count()));
                validEmployees.Add(employeeObj);
            }

            context.Employees.AddRange(validEmployees);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object dto)
        {
            var validationContext = new ValidationContext(dto);
            var validationResult = new List<ValidationResult>();

            return Validator.TryValidateObject(dto, validationContext, validationResult, true);
        }
    }
}