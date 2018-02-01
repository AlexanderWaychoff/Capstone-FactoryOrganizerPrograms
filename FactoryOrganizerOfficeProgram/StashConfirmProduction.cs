using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerOfficeProgram
{
    public class StashConfirmProduction : INotifyPropertyChanged
    {
        //Customer Item Cell ReportCode
        public string Customer { get; set; }
        public string ItemNumber { get; set; }
        public string CellNumber { get; set; }
        public int TotalOrder { get; set; }
        public string ReportCode { get; set; }
        public string ButtonVisibility { get; set; }
        public int ProductAwaitingConfirmationID { get; set; }
        public int Operation { get; set; }
        public string RequiredOperations { get; set; }
        public DateTime TimeOfReporting { get; set; }

        public StashConfirmProduction()
        {
            ButtonVisibility = "Visible";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
