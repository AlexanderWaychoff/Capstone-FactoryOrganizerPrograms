using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace FactoryOrganizerFloorProgram
{
    /// <summary>
    /// Interaction logic for EndCellRun.xaml
    /// </summary>
    public partial class EndCellRun : Window
    {
        DatabaseControl databaseControl = new DatabaseControl();

        StoreEntry cellEntry = new StoreEntry();
        StoreEntry cellProductionEntry = new StoreEntry();

        ObservableCollection<StoreEntry> scrapCodes;

        List<int> allEmployees = new List<int>();

        int storeNumber;
        int employeeNumber;

        bool amountIsValid = false;

        public EndCellRun()
        {
            InitializeComponent();

            scrapReasonList.ItemsSource = scrapCodes = new ObservableCollection<StoreEntry>();

            RetrieveScrapCodes();
        }

        private void CellNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(CellNumber.Text, out storeNumber))
                {
                    cellEntry = databaseControl.CheckSingleRow("AllCells", "CellNumber", storeNumber);

                    if (cellEntry.CellNumber == null)
                    {
                        MessageBox.Show("The cell number entered isn't registered in the system.  Make sure the cell number is correct or contact a supervisor if this problem persists.", "Invalid Cell Number");
                    }
                    else if (cellEntry.IsCellActive == false)
                    {
                        MessageBox.Show("This cell is offline.  To work in this cell, choose start cell from main menu.", "Invalid Cell Number");
                    }
                    else
                    {
                        ItemNumber.IsEnabled = true;
                        CellNumber.IsEnabled = false;
                    }
                }
            }
        }

        private void ItemNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                cellEntry = databaseControl.CheckSingleRowForCellRun(cellEntry, "CellProductions", "ItemNumber", ItemNumber.Text);

                if (cellEntry.ItemNumber == null)
                {
                    MessageBox.Show("The item number entered isn't registered in the system for production.  Make sure the item number is correct or contact a supervisor if this problem persists.", "Invalid Item Number");
                }
                else
                {
                    ItemNumber.IsEnabled = false;
                    AmountCompleted.IsEnabled = true;
                }
            }
        }

        private void AmountCompleted_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(AmountCompleted.Text, out storeNumber))
                {
                    if (storeNumber > cellEntry.TotalOrder)
                    {
                        MessageBox.Show("The amount entered is greater than the amount needed for production.  Make sure the amount is correct or contact a supervisor if this problem persists.", "Overran Production");
                    }
                    else
                    {
                        amountIsValid = true;
                        ScrapAmount.IsEnabled = true;
                        scrapReasonList.IsEnabled = true;
                        AmountCompleted.IsEnabled = false;
                    }
                }
                else
                {
                    MessageBox.Show("The amount entered is not valid.  Enter a number equal to or less than the order amount.", "Invalid Amount");
                }
            }
        }

        private void RetrieveScrapCodes()
        {
            List<StoreEntry> scrapList = databaseControl.RetrieveScrapCodes();
            foreach(StoreEntry scrapCode in scrapList)
            {
                scrapCodes.Add(scrapCode);
            }

        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if (amountIsValid)
            {
                storeNumber = 0;
                StoreEntry saveEntry = new StoreEntry();
                saveEntry = cellEntry;
                saveEntry.AmountCompleted = Convert.ToInt32(AmountCompleted.Text);
                if (int.TryParse(ScrapAmount.Text, out storeNumber))
                {
                    saveEntry.ScrapAmount = storeNumber;
                }
                else
                {
                    saveEntry.ScrapAmount = 0;
                }
                if(scrapReasonList.Text != null && storeNumber > 0)
                {
                    saveEntry.ScrapReason = scrapReasonList.Text;
                }
                else
                {
                    saveEntry.ScrapReason = "";
                }

                saveEntry.TotalOrder = cellEntry.TotalOrder - saveEntry.AmountCompleted;
                saveEntry.TimeOfReporting = DateTime.Now;

                databaseControl.ChangeSingleValue("CellProductions", "TotalPieces", saveEntry.TotalOrder.ToString(), "ItemNumber", saveEntry.ItemNumber.ToString());
                //Reported Pieces won't work on multiple entries, add value to cellEntry above
                databaseControl.ChangeSingleValue("CellProductions", "ReportedPieces", saveEntry.AmountCompleted.ToString(), "ItemNumber", saveEntry.ItemNumber.ToString());
                databaseControl.SubmitCellEntry(saveEntry);
                MessageBox.Show("Submission successful.", "Submission Saved");
                Close();
            }
            else
            {
                MessageBox.Show("Some required fields above do not have any values, or the values are incorrect.  Fill in all fields.", "Not Valid for Submission");
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
