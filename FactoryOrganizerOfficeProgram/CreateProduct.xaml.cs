using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
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
    /// Interaction logic for CreateProduct.xaml
    /// </summary>
    public partial class CreateProduct : Window
    {
        public ObservableCollection<ProductOperation> ProductOperations { get; set; }
        public ObservableCollection<FileName> AllCustomers { get; set; }
        public ObservableCollection<FileName> AllProductsForCustomer { get; set; }
        public ObservableCollection<FileName> AllCells { get; set; }
        public ObservableCollection<ConcatenateString> BaseInformation { get; set; }

        public CustomerInformation Customer;
        public CustomerInformation UserSubmittedCustomer;
        public FileName UserSubmittedProductID = new FileName();
        public FolderNames allFolderNames = new FolderNames();
        FileName customerFolderName;
        DatabaseControl DatabaseControl;

        bool hasWebsiteImage = false;
        bool hasWebsiteDescription = false;
        string websiteFileString;

        int productWasAdded = 0;
        string saveProductID;

        string csvName = "temporary";

        string basePathForTemporaryFolder;
        string basePathForTemporaryFolder2;
        //base folder Customers
        string customerFolder = "Customers";
        string unassignedProductsFolder = "Unassigned Products";
        string temporaryFolder = "Temporary Create Holder";

        string[] folders;



        public CreateProduct(DatabaseControl databaseControl)
        {
            InitializeComponent();

            DatabaseControl = databaseControl;

            lstMachineFunctions.ItemsSource = ProductOperations = new ObservableCollection<ProductOperation>();
            customerList.ItemsSource = AllCustomers = new ObservableCollection<FileName>();
            customerProducts.ItemsSource = AllProductsForCustomer = new ObservableCollection<FileName>();
            baseInformation.ItemsSource = BaseInformation = new ObservableCollection<ConcatenateString>();
            everyCustomerCell.ItemsSource = AllCells = new ObservableCollection<FileName>();

            basePathForTemporaryFolder = @".\" + customerFolder + @"\" + temporaryFolder;
            basePathForTemporaryFolder2 = @"\" + customerFolder + @"\" + temporaryFolder;

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
                if (folder != allFolderNames.TemporaryFolder)
                {
                    AllCustomers.Add(customerFolderName);
                }
            }
        }
        
        private void RetrieveAllProducts()
        {
            AllProductsForCustomer.Clear();
            folders = ExternalFile.RetrieveAllFolderNamesInDirectory(allFolderNames.CustomersFolder + @"\" + customerList.Text);
            foreach (string folder in folders)
            {
                customerFolderName = new FileName();
                customerFolderName.Name = folder;
                if (folder != allFolderNames.TemporaryFolder && folder != "Cells")
                {
                    AllProductsForCustomer.Add(customerFolderName);
                }
            }
        }

        private void OnDeleteMachineFunction(object sender, RoutedEventArgs e)
        {
            int indexOfProductOperations = ProductOperations.ToList().FindIndex(x => x == ((sender as FrameworkElement).DataContext as ProductOperation));
            string operationNumber = ProductOperations[indexOfProductOperations].Operation.ToString();
            ProductOperations.Remove((sender as FrameworkElement).DataContext as ProductOperation);

            for (int n = filesForOperations.Items.Count - 1; n >= 0; --n)
            {
                if (filesForOperations.Items[n].ToString().Contains(operationNumber))
                {
                    filesForOperations.Items.RemoveAt(n);
                }
            }
            if(ProductOperations.Count == 0)
            {
                customerList.IsEnabled = true;
                everyCustomerCell.IsEnabled = true;
                customerProducts.IsEnabled = true;
                addProductOperation.Visibility = Visibility.Visible;
            }
        }

        private void OnAddMachineFunction(object sender, RoutedEventArgs e)
        {
            if (customerProducts.Text == "" || customerProducts.Text == null || customerProducts.Text == "-Enter New ID-")
            {
                MessageBox.Show("To make operations for a product, a new Product ID is required.  If this product is for a specific customer, Load Customer Presets first along with a Product ID that is new.  If this product will be made in a cell select it from the dropdown menu and click Populate Cell Template.", "No Product ID entered");
            }
            else
            {
                customerList.IsEnabled = false;
                everyCustomerCell.IsEnabled = false;
                customerProducts.IsEnabled = false;
                ProductOperations.Add(new ProductOperation());
            }
        }

        private void OnAddScaleUnit(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt;*.rtf)|*.txt;*rtf|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            ExternalFile.CheckForDirectory(basePathForTemporaryFolder + @"\" + allFolderNames.OperationDocumentationFolder);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    int indexOfSenderInProductOperations = ProductOperations.ToList().FindIndex(x => x == (sender as FrameworkElement).DataContext as ProductOperation);
                    filesForOperations.Items.Add("Operation: " + 
                        ProductOperations[indexOfSenderInProductOperations].Operation + 
                        "    Description: " +
                        ProductOperations[indexOfSenderInProductOperations].Description +
                        "    File: " +
                        System.IO.Path.GetFileName(filename));
                }
                int indexOfProductOperations = ProductOperations.ToList().FindIndex(x => x == ((sender as FrameworkElement).DataContext as ProductOperation));
                string operationNumber = ProductOperations[indexOfProductOperations].Operation.ToString();

                ExternalFile.CopyFile(openFileDialog, basePathForTemporaryFolder2 + @"\" + allFolderNames.OperationDocumentationFolder, "Operation" + operationNumber + ".txt");
            }
        }

        private void OnDeleteScaleUnit(object sender, RoutedEventArgs e)
        {
            var delScaleUnit = (sender as FrameworkElement).DataContext as ScaleUnit;

            var mf = ProductOperations.FirstOrDefault(_ => _.ScaleUnits.Contains(delScaleUnit));

            if (mf != null)
            {
                mf.ScaleUnits.Remove(delScaleUnit);

                foreach (var scaleUnit in mf.ScaleUnits)
                {
                    scaleUnit.Index = mf.ScaleUnits.IndexOf(scaleUnit);
                }
            }
        }

        private void Sort_Click(object sender, RoutedEventArgs e)
        {
            ProductOperations = new ObservableCollection<ProductOperation>(from i in ProductOperations orderby i.Operation select i);

            lstMachineFunctions.ItemsSource = ProductOperations;
        }

        private void Info_Click(object sender, RoutedEventArgs e)
        {
            var createHelpInformation = new HelpCreateProduct();
            createHelpInformation.ShowDialog();
        }

        //private void CustomerList_KeyDown(object sender, KeyEventArgs e)
        //{

        //    if (e.Key == Key.Return)
        //    {
        //        UserSubmittedCustomer = new CustomerInformation();
        //        string content = CustomerList.Text;
        //        UserSubmittedCustomer.Name = content;
        //        int indexTest = AllCustomers.IndexOf(UserSubmittedCustomer);
        //        if(indexTest == -1 && !AllCustomers.Contains(UserSubmittedCustomer))
        //        {
        //            AllCustomers.Add(UserSubmittedCustomer);
        //            CustomerList.ItemsSource = AllCustomers;
        //        }
        //    }
        //}

        private void ProductID_KeyDown(object sender, KeyEventArgs e)
        {
            if(productWasAdded == 1)
            {
                string content = customerProducts.Text;
                int index = AllProductsForCustomer.IndexOf(UserSubmittedProductID);
                AllProductsForCustomer[index].Name = content;
                //UserSubmittedProductID.ProductID = content;
                customerProducts.ItemsSource = AllProductsForCustomer;
            }
            if (e.Key == Key.Return)
            {
                string content = customerProducts.Text;
                UserSubmittedProductID.Name = content;
                int indexTest = AllProductsForCustomer.IndexOf(UserSubmittedProductID);
                if (indexTest == -1 && productWasAdded < 1)
                {
                    saveProductID = content;
                    AllProductsForCustomer.Add(UserSubmittedProductID);
                    customerProducts.ItemsSource = AllProductsForCustomer;
                    productWasAdded++;
                }
                else if(saveProductID == null)
                {
                    MessageBox.Show("The product you entered is already created.  If you want to make changes to this item go to 'Edit'.", "Duplicate Product ID");
                }
                else
                {
                    MessageBox.Show("You already created a Product ID (shown below).  Use the Product ID or restart the Create menu: \n\n" + saveProductID,"Product ID already created");
                }
            }
        }

        private void SubmitFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SubmitWebsiteDescription_Click(object sender, RoutedEventArgs e)
        {
            ExternalFile.CheckForDirectory(allFolderNames.CustomersFolder + @"\" + allFolderNames.TemporaryFolder + @"\" + allFolderNames.WebsiteFolder);

            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt;*.rtf)|*.txt;*rtf";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                hasWebsiteImage = true;
                foreach (string filename in openFileDialog.FileNames)
                {
                    websiteFileString = System.IO.Path.GetFileName(filename);
                    filesForOperations.Items.Add("Website Image: " +
                        System.IO.Path.GetFileName(filename));
                }
                string websiteDescriptionName = "Description";
                ExternalFile.CopyFile(openFileDialog, basePathForTemporaryFolder2 + @"\" + allFolderNames.WebsiteFolder, websiteDescriptionName);
            }
        }

        private void SubmitImage_Click(object sender, RoutedEventArgs e)
        {
            ExternalFile.CheckForDirectory(allFolderNames.CustomersFolder + @"\" + allFolderNames.TemporaryFolder + @"\" + allFolderNames.WebsiteFolder);
            if (hasWebsiteImage)
            {
                ExternalFile.RemoveAllFilesFromFolder(allFolderNames.CustomersFolder + @"\" + allFolderNames.TemporaryFolder + @"\" + allFolderNames.WebsiteFolder);
                ReplaceWebsiteImage(sender);
            }
            else
            { 
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.jpeg;*.png)|*.jpeg;*png";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    hasWebsiteImage = true;
                    foreach (string filename in openFileDialog.FileNames)
                    {
                        websiteFileString = System.IO.Path.GetFileName(filename);
                        filesForOperations.Items.Add("Website Image: " +
                            System.IO.Path.GetFileName(filename));
                    }

                    ExternalFile.CopyWebsiteImageFile(openFileDialog, basePathForTemporaryFolder2 + @"\" + allFolderNames.WebsiteFolder);
                }
            }
        }

        private void ReplaceWebsiteImage(object sender)
        {
            MessageBox.Show("A website image has already been submitted.  If you load another image it will replace the previous one.  If you'd like to add a image that won't go to the website, click 'Submit File' instead.", "Replace Website Image");
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.jpeg;*.png)|*.jpeg;*png";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                for (int n = filesForOperations.Items.Count - 1; n >= 0; n--)
                {
                    if (filesForOperations.Items[n].ToString().Contains(websiteFileString))
                    {
                        filesForOperations.Items.RemoveAt(n);
                    }
                }
                foreach (string filename in openFileDialog.FileNames)
                {
                    filesForOperations.Items.Add("Website Image: " +
                        System.IO.Path.GetFileName(filename));
                }

                ExternalFile.CopyFile(openFileDialog, basePathForTemporaryFolder2 + @"\" + allFolderNames.WebsiteFolder);
            }
        }

        private void SubmitBaseInformation_Click(object sender, RoutedEventArgs e)
        {
            var submitBaseInformation = new SubmitBaseInformation();
            submitBaseInformation.ShowDialog();
            //enter csv loader here for temporary.csv
            bool fileExists = File.Exists(basePathForTemporaryFolder + @"\" + csvName + ".csv");
            if (fileExists)
            {
                BaseInformation.Clear();
                using (TextFieldParser parser = new TextFieldParser(basePathForTemporaryFolder + @"\" + csvName + ".csv"))
                {
                    ConcatenateString BaseInformationEntry;
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        string baseInformationEntry = "";

                        if (fields[2] != "-")
                        {
                            BaseInformationEntry = new ConcatenateString();
                            if(fields[1] == "-")
                            {
                                fields[1] = "";
                            }
                            baseInformationEntry += fields[0] + ": " + fields[2] + " " + fields[1];

                            BaseInformationEntry.JoinedString = baseInformationEntry.Trim();
                            BaseInformation.Add(BaseInformationEntry);
                        }
                    }
                }
            }
        }

        private void NewCustomer_Click(object sender, RoutedEventArgs e)
        {
            var customer = new Customer();
            customer.ShowDialog();
            RetrieveAllCustomers();
        }

        private void LoadCustomerPresets_Click(object sender, RoutedEventArgs e)
        {
            AllCells.Clear();
            folders = ExternalFile.RetrieveAllFileNamesInDirectory(allFolderNames.CustomersFolder + @"\" + customerList.Text + @"\" + allFolderNames.CellsFolder);
            foreach (string folder in folders)
            {
                customerFolderName = new FileName();
                customerFolderName.Name = folder;
                if (folder != allFolderNames.TemporaryFolder)
                {
                    AllCells.Add(customerFolderName);
                }
            }

            RetrieveAllProducts();
        }

        private void PopulateCellTemplate_Click(object sender, RoutedEventArgs e)
        {
            string cellProductsDirectoryPath = allFolderNames.CustomersFolder + @"\" + customerList.Text + @"\" + allFolderNames.CellsFolder;
            string cellProductsFolderName = @"\" + everyCustomerCell.Text;
            ExternalFile.CheckForDirectory(cellProductsDirectoryPath + cellProductsFolderName);
            folders = ExternalFile.RetrieveAllFolderNamesInDirectory(cellProductsDirectoryPath + cellProductsFolderName);
            foreach (string folder in folders)
            {
                customerFolderName = new FileName();
                customerFolderName.Name = folder;
                if (folder != allFolderNames.TemporaryFolder && folder != "Cells")
                {
                    AllProductsForCustomer.Add(customerFolderName);
                }
            }
            ProductOperations.Clear();
            LoadOperationsCSV(cellProductsDirectoryPath);
            customerList.IsEnabled = false;
            everyCustomerCell.IsEnabled = false;
            //customerProducts.IsEnabled = false;
            addProductOperation.Visibility = Visibility.Hidden;

        }
        private void LoadOperationsCSV(string baseFilePath)
        {
            bool fileExists = File.Exists(baseFilePath + @"\" + everyCustomerCell.Text + ".csv");
            if (fileExists)
            {
                //bool changesWereMade = CompareDuplicateAndProductDetails();

                //if (changesWereMade)
                //{
                //    if (MessageBox.Show("Changes were made to ' " + DetailSet.Text + "' that weren't saved.  Loading a Detail Set will undo any entries/deletions done.  Proceed?", "Load attempt with edit done to " + DetailSet.Text, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                //    {
                //        return;
                //    }
                //}
                ProductOperations.Clear();
                using (TextFieldParser parser = new TextFieldParser(baseFilePath + @"\" + everyCustomerCell.Text + ".csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    ProductOperation productBaseInformation;
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (fields.Any(x => x.Length == 0))
                        {
                            Console.WriteLine("We found an empty value in your CSV. Please check your file and try again.\nPress any key to return to main menu.");
                            Console.ReadKey(true);
                        }
                        productBaseInformation = new ProductOperation();
                        productBaseInformation.Operation = Convert.ToInt32(fields[1]);
                        productBaseInformation.Description = fields[2];

                        if (fields[3] == "null")
                        {
                            productBaseInformation.CycleTime = null;
                        }
                        else
                        {
                            productBaseInformation.CycleTime = float.Parse(fields[3]);
                        }

                        if(fields[4] == "true")
                        {
                            productBaseInformation.RequiredToReport = true;
                        }
                        else
                        {
                            productBaseInformation.RequiredToReport = false;
                        }
                        ProductOperations.Add(productBaseInformation);
                    }
                    //FillDuplicateProductDetailsToVerifyChanges();
                    //productDetailChanges.Items.Clear();
                    //productDetailChanges.Items.Add("Changes made to " + DetailSet.Text + ":");
                    //currentDetailSetIsLoaded = true;
                    //DetailSet.IsEnabled = false;
                }
            }
            customerList.IsEnabled = false;
            everyCustomerCell.IsEnabled = false;
        }

        private string SeeIfProductIsUnassigned()
        {
            string customerName = customerList.Text;
            if (customerName == null || customerName == "")
            {
                customerName = allFolderNames.UnassignedProductsFolder;
            }
            return customerName;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

            string productName = customerProducts.Text;
            string customerName = SeeIfProductIsUnassigned();
            if(customerName == null || customerName == "")
            {
                customerName = allFolderNames.UnassignedProductsFolder;
            }
            bool HasNullValue = CheckOperationNumberForNullValue();

            if (!HasNullValue)
            {
                //if (DetailSetExists)
                //{
                bool changesWereMade = true;//CompareDuplicateAndProductDetails();

                if (changesWereMade)
                {
                    if (MessageBox.Show("New product " + customerProducts.Text + " will be added for " + customerName + ".  Proceed?", "New Product: " + customerProducts.Text, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        //do no stuff
                    }
                    else
                    {
                        if (everyCustomerCell.Text == null || everyCustomerCell.Text == "")
                        {
                            SaveDetailsToCSVGeneral();
                        }
                        else
                        {
                            SaveDetailsForCell();
                        }
                        MessageBox.Show("New product operation saved.", "Save Confirmation");
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

        private void SaveDetailsForCell()
        {
            var csv = new StringBuilder();
            var sortedDetails = ProductOperations.OrderBy(x => x.Operation);
            sortedDetails.ToList();

            ExternalFile.CheckForDirectory(allFolderNames.CustomersFolder + @"\" + customerList.Text + @"\" + allFolderNames.CellsFolder);
            ExternalFile.CheckForDirectory(allFolderNames.CustomersFolder + @"\" + customerList.Text + @"\" + allFolderNames.CellsFolder + @"\" + everyCustomerCell.Text);
            ExternalFile.CheckForDirectory(allFolderNames.CustomersFolder + @"\" + customerList.Text + @"\" + allFolderNames.CellsFolder + @"\" + everyCustomerCell.Text + @"\" + customerProducts.Text);

            //put csv reader back in, redirect it to product number instead of replacing cell settings (change commented out line below)

            //File.WriteAllText(allFolderNames.CustomersFolder + @"\" + customerList.Text + @"\" + allFolderNames.CellsFolder + @"\" + everyCustomerCell.Text + ".csv", csv.ToString());

            ExternalFile.MoveFilesAndFoldersFromTemporary(basePathForTemporaryFolder, allFolderNames.CustomersFolder + @"\" + customerList.Text + @"\" + allFolderNames.CellsFolder + @"\" + everyCustomerCell.Text + @"\" + customerProducts.Text);

            ExternalFile.RemoveAllFoldersAndFilesInDirectory(basePathForTemporaryFolder);
            DatabaseControl.SubmitFileLocationForProduct(customerList.Text, customerProducts.Text, true, everyCustomerCell.Text); 
        }

        private void SaveDetailsToCSVGeneral()
        {
            var csv = new StringBuilder();
            var sortedDetails = ProductOperations.OrderBy(x => x.Operation);
            sortedDetails.ToList();
            string customerName = SeeIfProductIsUnassigned();


            ExternalFile.CheckForDirectory(allFolderNames.CustomersFolder + @"\" + customerName);
            ExternalFile.CheckForDirectory(allFolderNames.CustomersFolder + @"\" + customerName + @"\" + customerProducts.Text);
            ExternalFile.CheckForDirectory(allFolderNames.CustomersFolder + @"\" + customerName + @"\" + customerProducts.Text + @"\" + allFolderNames.OperationDocumentationFolder);

            foreach (ProductOperation operation in sortedDetails)
            {
                var customer = customerName;
                var operationNumber = operation.Operation;
                var description = "-";
                if (operation.Description != "")
                {
                    description = operation.Description;
                }
                var cycleTime = "null";
                float j;
                if (operation.CycleTime != null && float.TryParse(operation.CycleTime.ToString(), out j))
                {
                    cycleTime = operation.CycleTime.ToString();
                }
                var requiredToReport = "true";
                if (operation.RequiredToReport == false)
                {
                    requiredToReport = "false";
                }
                var newLine = string.Format("{0},{1},{2},{3},{4}", customer, operationNumber, description, cycleTime, requiredToReport);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(allFolderNames.CustomersFolder + @"\" + customerName + @"\" + customerProducts.Text + @"\" + customerProducts.Text + ".csv", csv.ToString());

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();
            //foreach (ConcatenateString x in BaseInformation)
            //{
            //    sb.Append(x + "\n");
            //}
            ExternalFile.MoveFilesAndFoldersFromTemporary(basePathForTemporaryFolder, allFolderNames.CustomersFolder + @"\" + customerName + @"\" + customerProducts.Text);

            ExternalFile.RemoveAllFoldersAndFilesInDirectory(basePathForTemporaryFolder);
            //ExternalFile.CombineFilesForPrint(sb, allFolderNames.CustomersFolder + @"\" + customerName + @"\" + customerProducts.Text + @"\" + allFolderNames.OperationDocumentationFolder);
            DatabaseControl.SubmitFileLocationForProduct(customerName, customerProducts.Text, false);
        }
    }
}
