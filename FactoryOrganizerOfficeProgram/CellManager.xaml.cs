using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
    /// Interaction logic for CellManager.xaml
    /// </summary>
    public partial class CellManager : Window
    {

        public ObservableCollection<ProductOperation> ProductOperations { get; set; }
        public ObservableCollection<FileName> AllCustomers { get; set; }
        public ObservableCollection<ProductProductionCode> AllProductsForCustomer { get; set; }
        public ObservableCollection<ConcatenateString> BaseInformation { get; set; }

        public CustomerInformation Customer;
        public CustomerInformation UserSubmittedCustomer;
        public ProductProductionCode UserSubmittedProductID = new ProductProductionCode();
        public FolderNames allFolderNames = new FolderNames();
        FileName customerFolderName;

        string csvName = "temporary";

        string basePathForTemporaryFolder;
        //base folder Customers
        string customerFolder = "Customers";
        string unassignedProductsFolder = "Unassigned Products";
        string temporaryFolder = "Temporary Create Holder";

        string[] folders;

        public CellManager()
        {
            InitializeComponent();

            lstMachineFunctions.ItemsSource = ProductOperations = new ObservableCollection<ProductOperation>();
            CustomerList.ItemsSource = AllCustomers = new ObservableCollection<FileName>();

            basePathForTemporaryFolder = @".\" + customerFolder + @"\" + temporaryFolder;

            ExternalFile.CheckForDirectory(customerFolder);
            ExternalFile.CheckForDirectory(customerFolder + @"\" + unassignedProductsFolder);
            ExternalFile.CheckForDirectory(customerFolder + @"\" + temporaryFolder);
            RetrieveAllCustomers();
        }

        private void RetrieveAllCustomers()
        {
            AllCustomers.Clear();
            folders = ExternalFile.RetrieveAllFolderNamesInDirectory(allFolderNames.CustomersFolder);
            foreach (string folder in folders)
            {
                customerFolderName = new FileName();
                customerFolderName.Name = folder;
                if (folder != allFolderNames.TemporaryFolder && folder != allFolderNames.UnassignedProductsFolder)
                {
                    AllCustomers.Add(customerFolderName);
                }
            }
        }

        private void NewCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = new Customer();
            customer.ShowDialog();
            RetrieveAllCustomers();
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            ProductOperations = new ObservableCollection<ProductOperation>(from i in ProductOperations orderby i.Operation select i);

            lstMachineFunctions.ItemsSource = ProductOperations;
        }

        private void OnDeleteMachineFunction(object sender, RoutedEventArgs e)
        {
            int indexOfProductOperations = ProductOperations.ToList().FindIndex(x => x == ((sender as FrameworkElement).DataContext as ProductOperation));
            string operationNumber = ProductOperations[indexOfProductOperations].Operation.ToString();
            ProductOperations.Remove((sender as FrameworkElement).DataContext as ProductOperation);
            if(ProductOperations.Count == 0)
            {
                CustomerList.IsEnabled = true;
                everyCustomerCell.IsEnabled = true;
            }
        }

        private void OnAddMachineFunction(object sender, RoutedEventArgs e)
        {
            if ((CustomerList.Text == "" || CustomerList.Text == null))
            {
                MessageBox.Show("No Customer is selected.  Select one from the drop down list or click new to add a Customer.  A cell must be assigned to a customer.", "No Customer Selected");
            }
            //else if CEllNumber == null
            else if (everyCustomerCell.Text == "" || everyCustomerCell.Text == null)
            {
                MessageBox.Show("No cell is entered.  Type a new cell into the Cell box (dropdown menu contains cells already associated with selected customer).  A cell must be assigned to a customer.", "No Cell Entered");
            }
            else
            {
                CustomerList.IsEnabled = false;
                everyCustomerCell.IsEnabled = false;
                ProductOperations.Add(new ProductOperation());
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            string detailSetName = CustomerList.Text;
            bool HasNullValue = CheckOperationNumberForNullValue();

            if (!HasNullValue)
            {
                //if (DetailSetExists)
                //{
                    bool changesWereMade = true;//CompareDuplicateAndProductDetails();

                    if (changesWereMade)
                    {
                        if (MessageBox.Show("New operation " + everyCustomerCell.Text + " will be added for " + CustomerList.Text + ".  Proceed?", "New Cell: " + everyCustomerCell.Text, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                        {
                            //do no stuff
                        }
                        else
                        {
                            SaveDetailsToCSV();
                        }
                    }
                    else
                    {
                        //MessageBox.Show("No changes are being made to '" + DetailSet.Text + "'.  Save canceled.", "Nothing to Save.");
                    }
                //}
                //else
                //{
                //    SaveDetailsToCSV();
                //    MessageBox.Show("Your details have been saved as '" + detailSetName + "'.  If you would like to edit these in the future select this detail set from the Product Requirement's Detail Set dropbox and load it.", "Product Detail Information Saved");
                //}
            }
            else
            {
                MessageBox.Show("At least one Operation field is empty.  Please enter a value in that field before saving or remove it.", "Empty Operation Value");
            }
        }

        private bool CheckOperationNumberForNullValue()
        {
            bool hasNullOperation = false;
            for (int i = 0; i < ProductOperations.Count; i++)
            {
                int j;
                if (!int.TryParse(ProductOperations[i].Operation.ToString(), out j) || ProductOperations[i].Operation == 0)
                {
                    hasNullOperation = true;
                }
            }
            return hasNullOperation;
        }

        private void SaveDetailsToCSV()
        {
            var csv = new StringBuilder();
            var sortedDetails = ProductOperations.OrderBy(x => x.Operation);
            sortedDetails.ToList();

            ExternalFile.CheckForDirectory(allFolderNames.CustomersFolder + @"\" + CustomerList.Text + @"\" + allFolderNames.CellsFolder);

            foreach (ProductOperation operation in sortedDetails)
            {
                var customer = CustomerList.Text;
                var operationNumber = operation.Operation;
                var description = "-";
                if (operation.Description != "")
                {
                    description = operation.Description;
                }
                var cycleTime = "null";
                float j;
                if(operation.CycleTime != null && float.TryParse(operation.CycleTime.ToString(), out j))
                {
                    cycleTime = operation.CycleTime.ToString();
                }
                var requiredToReport = "true";
                if(operation.RequiredToReport == false)
                {
                    requiredToReport = "false";
                }
                var newLine = string.Format("{0},{1},{2},{3},{4}", customer, operationNumber, description, cycleTime, requiredToReport);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(allFolderNames.CustomersFolder + @"\" + CustomerList.Text + @"\" + allFolderNames.CellsFolder + @"\" + everyCustomerCell.Text + ".csv", csv.ToString());
        }

        //private bool CompareDuplicateAndProductDetails()
        //{
        //    bool changesWereMade = false;
        //    if (ProductDetails.Count.Equals(DuplicateProductDetailsToVerifyChanges.Count))
        //    {
        //        for (int i = 0; i < ProductDetails.Count; i++)
        //        {
        //            if (ProductDetails[i].Detail.Equals(DuplicateProductDetailsToVerifyChanges[i].Detail) && ProductDetails[i].DescriptionOfDetail.Equals(DuplicateProductDetailsToVerifyChanges[i].DescriptionOfDetail))
        //            {

        //            }
        //            else
        //            {
        //                changesWereMade = true;
        //                i += ProductDetails.Count;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        changesWereMade = true;
        //    }
        //    return changesWereMade;
        //}

        //nice to have: save confirmation if changes are made

        //protected override void OnClosing(CancelEventArgs e)
        //{
        //    base.OnClosing(e);
        //    bool changesWereMade = CompareDuplicateAndProductDetails();

        //    if (changesWereMade)
        //    {
        //        if (MessageBox.Show("Current changes affect the Detail Set '" + DetailSet.Text + "'.  Closing now won't save these changes.  Proceed?", "Unsaved Changes to " + DetailSet.Text, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
        //        {
        //            //do nothing
        //        }
        //        else
        //        {
        //            isConfirmedToClose = true;
        //        }
        //    }
        //    else
        //    {
        //        isConfirmedToClose = true;
        //    }
        //    if (!isConfirmedToClose)
        //    {
        //        e.Cancel = true;
        //    }
        //}

    }
}
