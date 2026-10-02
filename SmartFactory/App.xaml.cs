using Microsoft.Extensions.DependencyInjection;
using SmartFactory.Data;
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
            Services.GetRequiredService<MainWindow>().Show();

            base.OnStartup(e);
        }

        protected override void OnExit(ExitEventArgs e)
        {
            Services.Dispose();
            base.OnExit(e);
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<SmartFactoryDbContext>();

            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IProductionPlanRepository, ProductionPlanRepository>();
            services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
            services.AddScoped<IProductionResultRepository, ProductionResultRepository>();
            services.AddScoped<IMachineRepository, MachineRepository>();
            services.AddScoped<ILineRepository, LineRepository>();
            services.AddScoped<IDefectRepository, DefectRepository>();
            services.AddScoped(typeof(ICommonRepository<,>), typeof(CommonRepository<,>));

            services.AddScoped<IProductionService, ProductionService>();
            services.AddScoped<IDefectService, DefectService>();
            services.AddScoped<IDashboardService, DashboardService>();

            services.AddScoped<ProductViewModel>();
            services.AddScoped<ProductionPlanViewModel>();
            services.AddScoped<WorkOrderViewModel>();
            services.AddScoped<ProductionResultViewModel>();
            services.AddScoped<DefectViewModel>();
            services.AddScoped<MachineViewModel>();
            services.AddScoped<LineViewModel>();
            services.AddScoped<DashboardViewModel>();

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
