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
    /// Interaction logic for LeaveCell.xaml
    /// </summary>
    public partial class LeaveCell : Window
    {
        DatabaseControl databaseControl = new DatabaseControl();
        StoreEntry storeEntry = new StoreEntry();

        //List<int> allEmployees = new List<int>();
        List<string> allEmployees;

        int cellNumber;
        int employeeNumber;
        public LeaveCell()
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
                    try
                    {
                        allEmployees = storeEntry.AllEmployeesInCell.Split(',').ToList();
                        if (storeEntry.CellNumber == null)
                        {
                            MessageBox.Show("The cell number entered isn't registered in the system.  Make sure the cell number is correct or contact a supervisor if this problem persists.", "Invalid Cell Number");
                        }
                        else if (storeEntry.IsCellActive == false)
                        {
                            MessageBox.Show("This cell is offline.  To work in this cell, choose start cell from main menu.", "Invalid Cell Number");
                        }
                        else if (allEmployees.Count <= 1)
                        {
                            //find where empty employee value is, this never triggers
                            MessageBox.Show("There is only one employee in this cell (" + EmployeeNumber.Text + ").  A cell must always be assigned to at least one employee while online.  To leave this cell, stop the cell instead.", "Only One Employee in Cell");
                        }
                        else
                        {
                            EmployeeNumber.IsEnabled = true;
                            CellNumber.IsEnabled = false;
                            EmployeeNumber.Focusable = true;
                            Keyboard.Focus(EmployeeNumber);
                        }
                    }
                    catch
                    {
                        MessageBox.Show("Could not find cell.  Make sure you entered the cell number correctly.", "Cell not Found");
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
                    //StoreEntry cellEntry = databaseControl.CheckSingleRow("AllCells", "CellNumber", CellNumber.Text);
                    //allEmployees = cellEntry.AllEmployeesInCell.Split(',').ToList();
                    bool containsDuplicate = CheckForDuplicateEmployeeInCell(EmployeeNumber.Text, allEmployees);
                    if (!containsDuplicate)
                    {
                        MessageBox.Show("Employee " + EmployeeNumber.Text + " is not in cell " + CellNumber.Text + ".", "Employee not in Cell");
                    }
                    else
                    {
                        string leadEmployee = allEmployees[0];
                        if (EmployeeNumber.Text == leadEmployee)
                        {
                            MessageBox.Show(EmployeeNumber.Text + " is currently assigned as lead of the cell.  Assign a helper to be the new lead:\n\n" + string.Join(", ", allEmployees.Where(x => x != leadEmployee)), "Assign New Lead of Cell");
                            EmployeeNumber.IsEnabled = false;
                            ReplaceLead.IsEnabled = true;
                            ReplaceLead.Focusable = true;
                            Keyboard.Focus(ReplaceLead);
                            allEmployees.Remove(EmployeeNumber.Text);
                        }
                        else
                        {
                            allEmployees.Remove(EmployeeNumber.Text);
                            if (MessageBox.Show("Cell " + CellNumber.Text + " will now have workers: \n\n" + "Lead Employee: " + leadEmployee + "\n\n Helper Employee(s): " + string.Join(", ", allEmployees.Where(x => x != leadEmployee)) + ".  \n\nProceed?", "Cell Worker Update for " + CellNumber.Text, MessageBoxButton.YesNo) == MessageBoxResult.No)
                            {
                                allEmployees.Add(EmployeeNumber.Text);
                            }
                            else
                            {
                                string allEmployeesEntry = string.Join(",", allEmployees);
                                databaseControl.ChangeSingleValue("AllCells", "EmployeesInCell", "\'" + allEmployeesEntry + "\'", "CellNumber", CellNumber.Text);
                                MessageBox.Show("Join cell succesful.", "Join Cell Completed");
                                Close();
                            }
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
            foreach (string employee in allEmployees)
            {
                if (employee == employeeNumber)
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

        private void ReplaceLead_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (int.TryParse(CellNumber.Text, out cellNumber))
                {
                    if (allEmployees.Remove(ReplaceLead.Text))
                    {
                        if (MessageBox.Show("Cell " + CellNumber.Text + " will now have workers: \n\n" + "Lead Employee: " + ReplaceLead.Text + "\n\n Helper Employee(s): " + string.Join(", ", allEmployees) + ".  \n\nProceed?", "Cell Worker Update for " + CellNumber.Text, MessageBoxButton.YesNo) == MessageBoxResult.No)
                        {
                            allEmployees.Add(ReplaceLead.Text);
                        }
                        else
                        {
                            allEmployees.Remove(EmployeeNumber.Text);
                            string allEmployeesEntry = ReplaceLead.Text + "," + string.Join(",", allEmployees);
                            databaseControl.ChangeSingleValue("AllCells", "EmployeesInCell", "\'" + allEmployeesEntry + "\'", "CellNumber", CellNumber.Text);
                            MessageBox.Show("Leave cell successful.", "Leave Cell Completed");
                            Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("The Employee entered is not one of the current helpers: \n\n", "Employee not Found");
                    }
                }
            }
        }
    }
}
