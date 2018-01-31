using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerOfficeProgram
{
    public class StashConfirmProduction
    {
        //Customer Item Cell ReportCode
        public string Customer { get; set; }
        public string ItemNumber { get; set; }
        public string CellNumber { get; set; }
        public int TotalOrder { get; set; }
        public string ReportCode { get; set; }
        public string ButtonVisibility { get; set; }

        public StashConfirmProduction()
        {
            ButtonVisibility = "Visible";
        }
    }
}
