namespace SoftJail.DataProcessor
{
    using AutoMapper;
    using Data;
    using Newtonsoft.Json;
    using SoftJail.Data.Models;
    using SoftJail.Data.Models.Enums;
    using SoftJail.DataProcessor.ImportDto;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml.Serialization;

    public class Deserializer
    {
        public static string ImportDepartmentsCells(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            //var config = new MapperConfiguration(cfg => cfg.AddProfile<SoftJailProfile>());
            //var mapper = config.CreateMapper();

            IEnumerable<DepartmentImportModel> departmentDTOs
                = JsonConvert.DeserializeObject<IEnumerable<DepartmentImportModel>>(jsonString);

            List<Department> validDepartments = new List<Department>();

            foreach (var departmentDTO in departmentDTOs)
            {
                if (!IsDepartmentValid(departmentDTO))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                Department departmentObj = new Department
                {
                    Name = departmentDTO.Name,
                    Cells = departmentDTO.Cells.Select(c => new Cell
                    {
                        HasWindow = c.HasWindow,
                        CellNumber = c.CellNumber
                    })
                    .ToList()
                };

                sb.AppendLine($"Imported {departmentObj.Name} with {departmentObj.Cells.Count()} cells");
                validDepartments.Add(departmentObj);
            }

            context.Departments.AddRange(validDepartments);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportPrisonersMails(SoftJailDbContext context, string jsonString)
        {
            StringBuilder sb = new StringBuilder();

            PrisonerImportModel[] prisonerDTOs
                = JsonConvert.DeserializeObject<PrisonerImportModel[]>(jsonString);

            List<Prisoner> validPrisoners = new List<Prisoner>();

            foreach (var prisonerDTO in prisonerDTOs)
            {
                if (!IsValid(prisonerDTO))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (!prisonerDTO.Mails.Any())
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                if (!prisonerDTO.Mails.Any(m => IsValid(m)))
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                bool IsIncarcerationDateValid
                    = DateTime.TryParseExact(prisonerDTO.IncarcerationDate, 
                                             "dd/MM/yyyy", 
                                             CultureInfo.InvariantCulture, 
                                             DateTimeStyles.None, 
                                             out DateTime incarcerationDate);

                if (!IsIncarcerationDateValid)
                {
                    sb.AppendLine("Invalid Data");
                    continue;
                }

                DateTime? releaseDateV = null;
                if (!string.IsNullOrEmpty(prisonerDTO.ReleaseDate))
                {
                    bool isReleaseDateValid
                        = DateTime.TryParseExact(prisonerDTO.ReleaseDate,
                                                 "dd/MM/yyyy",
                                                 CultureInfo.InvariantCulture,
                                                 DateTimeStyles.None,
                                                 out DateTime releaseDateValue);

                    if (!isReleaseDateValid)
                    {
                        sb.AppendLine("Invalid Data");
                        continue;
                    }

                    releaseDateV = releaseDateValue;
                }

                Prisoner prisonerObj = new Prisoner
                {
                    FullName = prisonerDTO.FullName,
                    Nickname = prisonerDTO.Nickname,
                    Age = prisonerDTO.Age,
                    IncarcerationDate = incarcerationDate,
                    ReleaseDate = releaseDateV,
                    Bail = prisonerDTO.Bail,
                    CellId = prisonerDTO.CellId,
                    Mails = prisonerDTO.Mails.Select(m => new Mail
                    {
                        Description = m.Description,
                        Sender = m.Sender,
                        Address = m.Address
                    })
                    .ToArray()
                };

                sb.AppendLine($"Imported {prisonerObj.FullName} {prisonerObj.Age} years old");
                validPrisoners.Add(prisonerObj);
            }

            context.Prisoners.AddRange(validPrisoners);
            context.SaveChanges();
            return sb.ToString().TrimEnd();
        }

        public static string ImportOfficersPrisoners(SoftJailDbContext context, string xmlString)
        {
            StringBuilder sb = new StringBuilder();

            var config = new MapperConfiguration(cfg => cfg.AddProfile<SoftJailProfile>());
            var mapper = config.CreateMapper();

            var serializer = new XmlSerializer(typeof(OfficerPrisonerImportModel[]), new XmlRootAttribute("Officers"));
            OfficerPrisonerImportModel[] opDTOs = (OfficerPrisonerImportModel[])serializer.Deserialize(new StringReader(xmlString));

            List<Officer> validOfficers = new List<Officer>();

            foreach (var opDTO in opDTOs)
            {
                if (!IsValid(opDTO))
                {
                    sb.AppendLine($"Invalid Data");
                    continue;
                }

                bool isPositionValid
                    = Enum.TryParse(opDTO.Position, out Position positionValue);

                if (!isPositionValid)
                {
                    sb.AppendLine($"Invalid Data");
                    continue;
                }

                bool isWeaponValid
                    = Enum.TryParse(opDTO.Weapon, out Weapon weaponValue);

                if (!isWeaponValid)
                {
                    sb.AppendLine($"Invalid Data");
                    continue;
                }

                Officer officerObj = new Officer
                {
                    FullName = opDTO.Name,
                    Salary = opDTO.Money,
                    Position = positionValue,
                    Weapon = weaponValue,
                    DepartmentId = opDTO.DepartmentId,
                    OfficerPrisoners = opDTO.Prisoners.Select(p => new OfficerPrisoner
                    {
                        PrisonerId = p.Id
                    })
                    .ToArray()
                };

                sb.AppendLine($"Imported {officerObj.FullName} ({officerObj.OfficerPrisoners.Count()} prisoners)");
                validOfficers.Add(officerObj);
            }

            context.Officers.AddRange(validOfficers);
            context.SaveChanges(); // FIX!!!!!
            return sb.ToString().TrimEnd();
        }

        private static bool IsValid(object obj)
        {
            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(obj);
            var validationResult = new List<ValidationResult>();

            bool isValid = Validator.TryValidateObject(obj, validationContext, validationResult, true);
            return isValid;
        }

        private static bool IsDepartmentValid(DepartmentImportModel dto)
        {
            bool isNameValid = dto.Name.Length >= 3 && dto.Name.Length <= 25;

            bool isCellValid = true;

            if (!dto.Cells.Any())
            {
                isCellValid = false;
            }

            foreach (var cell in dto.Cells)
            {
                if (cell.CellNumber >= 1 && cell.CellNumber <= 1000)
                {
                    continue;
                } else
                {
                    isCellValid = false;
                    break;
                }
            }


            return isNameValid && isCellValid;
        }
    }
}