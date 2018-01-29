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
            }
        }

        private void OnAddMachineFunction(object sender, RoutedEventArgs e)
        {
            if (CustomerList.Text == "" || CustomerList.Text == null)
            {
                MessageBox.Show("No Customer is selected.  Select one from the drop down list or click new to add a Customer.  A cell must be assigned to a customer.", "No Customer Selected");
            }
            else
            {
                CustomerList.IsEnabled = false;
                ProductOperations.Add(new ProductOperation());
            }
        }

    }
}
