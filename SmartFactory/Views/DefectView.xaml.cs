using SmartFactory.ViewModels;
using System.Windows.Controls;

namespace SmartFactory.Views
{
    public partial class DefectView : UserControl
    {
        public DefectView(DefectViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
