using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Hosting;
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
    /// Interaction logic for SubmitBaseInformation.xaml
    /// </summary>
    public partial class SubmitBaseInformation : Window
    {
        public ObservableCollection<SetupInformation> ProductDetails { get; set; }
        public ObservableCollection<FileName> LoadedDetailSets { get; set; }

        FileName FileForDetailSet;

        //csvName used in Create Product to read it too
        string csvName = "temporary";

        string baseDetailSetFilePath;
        string baseCustomerTemporaryFilePath;
        //base folder Settings
        string settingsFolder = "Settings";
        string productBaseInformationFolder = "Base Detail Sets";
        //base folder Customers
        string customerFolder = "Customers";
        string unassignedProductsFolder = "Unassigned Products";
        string temporaryFolder = "Temporary Create Holder";



        string[] files;

        public SubmitBaseInformation()
        {
            InitializeComponent();

            allDetailsInSet.ItemsSource = ProductDetails = new ObservableCollection<SetupInformation>();
            DetailSet.ItemsSource = LoadedDetailSets = new ObservableCollection<FileName>();

            baseDetailSetFilePath = @".\" + settingsFolder + @"\" + productBaseInformationFolder;
            baseCustomerTemporaryFilePath = @".\" + customerFolder + @"\" + temporaryFolder;

            ExternalFile.CheckForDirectory(settingsFolder);
            ExternalFile.CheckForDirectory(settingsFolder + @"\" + productBaseInformationFolder);

            ExternalFile.CheckForDirectory(customerFolder);
            ExternalFile.CheckForDirectory(customerFolder + @"\" + unassignedProductsFolder);
            ExternalFile.CheckForDirectory(customerFolder + @"\" + temporaryFolder);

            LoadDetailSets();
        }

        private void LoadDetailSets()
        {
            files = ExternalFile.RetrieveAllFileNamesInDirectory(settingsFolder + @"\" + productBaseInformationFolder);
            foreach (string file in files)
            {
                FileForDetailSet = new FileName();
                FileForDetailSet.Name = file;
                LoadedDetailSets.Add(FileForDetailSet);
            }
        }

        private void LoadDetails_Click(object sender, RoutedEventArgs e)
        {
            bool fileExists = File.Exists(baseDetailSetFilePath + @"\" + DetailSet.Text + ".csv");
            if (fileExists)
            {
                using (TextFieldParser parser = new TextFieldParser(baseDetailSetFilePath + @"\" + DetailSet.Text + ".csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    SetupInformation productBaseInformation;
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (fields.Any(x => x.Length == 0))
                        {
                            Console.WriteLine("We found an empty value in your CSV. Please check your file and try again.\nPress any key to return to main menu.");
                            Console.ReadKey(true);
                        }
                        productBaseInformation = new SetupInformation();
                        productBaseInformation.Detail = fields[0];
                        if (fields[1] == "-")
                        {
                            productBaseInformation.DescriptionOfDetail = "";
                        }
                        else
                        {
                            productBaseInformation.DescriptionOfDetail = fields[1];
                        }
                        ProductDetails.Add(productBaseInformation);
                    }
                    DetailSet.IsEnabled = false;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var csv = new StringBuilder();
            var sortedDetails = ProductDetails.OrderBy(x => x.Detail);
            sortedDetails.ToList();

            foreach (SetupInformation information in sortedDetails)
            {
                var detail = information.Detail;
                var description = information.DescriptionOfDetail;
                if (information.DescriptionOfDetail == null || information.DescriptionOfDetail == "")
                {
                    description = "-";
                }
                var saveInformation = information.SaveValue;
                if(information.SaveValue == null || information.SaveValue == "")
                {
                    saveInformation = "-";
                }
                var newLine = string.Format("{0},{1},{2}", detail, description, saveInformation);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(baseCustomerTemporaryFilePath + @"\" + csvName + ".csv", csv.ToString());
            MessageBox.Show("Entries saved.  Base Information in Create Product window will update once this window is closed.","Saved Base Information");
        }
    }
}
