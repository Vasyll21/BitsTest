using BitsOrchestra.Data;
using BitsOrchestra.Models;
using CsvHelper;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace BitsOrchestra.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors]
    public class EmployeesController : Controller
    {
        private readonly EmployeesAPIDBContext dbContext;

        public EmployeesController(EmployeesAPIDBContext dbContext) 
        { 
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetEmployees()
        {
            return Ok(await dbContext.Employees.ToListAsync());
        }

        [HttpGet]
        [Route("{id:guid}")]
        public async Task<IActionResult> GetEmployee([FromRoute] Guid id)
        {
            var employee = await dbContext.Employees.FindAsync(id);

            if (employee == null)
            {
                return NotFound();
            }

            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> AddEmployee(IFormFile file, [FromServices] Microsoft.AspNetCore.Hosting.IHostingEnvironment hostingEnvironment)
        {
            string fileName = $"{hostingEnvironment.WebRootPath}\\files\\{file.FileName}";
            using(FileStream fileStream = System.IO.File.Create(fileName))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            var employees = GetEmployeeList(fileName);

            await dbContext.Employees.AddRangeAsync(employees);
            await dbContext.SaveChangesAsync();

            return Ok(employees);
        }

        [HttpPut]
        [Route("{id:guid}")]
        public async Task<IActionResult> UpdateEmployee([FromRoute] Guid id, UpdateEmployeeRequestModel updateEmployeeRequestModel)
        {
            var employee = await dbContext.Employees.FindAsync(id);

            if(employee != null) 
            {
                employee.Name = updateEmployeeRequestModel.Name;
                employee.Salary = updateEmployeeRequestModel.Salary;
                employee.Married = updateEmployeeRequestModel.Married;
                employee.DateOfBirth = updateEmployeeRequestModel.DateOfBirth;
                employee.Phone = updateEmployeeRequestModel.Phone;

                await dbContext.SaveChangesAsync();

                return Ok(employee);
            }

            return NotFound();
        }

        [HttpDelete]
        [Route("{id:guid}")]
        public async Task<IActionResult> DeleteEmployee([FromRoute] Guid id)
        {
            var employee = await dbContext.Employees.FindAsync(id);

            if(employee != null)
            {
                dbContext.Remove(employee);
                await dbContext.SaveChangesAsync();

                return Ok(employee);
            }

            return NotFound();
        }

        private List<Employees> GetEmployeeList(string fileName)
        {
            List<Employees> employees = new List<Employees>();

            var path = $"{Directory.GetCurrentDirectory()}{@"\wwwroot\files"}" + "\\" + fileName;

            using(var reader = new StreamReader(path))
            {
                using(var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                {
                    csv.Read();
                    csv.ReadHeader();
                    
                    while (csv.Read())
                    {
                        var csvEmployee = csv.GetRecord<CsvEmployeeModel>();
                        var employee = new Employees()
                        {
                            Id = Guid.NewGuid(),
                            DateOfBirth = csvEmployee.DateOfBirth,
                            Married = csvEmployee.Married,
                            Phone = csvEmployee.Phone,
                            Salary = csvEmployee.Salary,
                            Name = csvEmployee.Name
                        };

                        employees.Add(employee);
                    }
                }
            }

            return employees;
        }
    }
}
