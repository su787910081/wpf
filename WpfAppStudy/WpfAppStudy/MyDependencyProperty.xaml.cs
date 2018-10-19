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
    /// MyDependencyProperty.xaml 的交互逻辑
    /// </summary>
    public partial class MyDependencyProperty : UserControl
    {
        public MyDependencyProperty()
        {
            InitializeComponent();
        }

        // 1、声明依赖属性变量
        public static readonly DependencyProperty MyColorProperty;

        // 2、在属性系统中进行注册
    }
}
