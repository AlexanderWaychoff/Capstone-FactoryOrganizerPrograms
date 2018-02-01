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
using System.Windows.Shapes;

namespace FactoryOrganizerFloorProgram
{
    /// <summary>
    /// Interaction logic for JoinCell.xaml
    /// </summary>
    public partial class JoinCell : Window
    {
        DatabaseControl databaseControl = new DatabaseControl();
        StoreEntry storeEntry = new StoreEntry();

        List<int> allEmployees = new List<int>();

        int cellNumber;
        int employeeNumber;
        public JoinCell()
        {
            InitializeComponent();
        }

        private void CellNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(CellNumber.Text, out cellNumber))
                {
                    storeEntry = databaseControl.CheckSingleRow("AllCells", "CellNumber", cellNumber);

                    if (storeEntry.CellNumber == null)
                    {
                        MessageBox.Show("The cell number entered isn't registered in the system.  Make sure the cell number is correct or contact a supervisor if this problem persists.", "Invalid Cell Number");
                    }
                    else if(storeEntry.IsCellActive == false)
                    {
                        MessageBox.Show("This cell is offline.  To work in this cell, choose start cell from main menu.", "Invalid Cell Number");
                    }
                    else
                    {
                        EmployeeNumber.IsEnabled = true;
                        CellNumber.IsEnabled = false;
                    }
                }
            }
        }

        private void EmployeeNumber_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(EmployeeNumber.Text, out employeeNumber))
                {
                    StoreEntry cellEntry = databaseControl.CheckSingleRow("AllCells", "CellNumber", CellNumber.Text);
                    List<string> allEmployees = cellEntry.AllEmployeesInCell.Split(',').ToList();
                    bool containsDuplicate = CheckForDuplicateEmployeeInCell(EmployeeNumber.Text, allEmployees);
                    if(containsDuplicate)
                    {
                        MessageBox.Show("Employee " + EmployeeNumber.Text + " is already in the cell.", "Employee Already in Cell");
                    }
                    else
                    {
                        string leadEmployee = allEmployees[0];

                        if (MessageBox.Show("Cell " + CellNumber.Text + " will now have workers: \n\n" + "Lead Employee: " + leadEmployee + "\n\n Helper Employee(s): " + string.Join(", ", allEmployees.Where(x => x != leadEmployee)) + ", " + EmployeeNumber.Text + ".  \n\nProceed?", "Cell Worker Update for " + CellNumber.Text, MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            //do no stuff
                        }
                        else
                        {
                            allEmployees.Add(EmployeeNumber.Text);
                            string allEmployeesEntry = string.Join(",", allEmployees);
                            databaseControl.ChangeSingleValue("AllCells", "EmployeesInCell", "\'" + allEmployeesEntry + "\'", "CellNumber", CellNumber.Text);
                            MessageBox.Show("Join cell succesful.", "Join Cell Completed");
                            Close();
                        }
                    }
                }
                else
                {
                    MessageBox.Show("The value entered isn't a number.", "Entry Isn't a Number");
                }
            }
        }

        private bool CheckForDuplicateEmployeeInCell(string employeeNumber, List<string> allEmployees)
        {
            foreach(string employee in allEmployees)
            {
                if(employee == employeeNumber)
                {
                    return true;
                }
            }
            return false;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
