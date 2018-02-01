using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerFloorProgram
{
    //[ImplementPropertyChanged]
    public class StoreEntry : INotifyPropertyChanged
    {
        bool isTextBoxVisible = true;
        string isTextBoxVisibleString = "True";

        public string Customer { get; set; }
        public int EmployeeNumber { get; set; }
        public string AllEmployeesInCell { get; set; }
        public string ItemNumber { get; set; }
        public string CellNumber { get; set; }
        public int TotalOrder { get; set; }
        public string ReportCode { get; set; }
        public string ButtonVisibility { get; set; }
        public bool IsTextBoxVisible {
            get
            {
                return isTextBoxVisible;
            }
            set
            {
                if (value != this.isTextBoxVisible)
                {
                    this.isTextBoxVisible = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string IsTextBoxVisibleString
        {
            get
            {
                return isTextBoxVisibleString;
            }
            set
            {
                if (value != this.isTextBoxVisibleString)
                {
                    this.isTextBoxVisibleString = value;
                    NotifyPropertyChanged(value);
                }
            }
        }
        public int ProductAwaitingConfirmationID { get; set; }
        public int Operation { get; set; }
        public string RequiredOperations { get; set; }
        public bool IsCellActive { get; set; }
        public DateTime TimeOfReporting { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public StoreEntry()
        {
            IsTextBoxVisible = isTextBoxVisible;
            IsTextBoxVisibleString = isTextBoxVisibleString;
            ButtonVisibility = "Visible";
        }

    }
}
