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

namespace FactoryOrganizerFloorProgram
{
    /// <summary>
    /// Interaction logic for StartCell.xaml
    /// </summary>
    public partial class StartCell : Window
    {
        DatabaseControl databaseControl = new DatabaseControl();
        StoreEntry storeEntry = new StoreEntry();

        ObservableCollection<StoreEntry> startCellEntry;

        int cellNumber;
        int leadEmployee;
        int helperEmployee;

        public StartCell()
        {
            InitializeComponent();

            cellEmployees.ItemsSource = startCellEntry = new ObservableCollection<StoreEntry>();
        }

        private void CellNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Enter)
            {
                if(int.TryParse(CellNumber.Text, out cellNumber))
                {
                    storeEntry = databaseControl.CheckSingleRow("AllCells", "CellNumber", cellNumber);

                    if (storeEntry.CellNumber == null)
                    {
                        MessageBox.Show("The cell number entered isn't registered in the system.  Make sure the cell number is correct or contact a supervisor if this problem persists.", "Invalid Cell Number");
                    }
                    else
                    {
                        LeadEmployeeNumber.IsEnabled = true;
                        CellNumber.IsEnabled = false;
                    }
                }
            }
        }

        private void LeadEmployeeNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(CellNumber.Text, out leadEmployee))
                {
                    LeadEmployeeNumber.IsEnabled = false;
                    StoreEntry helperEmployee = new StoreEntry();
                    startCellEntry.Add(helperEmployee);
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }

        private void HelperNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                StoreEntry helperNumber = new StoreEntry();
                try
                {
                    helperNumber.EmployeeNumber = ((sender as FrameworkElement).DataContext as StoreEntry).EmployeeNumber;
                    helperNumber.IsTextBoxVisibleString = "False";

                    //((sender as FrameworkElement).DataContext as StoreEntry).IsTextBoxVisible = false;
                    StoreEntry helperEmployee = new StoreEntry();
                    startCellEntry.Add(helperEmployee);

                }
                catch
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void StartCellButton_Click(object sender, RoutedEventArgs e)
        {

        }

    }
}
