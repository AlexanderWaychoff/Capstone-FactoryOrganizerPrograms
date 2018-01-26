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

        string baseDetailSetFilePath;
        string settingsFolder = "Settings";
        string productBaseInformationFolder = "Base Detail Sets";

        string[] files;

        public SubmitBaseInformation()
        {
            InitializeComponent();

            allDetailsInSet.ItemsSource = ProductDetails = new ObservableCollection<SetupInformation>();
            DetailSet.ItemsSource = LoadedDetailSets = new ObservableCollection<FileName>();

            baseDetailSetFilePath = @".\" + settingsFolder + @"\" + productBaseInformationFolder;

            ExternalFile.CheckForDirectory(settingsFolder);
            ExternalFile.CheckForDirectory(settingsFolder + @"\" + productBaseInformationFolder);

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
                        productBaseInformation.DescriptionOfDetail = fields[1];
                        ProductDetails.Add(productBaseInformation);
                    }
                    DetailSet.IsEnabled = false;
                }
            }
        }

    }
}
