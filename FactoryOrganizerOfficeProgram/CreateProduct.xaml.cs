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
        public ObservableCollection<CustomerInformation> AllCustomers { get; set; }
        public ObservableCollection<ProductProductionCode> AllProductsForCustomer { get; set; }
        public ObservableCollection<ConcatenateString> BaseInformation { get; set; }

        public CustomerInformation Customer;
        public CustomerInformation UserSubmittedCustomer;
        public ProductProductionCode UserSubmittedProductID = new ProductProductionCode();

        bool HasWebsiteImage = false;
        string websiteFileString;

        int productWasAdded = 0;
        string saveProductID;

        string csvName = "temporary";

        string basePathForTemporaryFolder;
        //base folder Customers
        string customerFolder = "Customers";
        string unassignedProductsFolder = "Unassigned Products";
        string temporaryFolder = "Temporary Create Holder";


        public CreateProduct()
        {
            InitializeComponent();

            lstMachineFunctions.ItemsSource = ProductOperations = new ObservableCollection<ProductOperation>();
            CustomerList.ItemsSource = AllCustomers = new ObservableCollection<CustomerInformation>();
            CustomerProducts.ItemsSource = AllProductsForCustomer = new ObservableCollection<ProductProductionCode>();
            baseInformation.ItemsSource = BaseInformation = new ObservableCollection<ConcatenateString>();

            basePathForTemporaryFolder = @".\" + customerFolder + @"\" + temporaryFolder;

            ExternalFile.CheckForDirectory(customerFolder);
            ExternalFile.CheckForDirectory(customerFolder + @"\" + unassignedProductsFolder);
            ExternalFile.CheckForDirectory(customerFolder + @"\" + temporaryFolder);
            RetrieveAllCustomers();
        }

        private void RetrieveAllCustomers()
        {
            Customer = new CustomerInformation();
            Customer.Name = "Baldor";
            this.AllCustomers.Add(Customer);
            //this.AllCustomers.Add("McMillan");
            CustomerList.ItemsSource = AllCustomers;
        }
        
        private void RetrieveAllProducts()
        {

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
        }

        private void OnAddMachineFunction(object sender, RoutedEventArgs e)
        {
            ProductOperations.Add(new ProductOperation());
        }

        private void OnAddScaleUnit(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt;*.rtf)|*.txt;*rtf|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
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
            }
        //var mf = (sender as FrameworkElement).DataContext as ProductOperation;

        //    mf.ScaleUnits.Add(new ScaleUnit(mf.ScaleUnits.Count));
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
            var createProductInformation = new CreateProductInformation();
            createProductInformation.ShowDialog();
        }

        private void CustomerList_KeyDown(object sender, KeyEventArgs e)
        {

            if (e.Key == Key.Return)
            {
                UserSubmittedCustomer = new CustomerInformation();
                string content = CustomerList.Text;
                UserSubmittedCustomer.Name = content;
                int indexTest = AllCustomers.IndexOf(UserSubmittedCustomer);
                if(indexTest == -1 && !AllCustomers.Contains(UserSubmittedCustomer))
                {
                    AllCustomers.Add(UserSubmittedCustomer);
                    CustomerList.ItemsSource = AllCustomers;
                }
            }
        }

        private void ProductID_KeyDown(object sender, KeyEventArgs e)
        {
            if(productWasAdded == 1)
            {
                string content = CustomerProducts.Text;
                int index = AllProductsForCustomer.IndexOf(UserSubmittedProductID);
                AllProductsForCustomer[index].ProductID = content;
                //UserSubmittedProductID.ProductID = content;
                CustomerProducts.ItemsSource = AllProductsForCustomer;
            }
            if (e.Key == Key.Return)
            {
                string content = CustomerProducts.Text;
                UserSubmittedProductID.ProductID = content;
                int indexTest = AllProductsForCustomer.IndexOf(UserSubmittedProductID);
                if (indexTest == -1 && productWasAdded < 1)
                {
                    saveProductID = content;
                    AllProductsForCustomer.Add(UserSubmittedProductID);
                    CustomerProducts.ItemsSource = AllProductsForCustomer;
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

        private void SubmitImage_Click(object sender, RoutedEventArgs e)
        {
            if (HasWebsiteImage)
            {
                ReplaceWebsiteImage(sender);
            }
            else
            { 
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Image files (*.jpeg;*.png)|*.jpeg;*png";
                openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                if (openFileDialog.ShowDialog() == true)
                {
                    HasWebsiteImage = true;
                    foreach (string filename in openFileDialog.FileNames)
                    {
                        websiteFileString = System.IO.Path.GetFileName(filename);
                        filesForOperations.Items.Add("Website Image: " +
                            System.IO.Path.GetFileName(filename));
                    }
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

        }

        private void LoadCustomerPresets_Click(object sender, RoutedEventArgs e)
        {

        }

        private void PopulateCellTemplate_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
