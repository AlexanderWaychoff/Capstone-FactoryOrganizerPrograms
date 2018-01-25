﻿using Microsoft.VisualBasic.FileIO;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for ProductRequirements.xaml
    /// </summary>
    public partial class ProductRequirements : Window
    {
        public ObservableCollection<ProductBaseInformation> ProductDetails { get; set; }
        public ObservableCollection<FileName> LoadedDetailSets { get; set; }
        public ObservableCollection<FileName> DuplicateProductDetailsToVerifyChanges { get; set; }

        public FileName FileForDetailSet;

        string baseDetailSetFilePath;
        string settingsFolder = "Settings";
        string productBaseInformationFolder = "Base Detail Sets";

        public ProductRequirements()
        {
            InitializeComponent();

            lstMachineFunctions.ItemsSource = ProductDetails = new ObservableCollection<ProductBaseInformation>();
            DetailSet.ItemsSource = LoadedDetailSets = new ObservableCollection<FileName>();

            baseDetailSetFilePath = @".\" + settingsFolder + @"\" + productBaseInformationFolder;

            ExternalFile.CheckForDirectory(settingsFolder);
            ExternalFile.CheckForDirectory(settingsFolder + @"\" + productBaseInformationFolder);

            LoadDetailSets();
        }

        private void LoadDetailSets()
        {
            string[] files = ExternalFile.RetrieveAllFileNamesInDirectory(settingsFolder + @"\" + productBaseInformationFolder);
            foreach (string file in files)
            {
                FileForDetailSet = new FileName();
                FileForDetailSet.Name = file;
                LoadedDetailSets.Add(FileForDetailSet);
            }
        }

        private void OnDeleteMachineFunction(object sender, RoutedEventArgs e)
        {
            ProductDetails.Remove((sender as FrameworkElement).DataContext as ProductBaseInformation);
        }

        private void OnAddMachineFunction(object sender, RoutedEventArgs e)
        {
            bool DetailsAreValid = true;
            for(int i = 0; i < ProductDetails.Count; i++)
            {
                if(ProductDetails[i].Detail == null)
                {
                    DetailsAreValid = false;
                    MessageBox.Show("At least one Detail field is empty.  Please enter a value in that field before adding more.", "Empty Detail Field");
                    i += ProductDetails.Count;
                }
            }
            if(DetailsAreValid)
            {
                ProductDetails.Add(new ProductBaseInformation());
            }
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
                    int indexOfSenderInProductOperations = ProductDetails.ToList().FindIndex(x => x == (sender as FrameworkElement).DataContext as ProductBaseInformation);
                }
            }
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            //add check to see if any details aren't saved
            this.Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            
            string detailSetName = DetailSet.Text;
            bool DetailSetExists = ExternalFile.CheckForFile(settingsFolder + @"\" + productBaseInformationFolder, DetailSet.Text);

            if (DetailSetExists)
            {
                //add check to see if changes were made and tell user which detail set is being saved
                if (MessageBox.Show("Current changes affect the Detail Set ' " + DetailSet.Text + "'.  Saved changes here will replace the previous entries.  Proceed?", "Changes to " + DetailSet.Text, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //do no stuff
                }
                else
                {
                    SaveDetailsToCSV();
                    this.Close();
                }
            }
            else
            {
                SaveDetailsToCSV();
                MessageBox.Show("Your details have been saved as '" + detailSetName + "'.  If you would like to edit these in the future select this detail set from the Product Requirement's Detail Set dropbox and load it.", "Product Detail Information Saved");
                this.Close();
            }

        }

        private void SaveDetailsToCSV()
        {
            var csv = new StringBuilder();
            var sortedDetails = ProductDetails.OrderBy(x => x.Detail);
            sortedDetails.ToList();

            foreach(ProductBaseInformation information in sortedDetails)
            {
                var detail = information.Detail;
                var description = information.DescriptionOfDetail;
                var newLine = string.Format("{0},{1}", detail, description);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(baseDetailSetFilePath + @"\" + DetailSet.Text + ".csv", csv.ToString());
        }

        private void LoadDetails_Click(object sender, RoutedEventArgs e)
        {
            ProductDetails.Clear();

            bool fileExists = File.Exists(baseDetailSetFilePath + @"\" + DetailSet.Text + ".csv");
            if (fileExists)
            {
                using (TextFieldParser parser = new TextFieldParser(baseDetailSetFilePath + @"\" + DetailSet.Text + ".csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    ProductBaseInformation productBaseInformation;
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();
                        if (fields.Any(x => x.Length == 0))
                        {
                            Console.WriteLine("We found an empty value in your CSV. Please check your file and try again.\nPress any key to return to main menu.");
                            Console.ReadKey(true);
                        }
                        productBaseInformation = new ProductBaseInformation();
                        productBaseInformation.Detail = fields[0];
                        productBaseInformation.DescriptionOfDetail = fields[1];
                        ProductDetails.Add(productBaseInformation);
                    }
                }
            }
        }
    }
}
