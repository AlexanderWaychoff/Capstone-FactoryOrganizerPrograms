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

namespace FactoryOrganizerOfficeProgram
{
    /// <summary>
    /// Interaction logic for ConfirmProduction.xaml
    /// </summary>
    public partial class ConfirmProduction : Window
    {
        public ObservableCollection<StashConfirmProduction> stashConfirmProduction;

        DateTime today;
        DatabaseControl databaseControl;
        CsvReader csvReader;
        FolderNames folderNames;
        public ConfirmProduction(DatabaseControl databaseController, CsvReader csvRead, FolderNames allFolders)
        {
            InitializeComponent();

            databaseControl = databaseController;
            csvReader = csvRead;
            folderNames = allFolders;

            confirmProductionList.ItemsSource = stashConfirmProduction = new ObservableCollection<StashConfirmProduction>();

            PopulateStash();
        }

        private void PopulateStash()
        {
            List<StashConfirmProduction> productsToConfirm;
            productsToConfirm = databaseControl.RetrieveProductsToConfirm();

            foreach(StashConfirmProduction product in productsToConfirm)
            {
                
                if (product.CellNumber == null || product.CellNumber == "")
                {
                    stashConfirmProduction.Add(product);
                }
                else
                {
                    product.ButtonVisibility = "Hidden";
                    stashConfirmProduction.Add(product);
                }

            }
        }

        private void ConfirmProduct_Click(object sender, RoutedEventArgs e)
        {
            StashConfirmProduction removedConfirmation = ((sender as FrameworkElement).DataContext as StashConfirmProduction);

            if (removedConfirmation.ButtonVisibility == "Hidden")
            {
                //yes/no confirm user wants to send to production
                databaseControl.SubmitCellJob(removedConfirmation);
                databaseControl.DeleteDefinedRow("ProductAwaitingConfirmations", "ProductAwaitingConfirmationID", removedConfirmation.ProductAwaitingConfirmationID);

                stashConfirmProduction.Remove((sender as FrameworkElement).DataContext as StashConfirmProduction);

                MessageBox.Show("Product " + removedConfirmation.ItemNumber + " sent out to cell " + removedConfirmation.CellNumber + " for production. ", "Cell Job sent for Production");
            }
            else
            {
                if(removedConfirmation.ReportCode == "" || removedConfirmation.ReportCode == null)
                {
                    MessageBox.Show("Report code cannot be empty on a job that doesn't go to a cell.  This is required for employees to enter pieces they have done.", "Report Code empty");
                }
                else
                {
                    string folderPath = folderNames.CustomersFolder + @"\" + removedConfirmation.Customer + @"\" + removedConfirmation.ItemNumber;
                    List<string> requiredOperations = csvReader.LoadRequiredOperationsCSV(folderPath, removedConfirmation.ItemNumber);
                    string firstRequiredOperation;
                    string allRequiredOperations = String.Join(",", requiredOperations);

                    try
                    {
                        firstRequiredOperation = requiredOperations[0];

                        today =  DateTime.Now;

                        removedConfirmation.Operation = Convert.ToInt32(firstRequiredOperation);
                        removedConfirmation.RequiredOperations = allRequiredOperations;
                        removedConfirmation.TimeOfReporting = today;
                        databaseControl.SubmitUnassignedJob(removedConfirmation);
                        databaseControl.DeleteDefinedRow("ProductAwaitingConfirmations", "ProductAwaitingConfirmationID", removedConfirmation.ProductAwaitingConfirmationID);

                        stashConfirmProduction.Remove((sender as FrameworkElement).DataContext as StashConfirmProduction);

                        MessageBox.Show("Product " + removedConfirmation.ItemNumber + " sent out for production.  Printable document is now available until entire job is completed.", "Job sent for Production");
                    }
                    catch
                    {
                        MessageBox.Show("The job you're trying to confirm doesn't have any required operations when it needs at least one.  Edit the operations for " + removedConfirmation.ItemNumber + ".", "No Required Operations");
                    }

                    
                    //yes/no confirm user wants to send to production
                    //make database entries
                }
            }
        }

        private void RandomizeCode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckFloorStatus_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
