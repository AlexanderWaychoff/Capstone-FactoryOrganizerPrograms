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
    /// Interaction logic for JoinLeaveCell.xaml
    /// </summary>
    public partial class JoinLeaveCell : Window
    {
        public JoinLeaveCell()
        {
            InitializeComponent();
        }

        private void JoinCell_Click(object sender, RoutedEventArgs e)
        {
            JoinCell joinCell = new JoinCell();
            joinCell.ShowDialog();
            Close();
        }

        private void LeaveCell_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
