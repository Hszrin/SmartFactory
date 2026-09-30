using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using SmartFactory.Models;

namespace SmartFactory.Data
{
    public class SmartFactoryDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductionPlan> ProductionPlans { get; set; }
        public DbSet<ProductionLine> ProductionLines { get; set; }
        public DbSet<WorkOrder> WorkOrders { get; set; }
        public DbSet<ProductionResult> ProductionResults { get; set; }
        public DbSet<Machine> Machines { get; set; }
        public DbSet<Defect> Defects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(
                @"Server=DESKTOP-D2NJDUA\SQLEXPRESS;" +
                "Database=SmartFactoryDB;" +
                "Trusted_Connection=True;" +
                "TrustServerCertificate=True;" +
                "Connect Timeout=5;");
        }
    }

}