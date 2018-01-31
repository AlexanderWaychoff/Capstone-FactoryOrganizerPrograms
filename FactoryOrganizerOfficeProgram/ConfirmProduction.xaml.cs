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

namespace FactoryOrganizerOfficeProgram
{
    /// <summary>
    /// Interaction logic for ConfirmProduction.xaml
    /// </summary>
    public partial class ConfirmProduction : Window
    {
        DatabaseControl databaseControl;
        public ConfirmProduction(DatabaseControl databaseController)
        {
            InitializeComponent();

            databaseControl = databaseController;
        }

        private void ConfirmProduct_Click(object sender, RoutedEventArgs e)
        {

        }

        private void RandomizeCode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void CheckFloorStatus_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
