using SoftUni.Data;
using SoftUni.Models;
using System.Globalization;
using System.Text;
using System.Xml.Linq;

namespace SoftUni
{
    public class StartUp
    {
        public static void Main(string[] args)
        {
            var context = new SoftUniContext();

            //3.Employees Full Information

            //Console.WriteLine(GetEmployeesFullInformation(context));

            //4.Employees with Salary Over 50 000

            //Console.WriteLine(GetEmployeesWithSalaryOver50000(context));

            //5.Employees from Research and Development

            //Console.WriteLine(GetEmployeesFromResearchAndDevelopment(context));

            //6.Adding a New Address and Updating Employee

            //Console.WriteLine(AddNewAddressToEmployee(context));

            //7.Employees and Projects

            //Console.WriteLine(GetEmployeesInPeriod(context));

            //8.Addresses by Town

            //Console.WriteLine(GetAddressesByTown(context));

            //9.Employee 147

            //Console.WriteLine(GetEmployee147(context));

            //10.Departments with More Than 5 Employees

            //Console.WriteLine(GetDepartmentsWithMoreThan5Employees(context));

            //11.Find Latest 10 Projects

            //Console.WriteLine(GetLatestProjects(context));

            //12.Increase Salaries

            //Console.WriteLine(IncreaseSalaries(context));

            //13.Find Employees by First Name Starting with "Sa"

            //Console.WriteLine(GetEmployeesByFirstNameStartingWithSa(context));

            //14.Delete Project by Id

            //Console.WriteLine(DeleteProjectById(context));

            //15.Remove Town

            //Console.WriteLine(RemoveTown(context));
        }

