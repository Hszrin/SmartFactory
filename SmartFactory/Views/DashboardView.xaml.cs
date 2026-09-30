using SmartFactory.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    /// <summary>
    /// DashBoard.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class DashboardView : UserControl
    {
        private readonly DashboardViewModel _viewModel;

        public DashboardView(DashboardViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = viewModel;
        }
    }
}
