using Microsoft.Extensions.DependencyInjection;
using SmartFactory.Data;
using SmartFactory.Models;
using SmartFactory.Repositories;
using SmartFactory.Repositories.Interface;
using SmartFactory.Services;
using SmartFactory.Services.Interface;
using SmartFactory.ViewModels;
using SmartFactory.Views;
using System.Windows;

namespace SmartFactory
{
    public partial class App : Application
    {
        public static ServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            ConfigureServices(services);

            Services = services.BuildServiceProvider();

            var mainWindow =
                Services.GetRequiredService<MainWindow>();

            mainWindow.Show();

            base.OnStartup(e);
        }

        private static void ConfigureServices(
            IServiceCollection services)
        {
            // DbContext
            services.AddScoped<SmartFactoryDbContext>();

            // =========================
            // Repository
            // =========================

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductionPlanRepository, ProductionPlanRepository>();
            services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
            services.AddScoped<IProductionResultRepository, ProductionResultRepository>();
            services.AddScoped<IMachineRepository, MachineRepository>();
            services.AddScoped<ILineRepository, LineRepository>();
            services.AddScoped<IDefectRepository, DefectRepository>(); 
            services.AddScoped<IMachineRepository, MachineRepository>();
            // Generic Repository
            services.AddScoped(
                typeof(ICommonRepository<,>),
                typeof(CommonRepository<,>));

            services.AddScoped<IProductionService, ProductionService>();
            services.AddScoped<IDefectService, DefectService>();
            services.AddScoped<IDashboardService, DashboardService>();

            // =========================
            // ViewModel
            // =========================

            services.AddScoped<ProductViewModel>();
            services.AddScoped<ProductionPlanViewModel>();
            services.AddScoped<WorkOrderViewModel>();
            services.AddScoped<ProductionResultViewModel>();
            services.AddScoped<DefectViewModel>();
            services.AddScoped<MachineViewModel>();
            services.AddScoped<LineViewModel>();
            services.AddScoped<DashboardViewModel>();

            // =========================
            // View
            // =========================

            services.AddScoped<ProductView>();
            services.AddScoped<ProductionPlanView>();
            services.AddScoped<WorkOrderView>();
            services.AddScoped<ProductionResultView>();
            services.AddScoped<DefectView>();
            services.AddScoped<MachineView>();
            services.AddScoped<LineView>();
            services.AddScoped<DashboardView>();

            services.AddSingleton<MainWindow>();
        }
    }
}