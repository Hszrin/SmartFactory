using SmartFactory.ViewModels;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    public partial class ProductView : UserControl
    {
        public ProductView(ProductViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
