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
        List<KeyValuePair<Image, bool>> problems = new List<KeyValuePair<Image, bool>>();
        bool view = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeProblems();

            var value = problems.Where(kvp => kvp.Key == mainImage2).Select(kvp => kvp.Value).ToString();
            Console.WriteLine("Test: " + value + " : Test end");

            SetAllHidden();
            mainImage2.Visibility = Visibility.Hidden;
        }

        protected void InitializeProblems()
        {
            problems.Add(new KeyValuePair<Image, bool>(inletValveA, false));
            problems.Add(new KeyValuePair<Image, bool>(inletValveB, true));
            problems.Add(new KeyValuePair<Image, bool>(outletValveA, false));
            problems.Add(new KeyValuePair<Image, bool>(outletValveB, false));
            problems.Add(new KeyValuePair<Image, bool>(primSealA, true));
            problems.Add(new KeyValuePair<Image, bool>(primSealB, true));
            problems.Add(new KeyValuePair<Image, bool>(notSure, true));
            problems.Add(new KeyValuePair<Image, bool>(mainImage2, true));
        }

        private void But1_Click(object sender, RoutedEventArgs e)
        {
            SetAllHidden();
        }

        private void But2_Click(object sender, RoutedEventArgs e)
        {
            ShowProblems();
        }
        
        private void But3_Click(object sender, RoutedEventArgs e)
        {
            if (view == false)
            {
                SetAllHidden();
                mainImage.Visibility = Visibility.Hidden;
                mainImage2.Visibility = Visibility.Visible;
                view = true;
            } else
            {
                ShowProblems();
                mainImage.Visibility = Visibility.Visible;
                mainImage2.Visibility = Visibility.Hidden;
                view = false;
            }
        }

        protected void SetAllHidden()
        {
            foreach (KeyValuePair<Image, bool> img in problems)
            {
                img.Key.Visibility = Visibility.Hidden;
            }
        }

        protected void ShowProblems()
        {
            foreach (KeyValuePair<Image, bool> img in problems)
            {
                if (img.Value == true & img.Key != mainImage2)
                    img.Key.Visibility = Visibility.Visible;
            }
        }
    }
}
