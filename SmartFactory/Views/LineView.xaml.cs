using SmartFactory.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    public partial class LineView : UserControl
    {
        private readonly LineViewModel _viewModel;

        public LineView(LineViewModel viewModel)
        {
            InitializeComponent();

            _viewModel = viewModel;
            DataContext = viewModel;
        }
    }
}