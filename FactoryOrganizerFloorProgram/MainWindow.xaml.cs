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

namespace FactoryOrganizerFloorProgram
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartCell_Click(object sender, RoutedEventArgs e)
        {
            StartCell startCell = new StartCell();
            startCell.ShowDialog();
        }

        private void EndCellRun_Click(object sender, RoutedEventArgs e)
        {
            EndCellRun endCellRun = new EndCellRun();
            endCellRun.ShowDialog();
        }

        private void JoinLeaveCell_Click(object sender, RoutedEventArgs e)
        {
            JoinLeaveCell joinLeaveCell = new JoinLeaveCell();
            joinLeaveCell.ShowDialog();
        }

        private void ReportCellScrap_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EndJob_Click(object sender, RoutedEventArgs e)
        {
            EndJobOperation endJobOperation = new EndJobOperation();
            endJobOperation.ShowDialog();
        }

        private void ReportJobScrap_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuSettings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();

            ValidationPrompt validationPrompt = new ValidationPrompt(settings);
            validationPrompt.Show();
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
