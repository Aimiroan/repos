using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace WpfApp1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<KeyValuePair<Image, bool>> problems = new List<KeyValuePair<Image, bool>>();
        bool viewSwitched = false;

        int solutionCount = 0;

        public MainWindow()
        {
            InitializeComponent();
            InitializeProblems();
            InitializeTable();

            foreach (KeyValuePair<Image, bool> problem in problems)
            {
                if (problem.Key == mainImage2 && problem.Value == false)
                {
                    but3.Visibility = Visibility.Hidden;
                }
                else if (problem.Key == mainImage2 && problem.Value == true)
                {
                    but3.Visibility = Visibility.Visible;
                }
            }

            SetAllHidden();
            mainImage2.Visibility = Visibility.Hidden;
            mainImage3.Visibility = Visibility.Hidden;
            mainImage2Border.Visibility = Visibility.Hidden;
            mainImage3Border.Visibility = Visibility.Hidden;
        }

        protected void InitializeProblems()
        {
            problems.Add(new KeyValuePair<Image, bool>(inletValveA, true));
            problems.Add(new KeyValuePair<Image, bool>(inletValveB, true));
            problems.Add(new KeyValuePair<Image, bool>(outletValveA, true));
            problems.Add(new KeyValuePair<Image, bool>(outletValveB, true));
            problems.Add(new KeyValuePair<Image, bool>(primSealA, true));
            problems.Add(new KeyValuePair<Image, bool>(primSealB, true));
            problems.Add(new KeyValuePair<Image, bool>(notSure, true));
            problems.Add(new KeyValuePair<Image, bool>(notSure2, true));
            problems.Add(new KeyValuePair<Image, bool>(notSure3, true));
            problems.Add(new KeyValuePair<Image, bool>(mainImage2, true));
            problems.Add(new KeyValuePair<Image, bool>(mainImage3, false));
        }

        protected void InitializeTable()
        {
            CheckSolutionCount();

            FlowDocument flowDoc = new FlowDocument();
            Table table = new Table();

            docReader.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            docReader.Document = flowDoc;
            flowDoc.Blocks.Add(table);
            table.CellSpacing = 10;
            table.Background = Brushes.White;
            int numberOfColumns = 3;

            for (int x = 0; x < numberOfColumns; x++)
            {
                table.Columns.Add(new TableColumn());

                if (x % 2 == 0)
                    table.Columns[x].Background = Brushes.Beige;
                else
                    table.Columns[x].Background = Brushes.LightSteelBlue;
            }

            table.RowGroups.Add(new TableRowGroup());
            for (int x = 0; x < solutionCount; x++)
            {
                table.RowGroups[0].Rows.Add(new TableRow());
            }

            TableRow currentRow = table.RowGroups[0].Rows[0];
            currentRow.Background = Brushes.Silver;
            currentRow.FontSize = 24;
            currentRow.FontWeight = FontWeights.Bold;

            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Order"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Advice"))));
            currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Location"))));

            table.RowGroups[0].Rows.Add(new TableRow());
            currentRow = table.RowGroups[0].Rows[1];
            currentRow.FontSize = 18;
            for (int x = 0; x < solutionCount; x++)
            {
                int y = x + 1;
                currentRow = table.RowGroups[0].Rows[y];
                currentRow.Cells.Add(new TableCell(new Paragraph(new Run(y.ToString()))));
            }
        }

        private void But1_Click(object sender, RoutedEventArgs e)
        {
            SetAllHidden();
        }

        private void But2_Click(object sender, RoutedEventArgs e)
        {
            ShowProblems();
            GetNewMessage();
        }

        private void But3_Click(object sender, RoutedEventArgs e)
        {
            if (viewSwitched == false)
            {
                SetAllHidden();
                mainImage.Visibility = Visibility.Hidden;
                mainImage2.Visibility = Visibility.Visible;
                mainImage2Border.Visibility = Visibility.Visible;
                mainImage3.Visibility = Visibility.Visible;
                mainImage3Border.Visibility = Visibility.Visible;
                viewSwitched = true;
            }
            else
            {
                ShowProblems();
                mainImage.Visibility = Visibility.Visible;
                mainImage2.Visibility = Visibility.Hidden;
                mainImage2Border.Visibility = Visibility.Hidden;
                mainImage3.Visibility = Visibility.Hidden;
                mainImage3Border.Visibility = Visibility.Hidden;
                viewSwitched = false;
            }
        }

        protected void SetAllHidden()
        {
            if (!viewSwitched)
            {
                foreach (KeyValuePair<Image, bool> img in problems)
                {
                    img.Key.Visibility = Visibility.Hidden;
                }
            }
        }

        protected void ShowProblems()
        {
            if (!viewSwitched)
            {
                foreach (KeyValuePair<Image, bool> img in problems)
                {
                    if (img.Value == true && img.Key != mainImage3)
                        img.Key.Visibility = Visibility.Visible;
                }
            }
        }

        protected void CheckSolutionCount()
        {
            // Return the amount of solutions
            solutionCount = 4;
        }

        [DllImport("DiagnosticsDLL.dll")]
        public static extern bool HasNewMessage();

        [DllImport("DiagnosticsDLL.dll")]
        public static extern IntPtr GetLastMessage();

        private string GetNewMessage()
        {
            IntPtr lastMessage = GetLastMessage();
            Console.WriteLine(Marshal.PtrToStringAnsi(lastMessage));
            return Marshal.PtrToStringAnsi(lastMessage);
        }
    }
}
