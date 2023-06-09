using BitsOrchestra.Models;
using Microsoft.EntityFrameworkCore;

namespace BitsOrchestra.Data
{
    public class EmployeesAPIDBContext : DbContext
    {
        public EmployeesAPIDBContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Employees> Employees { get; set; }
    }
}
