using SmartFactory.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    /// <summary>
    /// ProductionResultView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProductionResultView : UserControl
    {
        public ProductionResultView(ProductionResultViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
