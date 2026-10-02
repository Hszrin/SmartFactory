using Microsoft.EntityFrameworkCore;
using SmartFactory.Models;

namespace SmartFactory.Data
{
    public class SmartFactoryDbContext : DbContext
    {
        public DbSet<Product> Products => Set<Product>();
        public DbSet<ProductionPlan> ProductionPlans => Set<ProductionPlan>();
        public DbSet<ProductionLine> ProductionLines => Set<ProductionLine>();
        public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
        public DbSet<ProductionResult> ProductionResults => Set<ProductionResult>();
        public DbSet<Machine> Machines => Set<Machine>();
        public DbSet<Defect> Defects => Set<Defect>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured)
                return;

            optionsBuilder.UseSqlServer(
                @"Server=.\SQLEXPRESS;Database=SmartFactoryDB;Trusted_Connection=True;TrustServerCertificate=True;Connect Timeout=5;");
        }
    }
}
