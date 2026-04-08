using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfApp2.Models;

namespace WpfApp2.Views
{
    public partial class GamePage : UserControl
    {
        public GamePage()
        {
            InitializeComponent();
        }

        private void Item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Item item)
            {
                var viewModel = DataContext as ViewModels.MainViewModel;
                if (viewModel?.UseItemCommand?.CanExecute(item) == true)
                {
                    viewModel.UseItemCommand.Execute(item);
                }
            }
        }
    }
}
