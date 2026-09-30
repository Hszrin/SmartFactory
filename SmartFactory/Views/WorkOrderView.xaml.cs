using SmartFactory.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    /// <summary>
    /// WorkOrderView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class WorkOrderView : UserControl
    {
        public WorkOrderView(WorkOrderViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
