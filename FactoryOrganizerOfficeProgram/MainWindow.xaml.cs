using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FactoryOrganizerOfficeProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        FolderNames folderNames = new FolderNames();
        DatabaseControl databaseControl = new DatabaseControl();
        CsvReader csvReader = new CsvReader();
        public MainWindow()
        {
            InitializeComponent();

            SubmitToDatabase();
        }
        //MenuSettings_Click
        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.ShowDialog();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CreateProduct_Click(object sender, RoutedEventArgs e)
        {
            ExternalFile.RemoveAllFilesFromFolder(@".\" + folderNames.CustomersFolder + @"\" + folderNames.TemporaryFolder);
            var createProduct = new CreateProduct(databaseControl);
            createProduct.ShowDialog();
        }

        private void SubmitToDatabase()
        {
            databaseControl.SubmitProgramFolderLocation();
        }

        private void ConfirmProduction_Click(object sender, RoutedEventArgs e)
        {
            var confirmProduction = new ConfirmProduction(databaseControl, csvReader, folderNames);
            confirmProduction.ShowDialog();
        }
    }
}
