using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerOfficeProgram
{
    public class ProductBaseInformation
    {
        public string Detail { get; set; }

        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string DescriptionOfDetail { get; set; }

        public ProductBaseInformation()
        {
            DescriptionOfDetail = "-";
        }
    }
}
