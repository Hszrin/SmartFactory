using SmartFactory.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    public partial class DefectView : UserControl
    {
        private readonly DefectViewModel _viewModel;

        public DefectView(DefectViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = viewModel;
        }
    }
}