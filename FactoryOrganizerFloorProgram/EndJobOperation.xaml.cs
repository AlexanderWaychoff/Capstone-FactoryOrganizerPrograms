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
    /// Interaction logic for EndJobOperation.xaml
    /// </summary>
    public partial class EndJobOperation : Window
    {
        DatabaseControl databaseControl = new DatabaseControl();

        StoreEntry retrieveJobProduction = new StoreEntry();
        StoreEntry cellProductionEntry = new StoreEntry();

        ObservableCollection<StoreEntry> scrapCodes;

        List<int> allOperations = new List<int>();

        int storeNumber;
        int amountCompleted;
        int operationNumber;

        bool amountIsValid = false;

        public EndJobOperation()
        {
            InitializeComponent();

            scrapReasonList.ItemsSource = scrapCodes = new ObservableCollection<StoreEntry>();

            RetrieveScrapCodes();
        }

        private void EmployeeNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(EmployeeNumber.Text, out storeNumber))
                {
                    retrieveJobProduction.EmployeeNumber = storeNumber;
                    ProductCode.IsEnabled = true;
                    EmployeeNumber.IsEnabled = false;
                }
                else
                {
                    MessageBox.Show("The employee number entered isn't valid.", "Invalid Employee Number");
                }
            }
        }

        private void ProductCode_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                retrieveJobProduction = databaseControl.CheckSingleRowForJob("JobProductions", "ReportCode", ProductCode.Text.ToString());
                retrieveJobProduction.EmployeeNumber = storeNumber;
                //retrieveJobProduction = databaseControl.CheckSingleRowForCellRun(retrieveJobProduction, "CellProductions", "ItemNumber", ItemNumber.Text);
                try
                {
                    allOperations = retrieveJobProduction.RequiredOperations.Split(',').Select(Int32.Parse).ToList();
                    if (retrieveJobProduction.ReportCode == null)
                    {
                        MessageBox.Show("The product code entered isn't registered in the system for production.  Make sure the product code is correct or contact a supervisor if this problem persists.", "Invalid Product Code");
                    }
                    else
                    {
                        ProductCode.IsEnabled = false;
                        OperationNumber.IsEnabled = true;
                    }
                }
                catch
                {
                    MessageBox.Show("An error occured.  Try your last action again or contact your supervisor if the problem persists.", "Error");
                }
            }
        }

        private void OperationNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if(int.TryParse(OperationNumber.Text, out operationNumber))
                {
                    if(retrieveJobProduction.Operation != operationNumber)
                    {
                        MessageBox.Show("The operation entered doesn't match the required operation: " + retrieveJobProduction.Operation + ".\n\n  The amount required for operation " + retrieveJobProduction.Operation + " is: " + (retrieveJobProduction.TotalOrder - retrieveJobProduction.AmountCompleted).ToString() + " parts.");
                    }
                    else
                    {
                        OperationNumber.IsEnabled = false;
                        AmountCompleted.IsEnabled = true;
                    }
                }
                else
                {
                    MessageBox.Show("The operation entered isn't valid.", "Operation Not Valid");
                }
            }
        }

        private void AmountCompleted_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(AmountCompleted.Text, out amountCompleted))
                {
                    if (amountCompleted > retrieveJobProduction.TotalOrder - retrieveJobProduction.AmountCompleted)
                    {
                        MessageBox.Show("The amount entered is greater than the amount needed for production: \n\n" + (retrieveJobProduction.TotalOrder - retrieveJobProduction.AmountCompleted).ToString() + " parts. \n\n Make sure the amount is correct or contact a supervisor if this problem persists.", "Overran Production");
                    }
                    else
                    {
                        retrieveJobProduction.AmountCompleted += amountCompleted;
                        amountIsValid = true;
                        ScrapAmount.IsEnabled = true;
                        scrapReasonList.IsEnabled = true;
                        AmountCompleted.IsEnabled = false;
                    }
                }
                else
                {
                    MessageBox.Show("The amount entered is not valid.  Enter a number equal to or less than the remaining amount for the operation.", "Invalid Amount");
                }
            }
        }

        private void RetrieveScrapCodes()
        {
            List<StoreEntry> scrapList = databaseControl.RetrieveScrapCodes();
            foreach (StoreEntry scrapCode in scrapList)
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
                saveEntry = retrieveJobProduction;
                if (int.TryParse(ScrapAmount.Text, out storeNumber))
                {
                    saveEntry.ScrapAmount = storeNumber;
                }
                else
                {
                    saveEntry.ScrapAmount = 0;
                }
                if (scrapReasonList.Text != null && storeNumber > 0)
                {
                    saveEntry.ScrapReason = scrapReasonList.Text;
                }
                else
                {
                    saveEntry.ScrapReason = "";
                }

                //saveEntry.AmountCompleted = retrieveJobProduction.AmountCompleted + amountCompleted;
                saveEntry.TimeOfReporting = DateTime.Now;

                //databaseControl.ChangeSingleValue("CellProductions", "TotalPieces", saveEntry.TotalOrder.ToString(), "ItemNumber", saveEntry.ItemNumber.ToString());
                databaseControl.ChangeSingleValue("JobProductions", "ReportedPieces", saveEntry.AmountCompleted.ToString(), "ReportCode", saveEntry.ReportCode.ToString());

                //if reportedpieces == totalpieces, remove operation
                if(saveEntry.AmountCompleted == saveEntry.TotalOrder)
                {
                    allOperations.Remove(operationNumber);
                    int newRequiredOperation = allOperations[0];
                    string allRequiredOperationsLeftEntry = string.Join(",", allOperations);
                    int reportedPieces = 0;
                    databaseControl.ChangeSingleValue("JobProductions", "RequiredOperation", "\'" + allRequiredOperationsLeftEntry + "\'", "ReportCode", ProductCode.Text);
                    databaseControl.ChangeSingleValueOperation("JobProductions", "Operation", newRequiredOperation, "ReportCode", ProductCode.Text);
                    databaseControl.ChangeSingleValueOperation("JobProductions", "ReportedPieces", reportedPieces, "ReportCode", ProductCode.Text);
                }
                saveEntry.AmountCompleted = amountCompleted;
                databaseControl.SubmitJobEntry(saveEntry);
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
