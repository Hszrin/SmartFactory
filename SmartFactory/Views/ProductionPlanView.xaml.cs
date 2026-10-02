using SmartFactory.ViewModels;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    public partial class ProductionPlanView : UserControl
    {
        public ProductionPlanView(ProductionPlanViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
