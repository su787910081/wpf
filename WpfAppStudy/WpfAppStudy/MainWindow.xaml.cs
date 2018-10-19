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

namespace WpfAppStudy
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ButtonGridDemo_Click(object sender, RoutedEventArgs e)
        {
            DemoGrid dg = new DemoGrid();
            dg.Show();
        }

        private void ButtonGridDemoSplitter_Click(object sender, RoutedEventArgs e)
        {
            DemoGridSplitter dgp = new DemoGridSplitter();
            dgp.Show();
        }

        private void ButtonDependNormal_Click(object sender, RoutedEventArgs e)
        {
            DemoDependNormal dd = new DemoDependNormal();
            dd.Show();
        }

        private void ButtonDependUserDefine_Click(object sender, RoutedEventArgs e)
        {
            DemoDependUserDefine dd = new DemoDependUserDefine();
            dd.Show();
        }
    }
}
