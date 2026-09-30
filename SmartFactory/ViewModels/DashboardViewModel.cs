using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using SmartFactory.Dtos;
using SmartFactory.Services.Interface;
using System.Collections.ObjectModel;
using SmartFactory.Enums;

namespace SmartFactory.ViewModels
{
    public partial class DashboardViewModel : BaseViewModel ,IAsyncInitializable
    {
        private readonly IDashboardService _dashboardService;

        [ObservableProperty]
        private DashboardSummaryDto _summary = new();

        public ObservableCollection<MachineProductionDto> MachineProductions { get; } = new();
        public ObservableCollection<ProductProductionDto> ProductProductions { get; } = new();
        public ObservableCollection<DefectTypeDto> DefectTypes { get; } = new();
        public ObservableCollection<WorkOrderProgressDto> WorkOrderProgresses { get; } = new();


        [ObservableProperty]
        private ISeries[] productSeries = [];

        [ObservableProperty]
        private Axis[] productXAxes = [];
        [ObservableProperty]
        private Axis[] productYAxes = [];

        [ObservableProperty]
        private ISeries[] defectSeries = [];

        [ObservableProperty]
        public ISeries[] machineSeries = [];

        [ObservableProperty]
        private Axis[] machineXAxes = [];
        [ObservableProperty]
        private Axis[] machineYAxes = [];

        [ObservableProperty]
        private bool isMachineChartMode = true;

        [ObservableProperty]
        private bool isProductChartMode = true;

        [ObservableProperty]
        private bool isDefectChartMode = true;

        public bool HasMachineData => MachineProductions.Count > 0;
        public bool HasProductData => ProductProductions.Count > 0;
        public bool HasDefectData => DefectTypes.Count > 0;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string errorMessage = string.Empty;

        [ObservableProperty]
        private DashboardPeriod selectedPeriod = DashboardPeriod.Today;
        public SolidColorPaint LegendTextPaint { get; } =
            new SolidColorPaint(SKColors.White);

        public DashboardViewModel(IDashboardService dashboardService)
        {
            _dashboardService = dashboardService;
        }

        public override async Task InitializeAsync(CancellationToken token)
        {
            try
            {
                IsLoading = true;
                ErrorMessage = string.Empty;

                var (start, end) = GetDateRange();

                Summary = await _dashboardService
                    .GetSummaryAsync(start, end, token);

                await LoadCollectionAsync(
                    () => _dashboardService
                        .GetMachineProductionAsync(start, end, token),
                    MachineProductions);

                await LoadCollectionAsync(
                    () => _dashboardService
                        .GetProductProductionAsync(start, end, token),
                    ProductProductions);

                await LoadCollectionAsync(
                    () => _dashboardService
                        .GetDefectProductionAsync(start, end, token),
                    DefectTypes);

                await LoadCollectionAsync(
                    () => _dashboardService
                        .GetWorkOrderProgressAsync(token),
                    WorkOrderProgresses);

                token.ThrowIfCancellationRequested();

                CreateMachineChart();
                CreateProductChart();
                CreateDefectChart();

                OnPropertyChanged(nameof(HasMachineData));
                OnPropertyChanged(nameof(HasProductData));
                OnPropertyChanged(nameof(HasDefectData));
            }
            catch (OperationCanceledException)
            {
                // 화면 이동 때문에 정상적으로 취소된 것
                // 에러 메시지 띄우지 않음
                throw;
            }
            catch (Exception ex)
            {
                ErrorMessage =
                    $"대시보드 조회 중 오류가 발생했습니다.\n{ex.Message}";

                MachineProductions.Clear();
                ProductProductions.Clear();
                DefectTypes.Clear();
                WorkOrderProgresses.Clear();

                Summary = new DashboardSummaryDto();

                OnPropertyChanged(nameof(HasMachineData));
                OnPropertyChanged(nameof(HasProductData));
                OnPropertyChanged(nameof(HasDefectData));
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task Refresh()
        {
            await InitializeAsync(CancellationToken.None);
        }
        [RelayCommand]
        private void ChangeMachineMode(string mode)
        {
            if (bool.TryParse(mode, out bool isChart))
                IsMachineChartMode = isChart;
        }
        [RelayCommand]
        private void ChangeProductMode(string mode)
        {
            if (bool.TryParse(mode, out bool isChart))
                IsProductChartMode = isChart;
        }

        [RelayCommand]
        private void ChangeDefectMode(string mode)
        {
            if (bool.TryParse(mode, out bool isChart))
                IsDefectChartMode = isChart;
        }

        [RelayCommand]
        private async Task ChangePeriod(string period)
        {
            if (!Enum.TryParse<DashboardPeriod>(period, out var parsed))
                return;

            SelectedPeriod = parsed;
            await InitializeAsync(CancellationToken.None);
        }
        private (DateTime start, DateTime end) GetDateRange()
        {
            var today = DateTime.Today;

            return SelectedPeriod switch
            {
                DashboardPeriod.Today =>
                    (today, today.AddDays(1)),

                DashboardPeriod.Last7Days =>
                    (today.AddDays(-6), today.AddDays(1)),

                DashboardPeriod.ThisMonth =>
                    (new DateTime(today.Year, today.Month, 1),
                     new DateTime(today.Year, today.Month, 1).AddMonths(1)),

                _ =>
                    (today, today.AddDays(1))
            };
        }
        private void CreateMachineChart()
        {
            MachineSeries =
            [
                new ColumnSeries<int>
                {
                    Name = "생산량",
                    Values = MachineProductions
                        .Select(x => x.ProductionQuantity)
                        .ToArray()
                }
            ];

            MachineXAxes =
            [
                new Axis
                {
                    Labels = MachineProductions
                        .Select(x => x.MachineName)
                        .ToArray(),

                    LabelsPaint = new SolidColorPaint(SKColors.White)
                }
            ];

            MachineYAxes =
            [
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.White)
                }
            ];
        }
        private void CreateProductChart()
        {
            ProductSeries =
            [
                new ColumnSeries<int>
                {
                   Name = "생산량",
                    Values = ProductProductions
                        .Select(x => x.ProductionQuantity)
                        .ToArray()
                }
            ];

            ProductXAxes =
            [
                new Axis
                {
                    Labels = ProductProductions
                        .Select(x => x.ProductName)
                        .ToArray(),

                        LabelsPaint = new SolidColorPaint(SKColors.White)
                }
            ];

            ProductYAxes =
            [
                new Axis
                {
                    LabelsPaint = new SolidColorPaint(SKColors.White)
                }
            ];
        }

        private void CreateDefectChart()
        {
            DefectSeries = DefectTypes
                .Select(x => (ISeries)new PieSeries<int>
                {
                    Name = x.DefectType,
                    Values = [x.Quantity]
                })
                .ToArray();
        }
    }
}