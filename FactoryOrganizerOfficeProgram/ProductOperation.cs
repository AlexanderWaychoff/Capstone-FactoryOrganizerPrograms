using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerOfficeProgram
{
    public class ProductOperation
    {
        public string Customer { get; set; }
        public int Operation { get; set; }
        public string Description { get; set; }
        public float? CycleTime { get; set; }
        public bool RequiredToReport { get; set; }

        public ObservableCollection<ScaleUnit> ScaleUnits { get; set; }

        public ProductOperation()
        {
            ScaleUnits = new ObservableCollection<ScaleUnit>();
            Description = "-Enter Description-";
        }
    }
}
