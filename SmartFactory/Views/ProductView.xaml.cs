using SmartFactory.ViewModels;
using System.Windows;
using System.Windows.Controls;
namespace SmartFactory.Views
{
    /// <summary>
    /// ProductView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProductView : UserControl
    {
        public ProductView(
            ProductViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
