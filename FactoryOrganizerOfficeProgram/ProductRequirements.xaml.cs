using Microsoft.Win32;
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
    /// Interaction logic for ProductRequirements.xaml
    /// </summary>
    public partial class ProductRequirements : Window
    {
        public ObservableCollection<ProductBaseInformation> ProductDetails { get; set; }

        public ProductRequirements()
        {
            InitializeComponent();

            lstMachineFunctions.ItemsSource = ProductDetails = new ObservableCollection<ProductBaseInformation>();

        }

        private void OnDeleteMachineFunction(object sender, RoutedEventArgs e)
        {
            int indexOfProductOperations = ProductDetails.ToList().FindIndex(x => x == ((sender as FrameworkElement).DataContext as ProductBaseInformation));
            string operationNumber = ProductDetails[indexOfProductOperations].Detail.ToString();
            ProductDetails.Remove((sender as FrameworkElement).DataContext as ProductBaseInformation);

            //for (int n = filesForOperations.Items.Count - 1; n >= 0; --n)
            //{
            //    if (filesForOperations.Items[n].ToString().Contains(operationNumber))
            //    {
            //        filesForOperations.Items.RemoveAt(n);
            //    }
            //}
        }

        private void OnAddMachineFunction(object sender, RoutedEventArgs e)
        {
            ProductDetails.Add(new ProductBaseInformation());
        }

        private void OnAddScaleUnit(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Text files (*.txt;*.rtf)|*.txt;*rtf|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string filename in openFileDialog.FileNames)
                {
                    int indexOfSenderInProductOperations = ProductDetails.ToList().FindIndex(x => x == (sender as FrameworkElement).DataContext as ProductBaseInformation);
                    //filesForOperations.Items.Add("Operation: " +
                    //    ProductOperations[indexOfSenderInProductOperations].Operation +
                    //    "    Description: " +
                    //    ProductOperations[indexOfSenderInProductOperations].Description +
                    //    "    File: " +
                    //    System.IO.Path.GetFileName(filename));
                }
                //txtEditor.Text = File.ReadAllText(openFileDialog.FileName);
            }
            //var mf = (sender as FrameworkElement).DataContext as ProductOperation;

            //    mf.ScaleUnits.Add(new ScaleUnit(mf.ScaleUnits.Count));
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {

        }

        //private void OnDeleteScaleUnit(object sender, RoutedEventArgs e)
        //{
        //    var delScaleUnit = (sender as FrameworkElement).DataContext as ScaleUnit;

        //    var mf = ProductDetails.FirstOrDefault(_ => _.ScaleUnits.Contains(delScaleUnit));

        //    if (mf != null)
        //    {
        //        mf.ScaleUnits.Remove(delScaleUnit);

        //        foreach (var scaleUnit in mf.ScaleUnits)
        //        {
        //            scaleUnit.Index = mf.ScaleUnits.IndexOf(scaleUnit);
        //        }
        //    }
        //}
    }
}
