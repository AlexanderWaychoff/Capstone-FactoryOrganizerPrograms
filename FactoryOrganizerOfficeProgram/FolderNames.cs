﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerOfficeProgram
{
    public class FolderNames
    {
        //base folder Settings
        string settingsFolder = "Settings";
        string productBaseInformationFolder = "Base Detail Sets";
        string scrapCodeFolder = "Scrap Codes";
        //base folder Customers
        string customersFolder = "Customers";
        string unassignedProductsFolder = "Unassigned Products";
        string temporaryFolder = "Temporary Create Holder";
        string cellsFolder = "Cells";
        string websiteFolder = "Website";
        string printableFileFolder = "Printable File";
        string operationDocumentationFolder = "Operation Documentation";

        public string SettingsFolder { get; set; }
        public string CustomersFolder { get; set; }
        public string ProductBaseInformationFolder { get; set; }
        public string UnassignedProductsFolder { get; set; }
        public string TemporaryFolder { get; set; }
        public string ScrapCodeFolder { get; set; }
        public string CellsFolder { get; set; }
        public string WebsiteFolder { get; set; }
        public string PrintableFileFolder { get; set; }
        public string OperationDocumentationFolder { get; set; }

        public FolderNames()
        {
            SettingsFolder = settingsFolder;
            CustomersFolder = customersFolder;
            ProductBaseInformationFolder = productBaseInformationFolder;
            UnassignedProductsFolder = unassignedProductsFolder;
            TemporaryFolder = temporaryFolder;
            ScrapCodeFolder = scrapCodeFolder;
            CellsFolder = cellsFolder;
            WebsiteFolder = websiteFolder;
            WebsiteFolder = websiteFolder;
            PrintableFileFolder = printableFileFolder;
            OperationDocumentationFolder = operationDocumentationFolder;
        }
    }
}
