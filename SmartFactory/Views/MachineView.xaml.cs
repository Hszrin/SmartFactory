using SmartFactory.ViewModels;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    public partial class MachineView : UserControl
    {
        public MachineView(MachineViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
