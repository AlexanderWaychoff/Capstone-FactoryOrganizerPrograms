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
    /// Interaction logic for Customer.xaml
    /// </summary>
    public partial class Customer : Window
    {
        public ObservableCollection<FileName> LoadedCustomers { get; set; }

        FileName customerFolderName;

        string baseDetailSetFilePath;
        string customersFolder = "Customers";
        string productBaseInformationFolder = "Base Detail Sets";
        string filterTemporaryCreateHolder = "Temporary Create Holder";
        string filterUnassignedProducts = "Unassigned Products";

        string[] folders;

        public Customer()
        {
            InitializeComponent();

            Customers.ItemsSource = LoadedCustomers = new ObservableCollection<FileName>();

            LoadCustomers();
        }

        private void LoadCustomers()
        {
            folders = ExternalFile.RetrieveAllFolderNamesInDirectory(customersFolder);
            foreach (string folder in folders)
            {
                customerFolderName = new FileName();
                customerFolderName.Name = folder;
                if (folder != filterTemporaryCreateHolder && folder != filterUnassignedProducts)
                {
                    LoadedCustomers.Add(customerFolderName);
                }
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            if(Customers.Text != filterTemporaryCreateHolder && Customers.Text != filterUnassignedProducts)
            {
                bool folderExists = ExternalFile.CheckForDirectory(customersFolder + @"\" + Customers.Text);
                if(folderExists)
                {
                    MessageBox.Show("Customer already exists.  Please enter a new customer or click 'OK' to exit.", "Customer Already Exists");
                }
                else
                {
                    MessageBox.Show("Customer has been added.", "Customer Added");
                }
            }
            else
            {
                MessageBox.Show("Customer name not valid.  Try again.", "Customer Name Invalid");
            }
        }
    }
}
