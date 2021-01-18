using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Resources;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Microsoft.Win32;

namespace Diagnostics
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Dictionary<string, bool> problems = new Dictionary<string, bool>();
        FlowDocument flowDoc = new FlowDocument();
        Table table = new Table();
        bool viewSwitched = false;
        bool firstRun = true;
        IntPtr lastMessage;
        int reason = 0;
        int rowNumber = 1;
        bool newMessage = true;

        int solutionCount = 0;

        readonly int percentileA = 70;
        readonly int percentileB = 70;

        public MainWindow()
        {
            InitializeComponent();
            InitializeProblems();
            InitializeTable();

            foreach (KeyValuePair<string, bool> problem in problems)
            {
                if (problem.Key == mainImage2.Name && problem.Value == false)
                {
                    but3.Visibility = Visibility.Hidden;
                }
                else if (problem.Key == mainImage2.Name && problem.Value == true)
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
            problems.Add(inletValveA.Name, false);
            problems.Add(inletValveB.Name, false);
            problems.Add(outletValveA.Name, false);
            problems.Add(outletValveB.Name, false);
            problems.Add(primSealA.Name, false);
            problems.Add(primSealB.Name, false);
            problems.Add(pumpHead.Name, false);
            problems.Add(presDev.Name, false);
            problems.Add(presRip.Name, false);
            problems.Add(mainImage2.Name, false);
            problems.Add(mainImage3.Name, false);
        }

        /*
         * A method to initialize the table rows and columns.
         */
        protected void InitializeTable()
        {
            docReader.VerticalScrollBarVisibility = ScrollBarVisibility.Disabled;
            docReader.Document = flowDoc;

            flowDoc.Blocks.Clear();
            table.Columns.Clear();
            table.RowGroups.Clear();

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

            if (firstRun == true)
            {
                solutionCount = 1;
                firstRun = false;
            }
            else
            {
                solutionCount = problems.Count;

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

                solutionCount = 0;
            }
        }

        private void But1_Click(object sender, RoutedEventArgs e)
        {
            SetAllHidden();
            table.Columns.Clear();
            table.RowGroups.Clear();
        }

        private void But2_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetAllHidden();
                SetProblems();
                InitializeTable();
                ShowProblems();
            } catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void But3_Click(object sender, RoutedEventArgs e)
        {
            if (viewSwitched == false)
            {
                pumpHead.Visibility = Visibility.Hidden;

                //SetAllHidden();
                mainImage.Visibility = Visibility.Hidden;
                mainImage2.Visibility = Visibility.Visible;
                mainImage2Border.Visibility = Visibility.Visible;
                mainImage3.Visibility = Visibility.Visible;
                mainImage3Border.Visibility = Visibility.Visible;
                viewSwitched = true;
            }
            else
            {
                pumpHead.Visibility = Visibility.Visible;

                //ShowProblems();
                mainImage.Visibility = Visibility.Visible;
                mainImage2.Visibility = Visibility.Hidden;
                mainImage2Border.Visibility = Visibility.Hidden;
                mainImage3.Visibility = Visibility.Hidden;
                mainImage3Border.Visibility = Visibility.Hidden;
                viewSwitched = false;
            }
        }

        private void But4_Click(object sender, RoutedEventArgs e)
        {
            SaveProblem();
        }

        private void But5_Click(object sender, RoutedEventArgs e)
        {
            LoadProblem();
        }

        protected void SetAllHidden()
        {
            if (!viewSwitched)
            {
                foreach (KeyValuePair<string, bool> img in problems)
                {
                    Image image = (Image)FindName(img.Key);
                    image.Visibility = Visibility.Hidden;
                }
            }
        }

        /*
         * A method to show the problems which are situated in the pump.
         */
        public void ShowProblems()
        {
            if (!viewSwitched)
            {
                bool switchView = false;
                foreach (KeyValuePair<string, bool> img in problems)
                {
                    if (img.Value == true)
                    {
                        Image image = (Image)this.FindName(img.Key);
                        image.Visibility = Visibility.Visible;

                        if (img.Key == "mainImage2")
                        {
                            switchView = true;
                        }

                        char c = img.Key.Last();

                        string location = "";

                        if (c == 'A')
                            location = "A";
                        if (c == 'B')
                            location = "B";

                        addToTable(img.Key, location);
                    }
                }
                if (switchView == true)
                {
                    but3.Visibility = Visibility.Visible;
                } else
                {
                    but3.Visibility = Visibility.Hidden;
                }
                switchView = false;
            }
            rowNumber = 1;
        }

        /*
         * A method to see which problems are active within the pump.
         */
        public void SetProblems()
        {
            rowNumber = 1;

            string message = GetNewMessage();

            if (newMessage == true)
            {

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

                problems.Clear();

                string c = reason.ToString();

                for (int i = 0; i < c.Length; i++)
                {
                    reason = (int)char.GetNumericValue(c[i]);

                    switch (reason)
                    {
                        case 0:
                            problems.Clear();
                            InitializeProblems();
                            break;

                        case 1:
                            if (percentileA >= 70)
                            {
                                problems.Add(primSealA.Name, true);
                            }

                            if (percentileB >= 70)
                            {
                                problems.Add(primSealB.Name, true);
                            }
                            break;

                        case 2:
                            if (percentileA >= 70)
                            {
                                problems.Add(outletValveA.Name, true);
                            }

                            if (percentileB >= 70)
                            {
                                problems.Add(outletValveB.Name, true);
                            }
                            break;

                        case 3:
                            if (percentileA >= 70)
                            {
                                problems.Add(inletValveA.Name, true);
                            }

                            if (percentileB >= 70)
                            {
                                problems.Add(inletValveB.Name, true);
                            }
                            break;

                        case 5:
                            if (percentileA >= 70)
                            {
                                problems.Add(pumpHead.Name, true);
                                problems.Add(mainImage2.Name, true);
                            }
                            break;

                        case 6:
                            if (percentileA >= 70)
                            {
                                problems.Add(presDev.Name, true);
                            }
                            break;

                        case 7:
                            if (percentileA >= 70)
                            {
                                problems.Add(presRip.Name, true);
                            }
                            break;
                    }
                }
            }
        }

        private void addToTable(string problem, string location)
        {
            TableRow currentRow;

            switch (problem)
            {
                case "primSealA":
                case "primSealB":
                    currentRow = table.RowGroups[0].Rows[rowNumber];
                    currentRow.FontSize = 18;
                    if (location == "A")
                    {
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Replace primary seal"))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Pump A"))));
                    }

                    if (location == "B")
                    {
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Replace primary seal"))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Pump B"))));
                    }

                    rowNumber += 1;
                    break;

                case "outletValveA":
                case "outletValveB":
                    currentRow = table.RowGroups[0].Rows[rowNumber];
                    currentRow.FontSize = 18;
                    if (location == "A")
                    {
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Replace outlet valve"))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Pump A"))));
                    }

                    if (location == "B")
                    {
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Replace outlet valve"))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Pump B"))));
                    }
                    rowNumber += 1;
                    break;

                case "inletValveA":
                case "inletValveB":
                    currentRow = table.RowGroups[0].Rows[rowNumber];
                    currentRow.FontSize = 18;
                    if (location == "A")
                    {
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Replace inlet valve"))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Pump A"))));
                    }

                    if (location == "B")
                    {
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Replace inlet valve"))));
                        currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Pump B"))));
                    }
                    rowNumber += 1;
                    break;

                case "pumpHead":
                    currentRow = table.RowGroups[0].Rows[rowNumber];
                    currentRow.FontSize = 18;
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Replace pump head"))));
                    rowNumber += 1;
                    break;

                case "presDiv":
                    currentRow = table.RowGroups[0].Rows[rowNumber];
                    currentRow.FontSize = 18;
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Perform calibration"))));
                    rowNumber += 1;
                    break;

                case "presRip":
                    currentRow = table.RowGroups[0].Rows[rowNumber];
                    currentRow.FontSize = 18;
                    currentRow.Cells.Add(new TableCell(new Paragraph(new Run("Perform dynamic leak test"))));
                    rowNumber += 1;
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
        public string GetNewMessage()
        {
            string message;

            if (HasNewMessage())
            {
                lastMessage = GetLastMessage();
                message = Marshal.PtrToStringAnsi(lastMessage);
                newMessage = true;
            }
            else
            {
                message = "DLLMain has no reason";
                newMessage = false;
            }

            return message;
        }

        protected void SaveProblem()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "JSON file (*.json)|*.json"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    using (StreamWriter file = File.CreateText(saveFileDialog.FileName))
                    {
                        JsonSerializer serializer = new JsonSerializer();
                        serializer.Serialize(file, problems);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
        }

        protected void LoadProblem()
        {
            SetAllHidden();

            Dictionary<string, bool> objectOut = new Dictionary<string, bool>();

            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "JSON file (*.json)|*.json"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    var problems = File.ReadAllText(openFileDialog.FileName);
                    objectOut = JsonConvert.DeserializeObject<Dictionary<string, bool>>(problems);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                problems = objectOut;

                InitializeTable();
                ShowProblems();
            }
        }
    }
}
