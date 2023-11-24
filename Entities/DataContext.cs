using ExamAPI.Entities;
using Microsoft.EntityFrameworkCore;

namespace ExamAPI.Entities
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<Orders> Orders { get; set; }
    }
}