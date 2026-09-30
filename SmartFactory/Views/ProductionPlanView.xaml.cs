using SmartFactory.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    /// <summary>
    /// ProductionPlanView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProductionPlanView : UserControl
    {
        public ProductionPlanView(
            ProductionPlanViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
