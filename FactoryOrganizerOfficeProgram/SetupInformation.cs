﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerOfficeProgram
{
    public class SetupInformation
    {
        public string Detail { get; set; }

        public string DescriptionOfDetail { get; set; }

        public SetupInformation()
        {
            DescriptionOfDetail = "-";
        }
    }
}