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
    /// Interaction logic for ConfirmProduction.xaml
    /// </summary>
    public partial class ConfirmProduction : Window
    {
        public ObservableCollection<StashConfirmProduction> stashConfirmProduction;

        DatabaseControl databaseControl;
        public ConfirmProduction(DatabaseControl databaseController)
        {
            InitializeComponent();

            databaseControl = databaseController;

            confirmProductionList.ItemsSource = stashConfirmProduction = new ObservableCollection<StashConfirmProduction>();

            PopulateStash();
        }

        private void PopulateStash()
        {
            List<StashConfirmProduction> productsToConfirm;
            productsToConfirm = databaseControl.RetrieveProductsToConfirm();

            foreach(StashConfirmProduction product in productsToConfirm)
            {
                if (product.CellNumber == null || product.CellNumber == "")
                {
                    stashConfirmProduction.Add(product);
                }
                else
                {
                    product.ButtonVisibility = "Hidden";
                    stashConfirmProduction.Add(product);
                }

            }
        }

        private void ConfirmProduct_Click(object sender, RoutedEventArgs e)
        {
            StashConfirmProduction removedConfirmation = ((sender as FrameworkElement).DataContext as StashConfirmProduction);

            if (removedConfirmation.ButtonVisibility == "Hidden")
            {
                //product is ran in a cell, send to separate database table and delete entry from data table and observable productsToConfirm
                databaseControl.SubmitCellJob(removedConfirmation);

                MessageBox.Show("Product " + removedConfirmation.ItemNumber + " sent out to cell " + removedConfirmation.CellNumber + " for production. ", "Cell Product sent for Production");
            }
            else
            {
                //do checks for filled in Report Code
            }
        }

        private void RandomizeCode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckFloorStatus_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Print_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
