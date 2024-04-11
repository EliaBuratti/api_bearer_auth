using Ex_api_DTO.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ex_api_DTO.Database
{
    public class FakeDatabase : DbContext
    {
        public FakeDatabase(DbContextOptions<FakeDatabase> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
