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

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        readonly SolidColorBrush b = new SolidColorBrush();
        Image mainImage = new Image();

        public MainWindow()
        {
            InitializeComponent();
        }

        private void But1_Click(object sender, RoutedEventArgs e)
        {
            b.Color = Color.FromRgb(0, 0, 0);
            Rec1.Fill = b;
        }

        private void But2_Click(object sender, RoutedEventArgs e)
        {
            b.Color = Color.FromRgb(255, 0, 0);
            Rec1.Fill = b;
        }
    }
}
