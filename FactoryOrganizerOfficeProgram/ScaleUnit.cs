using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerOfficeProgram
{
    public class ScaleUnit
    {
        public string Name { get; set; }
        public int Index { get; set; }

        public ScaleUnit(int index)
        {
            this.Index = index;
        }
    }
}
