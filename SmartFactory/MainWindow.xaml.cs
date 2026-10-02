using Microsoft.Extensions.DependencyInjection;
using SmartFactory.ViewModels;
using SmartFactory.Views;
using System.Windows;

namespace SmartFactory
{
    public partial class MainWindow : Window
    {
        private IServiceScope? _currentScope;
        private CancellationTokenSource? _navigationCts;

        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
            Closed += MainWindow_Closed;
        }

        private async void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            Loaded -= MainWindow_Loaded;
            await NavigateAsync<DashboardView, DashboardViewModel>();
        }

        private async Task NavigateAsync<TView, TViewModel>()
            where TView : FrameworkElement
            where TViewModel : class, IAsyncInitializable
        {
            // 새 메뉴를 선택하면 이전 화면의 미완료 조회만 취소한다.
            _navigationCts?.Cancel();

            var cts = new CancellationTokenSource();
            var token = cts.Token;
            _navigationCts = cts;

            var newScope = App.Services.CreateScope();

            try
            {
                var view = newScope.ServiceProvider.GetRequiredService<TView>();
                var viewModel = newScope.ServiceProvider.GetRequiredService<TViewModel>();

                await viewModel.InitializeAsync(token);
                token.ThrowIfCancellationRequested();

                if (_navigationCts != cts)
                {
                    newScope.Dispose();
                    return;
                }

                // 새 화면의 로딩이 끝난 뒤 교체하여 기존 DbContext의 수명을 보장한다.
                var oldScope = _currentScope;
                _currentScope = newScope;
                MainContent.Content = view;
                oldScope?.Dispose();
            }
            catch (OperationCanceledException)
            {
                newScope.Dispose();
            }
            catch (Exception ex)
            {
                newScope.Dispose();
                MessageBox.Show(
                    $"화면을 불러오는 중 오류가 발생했습니다.\n{ex.Message}",
                    "오류",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
            finally
            {
                if (_navigationCts == cts)
                    _navigationCts = null;

                cts.Dispose();
            }
        }

        private async void ProductButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync<ProductView, ProductViewModel>();
        }

        private async void ProductionPlanButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync<ProductionPlanView, ProductionPlanViewModel>();
        }

        private async void WorkOrderButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync<WorkOrderView, WorkOrderViewModel>();
        }

        private async void ProductionResultButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync<ProductionResultView, ProductionResultViewModel>();
        }

        private async void DefectButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync<DefectView, DefectViewModel>();
        }

        private async void MachineButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync<MachineView, MachineViewModel>();
        }

        private async void LineButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync<LineView, LineViewModel>();
        }

        private async void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            await NavigateAsync<DashboardView, DashboardViewModel>();
        }

        private void MainWindow_Closed(object? sender, EventArgs e)
        {
            _navigationCts?.Cancel();
            _currentScope?.Dispose();
        }
    }
}