        //3.Employees Full Information
        public static string GetEmployeesFullInformation(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.LastName,
                    e.MiddleName,
                    e.JobTitle,
                    e.Salary
                })
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FirstName} {e.LastName} {e.MiddleName} {e.JobTitle} {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //4.Employees with Salary Over 50 000
        public static string GetEmployeesWithSalaryOver50000(SoftUniContext context)
        {
            var wellPaidEmployees = context.Employees
                .Select(e => new
                {
                    e.FirstName,
                    e.Salary
                })
                .Where(s => s.Salary > 50000)
                .OrderBy(e => e.FirstName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in wellPaidEmployees)
            {
                sb.AppendLine($"{e.FirstName} - {e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //5.Employees from Research and Development
        public static string GetEmployeesFromResearchAndDevelopment(SoftUniContext context)
        {
            var employees = context.Employees
                .Select(e => new 
                {
                    FullName = e.FirstName + " " + e.LastName,
                    e.Salary,
                    e.Department
                }) 
                .Where(e => e.Department.Name == "Research and Development")
                .OrderBy(e => e.Salary)
                .ThenByDescending(e => e.FullName)
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.FullName} from {e.Department.Name} - ${e.Salary:f2}");
            }

            return sb.ToString().TrimEnd();
        }

        //6.Adding a New Address and Updating Employee
        public static string AddNewAddressToEmployee(SoftUniContext context)
        {
            Address address = new Address();
            address.AddressText = "Vitoshka 15";
            address.TownId = 4;

            context.Add(address);
            context.SaveChanges();

            var searchedEmployee = context.Employees
                .Where(e => e.LastName == "Nakov")
                .FirstOrDefault();

            searchedEmployee.Address = address;
            context.SaveChanges();

            var employees = context.Employees
                .Select(e => new
                {
                    e.AddressId,
                    e.Address
                })
                .OrderByDescending(e => e.AddressId)
                .Take(10)
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in employees)
            {
                sb.AppendLine($"{e.Address.AddressText}");
            }

            return sb.ToString().TrimEnd();
        }

        //7.Employees and Projects

        public static string GetEmployeesInPeriod(SoftUniContext context)
        {
            var result = context.Employees
                .Take(10)
                .Select(e => new
                {
                   EmployeesName = $"{e.FirstName} {e.LastName}",
                   ManagerNames = $"{e.Manager.FirstName} {e.Manager.LastName}",
                   Projects = e.EmployeesProjects
                    .Where(ep => 
                        ep.Project.StartDate.Year >= 2001 && 
                        ep.Project.StartDate.Year <= 2003)
                      .Select(ep => new
                      {
                          ProjectName = ep.Project.Name,
                          ep.Project.StartDate,
                          EndDate = ep.Project.EndDate.HasValue
                            ? ep.Project.EndDate.Value.ToString("M/d/yyyy h:mm:ss tt")
                            : "not finished"
                      })
                })
                
                .ToList();

            var sb = new StringBuilder();

            foreach (var e in result)
            {
                sb.AppendLine($"{e.EmployeesName} - Manager: {e.ManagerNames}");

               if (e.Projects.Any())
               {
                    foreach (var p in e.Projects)
                    {
                        sb.AppendLine($"--{p.ProjectName} - {p.StartDate:M/d/yyyy h:mm:ss tt} - {p.EndDate}");
                    }
                }
            }

            return sb.ToString().TrimEnd();
        }

        //8.Addresses by Town

        public static string GetAddressesByTown(SoftUniContext context)
        {
            string[] addressesInfo = context.Addresses
                .OrderByDescending(a => a.Employees.Count)
                .ThenBy(a => a.Town.Name)
                .ThenBy(a => a.AddressText)
                .Take(10)
                .Select(a => $"{a.AddressText}, {a.Town.Name} - {a.Employees.Count} employees")
                .ToArray();

            return string.Join(Environment.NewLine, addressesInfo);
        }

        //9.Employee 147

        public static string GetEmployee147(SoftUniContext context)
        {
            var employee147Info = context.Employees
               .Where(e => e.EmployeeId == 147)
               .Select(x => new
               {
                   x.FirstName,
                   x.LastName,
                   x.JobTitle,
                   Projects = x.EmployeesProjects.Select(p => new { p.Project.Name }).OrderBy(p => p.Name).ToArray()
               })
               .FirstOrDefault();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"{employee147Info.FirstName} {employee147Info.LastName} - {employee147Info.JobTitle}");
            sb.Append(string.Join(Environment.NewLine, employee147Info.Projects.Select(p => p.Name)));

            return sb.ToString().TrimEnd();
        }

        //10.Departments with More Than 5 Employees

        public static string GetDepartmentsWithMoreThan5Employees(SoftUniContext context)
        {
            var result = context.Departments
              .Where(d => d.Employees.Count > 5)
              .OrderBy(d => d.Employees.Count)
              .ThenBy(d => d.Name)
              .Select(d => new
              {
                  DepartmentName = d.Name,
                  ManagerNames = $"{d.Manager.FirstName} {d.Manager.LastName}",
                  Employees = d.Employees
                  .OrderBy(e => e.FirstName)
                  .ThenBy(e => e.LastName)
                  .Select(e => new
                  {
                      EmployeeInfo = $"{e.FirstName} {e.LastName} - {e.JobTitle}",
                  })
                  .ToList()
              })
              .ToList();
                
            var sb = new StringBuilder();

            foreach ( var r in result )
            {
                sb.AppendLine($"{r.DepartmentName} - {r.ManagerNames}");
                sb.Append(string.Join (Environment.NewLine, r.Employees.Select(e => e.EmployeeInfo)));
            }
            return sb.ToString().TrimEnd();
        }

        //11.Find Latest 10 Projects

        public static string GetLatestProjects(SoftUniContext context)
        {
            var projects = context.Projects
                .OrderByDescending(p => p.StartDate)
                .Take(10)               
                .OrderBy(p => p.Name)
                .Select(p => new
                {
                    p.Name,
                    p.Description,
                    p.StartDate
                })
                .ToList();


            var sb = new StringBuilder();
            foreach ( var p in projects )
            {
                sb.AppendLine($"{p.Name}");
                sb.AppendLine($"{p.Description}");
                sb.AppendLine($"{p.StartDate:M/d/yyyy h:mm:ss tt}");
            }

            return sb.ToString().TrimEnd();
        }

        //12.Increase Salaries

        public static string IncreaseSalaries(SoftUniContext context)
        {
            decimal salaryModifier = 1.12m;
            string[] departments = new[] {"Engineering", "Tool Design", "Marketing", "Information Services"};

            var employeesForSalaryIncrease = context.Employees
                .Where(e => departments.Contains(e.Department.Name));

            foreach (var e in employeesForSalaryIncrease)
            {
                e.Salary *= salaryModifier;
            }

            context.SaveChanges();


            string[] emplyeesInfoText = context.Employees
                .Where(e => departments.Contains(e.Department.Name))
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} (${e.Salary:f2})")
                .ToArray();

            return string.Join(Environment.NewLine, emplyeesInfoText);
        }

        //13.Find Employees by First Name Starting with "Sa"
        public static string GetEmployeesByFirstNameStartingWithSa(SoftUniContext context)
        {
            var employees = context.Employees
                .Where(e => e.FirstName.Substring(0, 2).ToLower() == "sa")
                .OrderBy(e => e.FirstName)
                .ThenBy(e => e.LastName)
                .Select(e => $"{e.FirstName} {e.LastName} - {e.JobTitle} - (${e.Salary:f2})")
                .ToList();

            return string.Join(Environment.NewLine, employees);
        }

        //14.Delete Project by Id
        public static string DeleteProjectById(SoftUniContext context)
        {
            var project = context.Projects.Find(2);


            var employeesProjectToDelete = context.EmployeesProjects
                .Where(ep => ep.ProjectId == 2);

            context.EmployeesProjects.RemoveRange(employeesProjectToDelete);

            context.Projects.Remove(project);

            context.SaveChanges();

            var projects = context.Projects
                .Take(10)
                .Select(p => p.Name)
                .ToList();



            return string.Join (Environment.NewLine, projects);
        }

        //15.Remove Town

        public static string RemoveTown(SoftUniContext context)
        {
            Town townToDelete = context.Towns
                .Where(t => t.Name == "Seattle")
                .FirstOrDefault();

            Address[] addressesToDelete = context.Addresses
                .Where(a => a.TownId == townToDelete.TownId)
                .ToArray();

            Employee[] employeesToRemoveAddressFrom = context.Employees
                .Where(e => addressesToDelete
                .Contains(e.Address))
                .ToArray();

            foreach (Employee e in employeesToRemoveAddressFrom)
            {
                e.AddressId = null;
            }

            context.Addresses.RemoveRange(addressesToDelete);
            context.Towns.Remove(townToDelete);
            context.SaveChanges();

            return $"{addressesToDelete.Count()} addresses in Seattle were deleted";
        }
    } 
}
