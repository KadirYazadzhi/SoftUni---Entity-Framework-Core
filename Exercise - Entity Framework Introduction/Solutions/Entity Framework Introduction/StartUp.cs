namespace SoftUni {
    using System;
    using System.Linq;
    using System.Text;
    using System.Globalization;
    using Data;
    using Models;
    using Microsoft.EntityFrameworkCore;

    public class StartUp {
        public static void Main(string[] args) {
             using SoftUniContext context = new SoftUniContext();
             // Test methods here if needed
        }

        // Problem 03
        public static string GetEmployeesFullInformation(SoftUniContext context) {
            StringBuilder sb = new StringBuilder();
            var employees = context.Employees
                .OrderBy(e => e.EmployeeId)
                .Select(e => new {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToArray();

            foreach (var e in employees) {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        // Problem 04
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context) {
             var employees = context.Employees
                .Where(e => e.Salary > 50000)
                .Select(e => new {
                    e.FirstName,
                    e.Salary
                })
                .OrderBy(e => e.FirstName)
                .ToArray();
            
            StringBuilder sb = new StringBuilder();
            foreach (var e in employees) {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        // Problem 05
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context) {
            var employees = context.Employees
                .Where(e => e.Department.Name == "Research and Development")
                .Select(e => new {
                    e.FirstName,
                    e.LastName,
                    DepartmentName = e.Department.Name,
                    e.Salary
                })
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FirstName)
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var e in employees) {
                sb.AppendLine($"{e.FirstName} {e.LastName} from {e.DepartmentName} - ${e.Salary:f2}");
            }
            return sb.ToString().TrimEnd();
        }

        // Problem 06
        public static string AddNewAddressToEmployee(SoftUniContext context) {
            var address = new Address {
                AddressText = "Vitoshka 15",
                TownId = 4
            };

            var employee = context.Employees
                .First(e => e.LastName == "Nakov");
            
            employee.Address = address;
            context.SaveChanges();

            var addresses = context.Employees
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .Select(e => e.Address.AddressText)
                .ToArray();
            
            return string.Join(Environment.NewLine, addresses);
        }
        
        // Problem 07
        public static string GetEmployeesInPeriod(SoftUniContext context) {
            var employees = context.Employees
                .Take(10)
                .Select(e => new {
                    e.FirstName,
                    e.LastName,
                    ManagerFirstName = e.Manager.FirstName,
                    ManagerLastName = e.Manager.LastName,
                    Projects = e.EmployeesProjects
                        .Where(ep => ep.Project.StartDate.Year >= 2001 && ep.Project.StartDate.Year <= 2003)
                        .Select(ep => new {
                            ep.Project.Name,
                            ep.Project.StartDate,
                            ep.Project.EndDate
                        })
                        .ToArray()
                })
                .ToArray();

             StringBuilder sb = new StringBuilder();
             foreach (var e in employees) {
                sb.AppendLine($"{e.FirstName} {e.LastName} - Manager: {e.ManagerFirstName} {e.ManagerLastName}");
                foreach (var p in e.Projects) {
                    var endDate = p.EndDate.HasValue 
                        ? p.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture) 
                        : "not finished";
                    var startDate = p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture);
                    sb.AppendLine($"--{p.Name} - {startDate} - {endDate}");
                }
             }
             return sb.ToString().TrimEnd();
        }

        // Problem 08
        public static string GetAddressesByTown(SoftUniContext context) {
            var addresses = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => new {
                    a.AddressText,
                    TownName = a.Town.Name,
                    EmployeeCount = a.Employees.Count
                })
                .ToArray();

             StringBuilder sb = new StringBuilder();
             foreach (var a in addresses) {
                 sb.AppendLine($"{a.AddressText}, {a.TownName} - {a.EmployeeCount} employees");
             }
             return sb.ToString().TrimEnd();
        }

        // Problem 09
        public static string GetEmployee147(SoftUniContext context) {
            var employee = context.Employees
                .Where(e => e.EmployeeId == 147)
                .Select(e => new {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    Projects = e.EmployeesProjects
                        .Select(ep => ep.Project.Name)
                        .OrderBy(n => n)
                        .ToArray()
                })
                .Single();
            
            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{employee.FirstName} {employee.LastName} - {employee.JobTitle}");
            foreach (var p in employee.Projects) {
                sb.AppendLine(p);
            }
            return sb.ToString().TrimEnd();
        }

        // Problem 10
        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context) {
            var departments = context.Departments
                .Where(d => d.Employees.Count > 5)
                .OrderBy(d => d.Employees.Count)
                .ThenBy(d => d.Name)
                .Select(d => new {
                    d.Name,
                    ManagerFirstName = d.Manager.FirstName,
                    ManagerLastName = d.Manager.LastName,
                    Employees = d.Employees
                        .OrderBy(e => e.FirstName)
                        .ThenBy(e => e.LastName)
                        .Select(e => new {
                            e.FirstName,
                            e.LastName,
                            e.JobTitle
                        })
                        .ToArray()
                })
                .ToArray();

            StringBuilder sb = new StringBuilder();
            foreach (var d in departments) {
                sb.AppendLine($"{d.Name} - {d.ManagerFirstName} {d.ManagerLastName}");
                foreach (var e in d.Employees) {
                    sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        // Problem 11
        public static string GetLatestProjects(SoftUniContext context) {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)
                .OrderBy(p => p.Name)
                .Select(p => new {
                    p.Name,
                    p.Description,
                    p.StartDate
                })
                .ToArray();
            
            StringBuilder sb = new StringBuilder();
            foreach (var p in projects) {
                sb.AppendLine(p.Name);
                sb.AppendLine(p.Description);
                sb.AppendLine(p.StartDate.ToString("M/d/yyyy h:mm:ss tt", CultureInfo.InvariantCulture));
            }
            return sb.ToString().TrimEnd();
        }

        // Problem 12
        public static string IncreaseSalaries(SoftUniContext context) {
            var departments = new string[] { "Engineering", "Tool Design", "Marketing", "Information Services" };
            var employees = context.Employees
                .Where(e => departments.Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .ToArray(); // Materialize to update
            
            foreach (var e in employees) {
                e.Salary *= 1.12m;
            }
            context.SaveChanges();

            StringBuilder sb = new StringBuilder();
            foreach (var e in employees) {
                sb.AppendLine($"{e.FirstName} {e.LastName} (${e.Salary:f2})");
            }
            return sb.ToString().TrimEnd();
        }

        // Problem 13
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context) {
             var employees = context.Employees
                .Where(e => EF.Functions.Like(e.FirstName, "Sa%"))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => new {
                    e.FirstName,
                    e.LastName,
                    e.JobTitle,
                    e.Salary
                })
                .ToArray();
            
            StringBuilder sb = new StringBuilder();
            foreach (var e in employees) {
                sb.AppendLine($"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})");
            }
            return sb.ToString().TrimEnd();
        }

        // Problem 14
        public static string DeleteProjectById(SoftUniContext context) {
            var project = context.Projects.Find(2);
            var employeesProjects = context.EmployeesProjects
                .Where(ep => ep.ProjectId == 2);
            
            context.EmployeesProjects.RemoveRange(employeesProjects);
            context.Projects.Remove(project);
            context.SaveChanges();

            var projects = context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToArray();
            
            return string.Join(Environment.NewLine, projects);
        }

        // Problem 15
        public static string RemoveTown(SoftUniContext context) {
            var town = context.Towns
                .Include(t => t.Addresses)
                .FirstOrDefault(t => t.Name == "Seattle");
            
            int count = 0;
            if (town != null) {
                var addresses = town.Addresses;
                var addressIds = addresses.Select(a => a.AddressId).ToArray();
                var employees = context.Employees
                    .Where(e => e.AddressId.HasValue && addressIds.Contains(e.AddressId.Value))
                    .ToArray();
                
                foreach (var e in employees) {
                    e.AddressId = null;
                }

                count = addresses.Count;
                context.Addresses.RemoveRange(addresses);
                context.Towns.Remove(town);
                context.SaveChanges();
            }

            return $"{count} addresses in Seattle were deleted";
        }
    }
}