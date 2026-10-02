using SmartFactory.ViewModels;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    public partial class WorkOrderView : UserControl
    {
        public WorkOrderView(WorkOrderViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
