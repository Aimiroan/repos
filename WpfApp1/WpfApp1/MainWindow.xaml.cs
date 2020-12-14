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
        IntPtr lastMessage;
        int reason = 0;

        int solutionCount = 0;

        int percentileA = 70;
        int percentileB = 70;

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

        /*
         * A method to initialize the images into the created list.
         */
        protected void InitializeProblems()
        {
            problems.Add(new KeyValuePair<Image, bool>(inletValveA, false));
            problems.Add(new KeyValuePair<Image, bool>(inletValveB, false));
            problems.Add(new KeyValuePair<Image, bool>(outletValveA, false));
            problems.Add(new KeyValuePair<Image, bool>(outletValveB, false));
            problems.Add(new KeyValuePair<Image, bool>(primSealA, false));
            problems.Add(new KeyValuePair<Image, bool>(primSealB, false));
            problems.Add(new KeyValuePair<Image, bool>(notSure, false));
            problems.Add(new KeyValuePair<Image, bool>(notSure2, false));
            problems.Add(new KeyValuePair<Image, bool>(notSure3, false));
            problems.Add(new KeyValuePair<Image, bool>(mainImage2, false));
            problems.Add(new KeyValuePair<Image, bool>(mainImage3, false));
        }

        /*
         * A method to initialize the table rows and columns.
         */
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
            SetProblems();
            ShowProblems();
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

        /*
         * A method to show the problems which are situated in the pump.
         */
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

        /*
         * A method to set the solution count for initializing the table.
         */
        protected void CheckSolutionCount()
        {
            solutionCount = 4;
        }

        /*
         * A method to see which problems are active within the pump.
         */
        protected void SetProblems()
        {
            InitializeProblems();

            string message = GetNewMessage();

            string a = message;
            string b = string.Empty;

            for (int i = 0; i < a.Length; i++)
            {
                if (char.IsDigit(a[i]))
                    b += a[i];
            }

            if (b.Length > 0)
                reason = int.Parse(b);
            if (b.Length == 0)
                reason = 0;

            switch (reason)
            {
                case 0:
                    problems.Clear();
                    InitializeProblems();
                    break;

                case 1:
                    if (percentileA >= 70)
                        problems.Add(new KeyValuePair<Image, bool>(primSealA, true));

                    if (percentileB >= 70)
                        problems.Add(new KeyValuePair<Image, bool>(primSealB, true));
                    break;

                case 2:
                    if (percentileA >= 70)
                        problems.Add(new KeyValuePair<Image, bool>(outletValveA, true));

                    if (percentileB >= 70)
                        problems.Add(new KeyValuePair<Image, bool>(outletValveB, true));
                    break;

                case 3:
                    if (percentileA >= 70)
                        problems.Add(new KeyValuePair<Image, bool>(inletValveA, true));

                    if (percentileB >= 70)
                        problems.Add(new KeyValuePair<Image, bool>(inletValveB, true));
                    break;

                case 5:
                    if (percentileA >= 70)
                        problems.Add(new KeyValuePair<Image, bool>(notSure, true));
                    break;

                case 6:
                    if (percentileA >= 70)
                        problems.Add(new KeyValuePair<Image, bool>(notSure2, true));
                    break;

                case 7:
                    if (percentileA >= 70)
                        problems.Add(new KeyValuePair<Image, bool>(notSure3, true));
                    break;
            }
        }

        [DllImport("DiagnosticsDLL.dll")]
        public static extern bool HasNewMessage();

        [DllImport("DiagnosticsDLL.dll")]
        public static extern IntPtr GetLastMessage();

        /*
         * A method to retrieve the latest message if there is one available.
         */
        private string GetNewMessage()
        {
            string message;

            if (HasNewMessage())
            {
                lastMessage = GetLastMessage();
                message = Marshal.PtrToStringAnsi(lastMessage);
            }
            else
            {
                message = "DLLMain has no reason";
            }

            return message;
        }
    }
}
