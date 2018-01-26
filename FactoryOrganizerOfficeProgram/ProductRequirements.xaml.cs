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
    /// Interaction logic for ProductRequirements.xaml
    /// </summary>
    public partial class ProductRequirements : Window
    {
        public ObservableCollection<SetupInformation> ProductDetails { get; set; }
        public ObservableCollection<FileName> LoadedDetailSets { get; set; }
        public ObservableCollection<SetupInformation> DuplicateProductDetailsToVerifyChanges = new ObservableCollection<SetupInformation>();

        public FileName FileForDetailSet;
        SetupInformation inheritSender;

        string baseDetailSetFilePath;
        string settingsFolder = "Settings";
        string productBaseInformationFolder = "Base Detail Sets";
        string logChange;

        string[] files;

        bool currentDetailSetIsLoaded = true;
        bool checkedComboBoxTextOnceForNewDetailSet = false;
        bool isConfirmedToClose = false;

        public ProductRequirements()
        {
            InitializeComponent();

            lstMachineFunctions.ItemsSource = ProductDetails = new ObservableCollection<SetupInformation>();
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

        private void OnDeleteMachineFunction(object sender, RoutedEventArgs e)
        {
            ChangeLogAddRemoveDetail(sender);
            ProductDetails.Remove((sender as FrameworkElement).DataContext as SetupInformation);
        }

        private void OnAddMachineFunction(object sender, RoutedEventArgs e)
        {
            if (currentDetailSetIsLoaded)
            {
                bool DetailsAreValid = true;
                if (DetailSet.Text == "")
                {
                    MessageBox.Show("No Detail Set is selected.  Select one from the drop down list and load it, or type a new Detail Set into the field.", "No Detail Set Selected");
                }
                else
                {
                    if(!checkedComboBoxTextOnceForNewDetailSet)
                    {
                        checkedComboBoxTextOnceForNewDetailSet = true;
                        DetailSet.IsEnabled = false;
                        productDetailChanges.Items.Add("Changes made to " + DetailSet.Text + ":");
                    }
                    for (int i = 0; i < ProductDetails.Count; i++)
                    {
                        if (ProductDetails[i].Detail == null || ProductDetails[i].Detail == "")
                        {
                            DetailsAreValid = false;
                            MessageBox.Show("At least one Detail field is empty.  Please enter a value in that field before adding more.", "Empty Detail Field");
                            i += ProductDetails.Count;
                        }
                    }
                    if (DetailsAreValid)
                    {
                        ChangeLogAddNewBlankDetail();
                        ProductDetails.Add(new SetupInformation());
                    }
                }
            }
            else
            {
                MessageBox.Show("You selected a detail set from the drop down menu, but didn't load it.  Please load the selected detail set before adding a Product Detail or type a new name for Detail Set to make a new one.", "Detail Set not Loaded");
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
                    int indexOfSenderInProductOperations = ProductDetails.ToList().FindIndex(x => x == (sender as FrameworkElement).DataContext as SetupInformation);
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            
            string detailSetName = DetailSet.Text;
            bool DetailSetExists = ExternalFile.CheckForFile(settingsFolder + @"\" + productBaseInformationFolder, DetailSet.Text);
            bool HasNullValue = CheckProductDetailsForNullValue();

            if (!HasNullValue)
            {
                if (DetailSetExists)
                {
                    bool changesWereMade = CompareDuplicateAndProductDetails();
                    
                    if (changesWereMade)
                    {
                        if (MessageBox.Show("Current changes affect the Detail Set '" + DetailSet.Text + "'.  Saved changes here will replace the previous entries.  Proceed?", "Changes to " + DetailSet.Text, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
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
                        MessageBox.Show("No changes are being made to '" + DetailSet.Text + "'.  Save canceled.", "Nothing to Save.");
                    }
                }
                else
                {
                    SaveDetailsToCSV();
                    MessageBox.Show("Your details have been saved as '" + detailSetName + "'.  If you would like to edit these in the future select this detail set from the Product Requirement's Detail Set dropbox and load it.", "Product Detail Information Saved");
                }
            }
            else
            {
                MessageBox.Show("At least one Detail field is empty.  Please enter a value in that field before saving or remove it.", "Empty Detail Value");
            }
        }

        private bool CheckProductDetailsForNullValue()
        {
            bool hasNullDetail = false;
            for(int i = 0; i < ProductDetails.Count; i++)
            {
                if(ProductDetails[i].Detail == null)
                {
                    hasNullDetail = true;
                }
            }
            return hasNullDetail;
        }

        private void SaveDetailsToCSV()
        {
            var csv = new StringBuilder();
            var sortedDetails = ProductDetails.OrderBy(x => x.Detail);
            sortedDetails.ToList();

            foreach(SetupInformation information in sortedDetails)
            {
                var detail = information.Detail;
                var description = "-";
                if (information.DescriptionOfDetail != "")
                {
                    description = information.DescriptionOfDetail;
                }
                var newLine = string.Format("{0},{1}", detail, description);
                csv.AppendLine(newLine);
            }
            File.WriteAllText(baseDetailSetFilePath + @"\" + DetailSet.Text + ".csv", csv.ToString());
            DuplicateProductDetailsToVerifyChanges = ProductDetails;
            productDetailChanges.Items.Add("*" + DetailSet.Text + " Detail Set has been saved*");
        }

        private void LoadDetails_Click(object sender, RoutedEventArgs e)
        {
            bool fileExists = File.Exists(baseDetailSetFilePath + @"\" + DetailSet.Text + ".csv");
            if (fileExists)
            {
                bool changesWereMade = CompareDuplicateAndProductDetails();

                if (changesWereMade)
                {
                    if (MessageBox.Show("Changes were made to ' " + DetailSet.Text + "' that weren't saved.  Loading a Detail Set will undo any entries/deletions done.  Proceed?", "Load attempt with edit done to " + DetailSet.Text, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                    {
                        return;
                    }
                }
                ProductDetails.Clear();
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
                    FillDuplicateProductDetailsToVerifyChanges();
                    productDetailChanges.Items.Clear();
                    productDetailChanges.Items.Add("Changes made to " + DetailSet.Text + ":");
                    currentDetailSetIsLoaded = true;
                    DetailSet.IsEnabled = false;
                }                
            }
        }

        private void FillDuplicateProductDetailsToVerifyChanges()
        {
            SetupInformation duplicate;
            DuplicateProductDetailsToVerifyChanges.Clear();
            foreach(SetupInformation detail in ProductDetails)
            {
                duplicate = new SetupInformation();
                duplicate.Detail = detail.Detail;
                duplicate.DescriptionOfDetail = detail.DescriptionOfDetail;
                DuplicateProductDetailsToVerifyChanges.Add(duplicate);
            }
        }

        private bool CompareDuplicateAndProductDetails()
        {
            bool changesWereMade = false;
            if (ProductDetails.Count.Equals(DuplicateProductDetailsToVerifyChanges.Count))
            {
                for (int i = 0; i < ProductDetails.Count; i++)
                {
                    if (ProductDetails[i].Detail.Equals(DuplicateProductDetailsToVerifyChanges[i].Detail) && ProductDetails[i].DescriptionOfDetail.Equals(DuplicateProductDetailsToVerifyChanges[i].DescriptionOfDetail))
                    {

                    }
                    else
                    {
                        changesWereMade = true;
                        i += ProductDetails.Count;
                    }
                }
            }
            else
            {
                changesWereMade = true;
            }
            return changesWereMade;
        }

        private void ChangeLogAddNewBlankDetail()
        {
            productDetailChanges.Items.Add("::New blank detail added.");
        }

        private void ChangeLogAddRemoveDetail(object sender)
        {
            SetupInformation inheritSender;
            inheritSender = ((sender as FrameworkElement).DataContext as SetupInformation);
            if (inheritSender.Detail != null)
            {
                productDetailChanges.Items.Add("::" + inheritSender.Detail + " detail removed.");
            }
            else
            {
                productDetailChanges.Items.Add("::Blank detail removed.");
            }
        }

        private void ChangeLogAddChangeDetail(SetupInformation changedSender)
        {
            if (logChange == null || logChange == "")
            {
                productDetailChanges.Items.Add("::Blank Detail is now " + changedSender.Detail + ".");
            }
            else if (changedSender.Detail == "")
            {
                productDetailChanges.Items.Add("::Detail " + logChange + " is now Blank.");
            }
            else
            {
                productDetailChanges.Items.Add("::" + logChange + " changed to " + changedSender.Detail + ".");
            }
        }

        private void ChangeLogAddChangeDescriptionOfDetail(SetupInformation changedSender)
        {
            if (logChange == null)
            {
                productDetailChanges.Items.Add("::Blank Description is now " + changedSender.DescriptionOfDetail + ".");
            }
            else if (changedSender.DescriptionOfDetail == "")
            {
                productDetailChanges.Items.Add("::Description " + logChange + " is now Blank.");
            }
            else
            {
                productDetailChanges.Items.Add("::" + logChange + " changed to " + changedSender.DescriptionOfDetail + ".");
            }
        }

        private void detailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            inheritSender = ((sender as FrameworkElement).DataContext as SetupInformation);
            logChange = inheritSender.Detail;
        }

        private void detailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetupInformation changedSender;
            changedSender = ((sender as FrameworkElement).DataContext as SetupInformation);

            if (changedSender.Detail != logChange)
            {
                ChangeLogAddChangeDetail(changedSender);
            }
        }

        private void descriptionOfDetailTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            inheritSender = ((sender as FrameworkElement).DataContext as SetupInformation);
            logChange = inheritSender.DescriptionOfDetail;
        }

        private void descriptionOfDetailTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            SetupInformation changedSender;
            changedSender = ((sender as FrameworkElement).DataContext as SetupInformation);

            if (changedSender.DescriptionOfDetail != logChange)
            {
                ChangeLogAddChangeDescriptionOfDetail(changedSender);
            }
        }

        private void DetailSet_DropDownClosed(object sender, EventArgs e)
        {
            if(DetailSet.Text != "" || CheckDetailSetTextForMatches())
            {
                currentDetailSetIsLoaded = false;
            }
        }

        private void DetailSet_LostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            currentDetailSetIsLoaded = CheckDetailSetTextForMatches();
        }

        private bool CheckDetailSetTextForMatches()
        {
            foreach (string file in files)
            {
                if (file == DetailSet.Text)
                {
                    return false;
                }
            }
            return true;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);
            bool changesWereMade = CompareDuplicateAndProductDetails();

            if (changesWereMade)
            {
                if (MessageBox.Show("Current changes affect the Detail Set '" + DetailSet.Text + "'.  Closing now won't save these changes.  Proceed?", "Unsaved Changes to " + DetailSet.Text, MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.No)
                {
                    //do nothing
                }
                else
                {
                    isConfirmedToClose = true;
                }
            }
            else
            {
                isConfirmedToClose = true;
            }
            if (!isConfirmedToClose)
            {
                e.Cancel = true;
            }
        }
    }
}
