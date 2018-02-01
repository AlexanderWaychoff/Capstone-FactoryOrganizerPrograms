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
    /// Interaction logic for ValidationPrompt.xaml
    /// </summary>
    public partial class ValidationPrompt : Window
    {
        Settings settings;
        public ValidationPrompt(Settings window)
        {
            InitializeComponent();
            settings = window;
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            if(Password.Text == "password")
            {
                settings.Show();
            }
            else
            {
                MessageBox.Show("Password is incorrect.  Contact a supervisor to proceed.", "Incorrect Password");
            }
        }
    }
}
