using Microsoft.EntityFrameworkCore;
using RedisDemo3.Entity;
using System.Collections.Generic;

namespace RedisDemo3.DBContext
{
    public class DbContextClass : DbContext
    {
        public DbContextClass(DbContextOptions<DbContextClass> options) : base(options) { }

        public DbSet<Product> Products { get; set; }
    }
}
