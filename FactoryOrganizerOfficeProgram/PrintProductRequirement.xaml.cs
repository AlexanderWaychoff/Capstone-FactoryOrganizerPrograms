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
    /// Interaction logic for PrintProductRequirement.xaml
    /// </summary>
    public partial class PrintProductRequirement : Window
    {
        public ObservableCollection<StashConfirmProduction> stashConfirmProduction;

        DateTime today;
        DatabaseControl databaseControl;
        CsvReader csvReader;
        FolderNames folderNames;

        public PrintProductRequirement(DatabaseControl databaseController, CsvReader csvRead, FolderNames allFolders)
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
            productsToConfirm = databaseControl.RetrieveProductsForPrint();

            foreach (StashConfirmProduction product in productsToConfirm)
            {
                stashConfirmProduction.Add(product);
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {
            StashConfirmProduction fileToPrint = ((sender as FrameworkElement).DataContext as StashConfirmProduction);

            string printFilePath = folderNames.CustomersFolder + @"\" + fileToPrint.Customer + @"\" + fileToPrint.ItemNumber + @"\" + folderNames.PrintableFileFolder;

            ExternalFile.OpenFileForPrint(printFilePath, fileToPrint);

        }
    }
}
