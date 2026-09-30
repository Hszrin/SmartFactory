using SmartFactory.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    public partial class MachineView : UserControl
    {
        private readonly MachineViewModel _viewModel;

        public MachineView(MachineViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = viewModel;
        }
    }
}