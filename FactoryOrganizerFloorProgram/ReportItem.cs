using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerFloorProgram
{
    public class ReportItem
    {
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

        public ReportItem()
        {
            ButtonVisibility = "Visible";
        }

    }
}
