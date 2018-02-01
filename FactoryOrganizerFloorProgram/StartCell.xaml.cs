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

        List<int> allEmployees = new List<int>();

        int cellNumber;
        int leadEmployee;
        int helperEmployee;

        public StartCell()
        {
            InitializeComponent();

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
                if (int.TryParse(LeadEmployeeNumber.Text, out leadEmployee))
                {
                    LeadEmployeeNumber.IsEnabled = false;
                    HelperEmployee1.IsEnabled = true;
                    allEmployees.Add(leadEmployee);
                }
                else
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
            if (allEmployees.Count != 0)
            {
                if (MessageBox.Show("Cell " + CellNumber.Text + " will now start with: \n" + "Lead Employee: " + LeadEmployeeNumber.Text + "\n\n Helper Employee(s): " + string.Join(", ", allEmployees.Where(x => x != leadEmployee)) + ".  \n\nProceed?", "Cell Start: " + CellNumber.Text, MessageBoxButton.YesNo) == MessageBoxResult.No)
                {
                    //do no stuff
                }
                else
                {
                    string allEmployeesEntry = string.Join(",", allEmployees);
                    databaseControl.ChangeSingleValue("AllCells", "EmployeesInCell", "\'" + allEmployeesEntry + "\'", "CellNumber", CellNumber.Text);
                    databaseControl.ChangeSingleValue("AllCells", "IsCellActive", "1", "CellNumber", CellNumber.Text);
                    databaseControl.ChangeSingleValue("AllCells", "CellStartTime", "\'" + DateTime.Now.ToString() + "\'", "CellNumber", CellNumber.Text);
                    //update database string.join(",", allEmployees)
                }
            }
            else
            {
                MessageBox.Show("No employees have been entered, cell cannot be started.  Click the 'Cancel' button to terminate starting a cell.", "No Employees Entered");
            }
        }

        public bool CheckForDuplicateEmployee(int employeeNumber)
        {
            foreach(int employee in allEmployees)
            {
                if(employee == employeeNumber)
                {
                    return false;
                }
            }
            return true;
        }

        private void HelperEmployee1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(HelperEmployee1.Text, out helperEmployee))
                {
                    if (CheckForDuplicateEmployee(helperEmployee))
                    {
                        HelperEmployee1.IsEnabled = false;
                        HelperEmployee2.IsEnabled = true;
                        allEmployees.Add(helperEmployee);
                    }
                    else
                    {
                        MessageBox.Show("The employee number entered has already been submitted.", "Duplicate Employee Number");
                    }
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }

        private void HelperEmployee2_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(HelperEmployee2.Text, out helperEmployee))
                {
                    if (CheckForDuplicateEmployee(helperEmployee))
                    {
                        HelperEmployee2.IsEnabled = false;
                        HelperEmployee3.IsEnabled = true;
                        allEmployees.Add(helperEmployee);
                    }
                    else
                    {
                        MessageBox.Show("The employee number entered has already been submitted.", "Duplicate Employee Number");
                    }
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }
        private void HelperEmployee3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(HelperEmployee3.Text, out helperEmployee))
                {
                    if (CheckForDuplicateEmployee(helperEmployee))
                    {
                        HelperEmployee3.IsEnabled = false;
                        HelperEmployee4.IsEnabled = true;
                        allEmployees.Add(helperEmployee);
                    }
                    else
                    {
                        MessageBox.Show("The employee number entered has already been submitted.", "Duplicate Employee Number");
                    }
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }
        private void HelperEmployee4_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(HelperEmployee4.Text, out helperEmployee))
                {
                    if (CheckForDuplicateEmployee(helperEmployee))
                    {
                        HelperEmployee4.IsEnabled = false;
                        HelperEmployee5.IsEnabled = true;
                        allEmployees.Add(helperEmployee);
                    }
                    else
                    {
                        MessageBox.Show("The employee number entered has already been submitted.", "Duplicate Employee Number");
                    }
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }
        private void HelperEmployee5_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(HelperEmployee5.Text, out helperEmployee))
                {
                    if (CheckForDuplicateEmployee(helperEmployee))
                    {
                        HelperEmployee5.IsEnabled = false;
                        HelperEmployee6.IsEnabled = true;
                        allEmployees.Add(helperEmployee);
                    }
                    else
                    {
                        MessageBox.Show("The employee number entered has already been submitted.", "Duplicate Employee Number");
                    }
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }
        private void HelperEmployee6_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(HelperEmployee6.Text, out helperEmployee))
                {
                    if (CheckForDuplicateEmployee(helperEmployee))
                    {
                        HelperEmployee6.IsEnabled = false;
                        HelperEmployee7.IsEnabled = true;
                        allEmployees.Add(helperEmployee);
                    }
                    else
                    {
                        MessageBox.Show("The employee number entered has already been submitted.", "Duplicate Employee Number");
                    }
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }
        private void HelperEmployee7_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(HelperEmployee7.Text, out helperEmployee))
                {
                    if (CheckForDuplicateEmployee(helperEmployee))
                    {
                        HelperEmployee7.IsEnabled = false;
                        HelperEmployee8.IsEnabled = true;
                        allEmployees.Add(helperEmployee);
                    }
                    else
                    {
                        MessageBox.Show("The employee number entered has already been submitted.", "Duplicate Employee Number");
                    }
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }
        private void HelperEmployee8_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(HelperEmployee8.Text, out helperEmployee))
                {
                    if (CheckForDuplicateEmployee(helperEmployee))
                    {
                        HelperEmployee8.IsEnabled = false;
                        allEmployees.Add(helperEmployee);
                    }
                    else
                    {
                        MessageBox.Show("The employee number entered has already been submitted.", "Duplicate Employee Number");
                    }
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }
    }
}
