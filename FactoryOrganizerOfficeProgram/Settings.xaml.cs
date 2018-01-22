using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Settings : Window
    {
        public string localConnection = "Data Source=(localdb)\\MSSQLLocalDB;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        public ObservableCollection<string> GatheredConnections = new ObservableCollection<string>();
        public Settings()
        {
            InitializeComponent();
            AvailableConnections.ItemsSource = GatheredConnections;
        }

        private void SearchForSQLConnections_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                RetrieveServerNames(SQLConnection.SqlTestInfo());
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
        }
        
        private void RetrieveServerNames(DataTable connectionsToCheck)
        {
            string serverName;
            foreach(DataRow server in connectionsToCheck.Rows)
            {
                serverName = server["ServerName"].ToString();
                this.GatheredConnections.Add(serverName);
            }
            AvailableConnections.ItemsSource = GatheredConnections;
        }
    }
}
