using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Test.Business.Entities;

namespace Test.Data.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=DbConcurrencyProj;Trusted_Connection=True;MultipleActiveResultSets=true");
        }

        

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Lock>().HasKey(l => new
            {
                l.EntityId,
                l.ObjectName
            });                         
        }

        public DbSet<Event> Events { get; set; }
        public DbSet<Lock> Locks { get; set; }
        

    }
}
