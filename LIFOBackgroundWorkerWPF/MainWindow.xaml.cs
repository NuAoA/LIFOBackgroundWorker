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

namespace LIFOBackgroundWorkerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        ViewModel vm;
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            vm =  new ViewModel();
            this.DataContext = vm;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            vm.AddStackItem();
        }

        private void Button_Click_start(object sender, RoutedEventArgs e)
        {
            vm.start();
        }

        private void Button_Click_finish(object sender, RoutedEventArgs e)
        {
            vm.stop();
        }

        private void Button_Click_clear(object sender, RoutedEventArgs e)
        {
            vm.clear();
        }
    }
}
