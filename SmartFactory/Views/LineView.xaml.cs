using SmartFactory.ViewModels;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    public partial class LineView : UserControl
    {
        public LineView(LineViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
