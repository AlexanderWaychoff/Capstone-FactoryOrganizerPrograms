using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerOfficeProgram
{
    public class CsvReader
    {

        public CsvReader()
        {

        }
        //"Customers\\Unassigned Product\\7657"
        public List<string> LoadRequiredOperationsCSV(string baseFilePath, string itemNumber)
        {
            List<string> requiredOperations = new List<string>();
            bool fileExists = File.Exists(@".\" + baseFilePath + @"\" + itemNumber + ".csv");
            if (fileExists)
            {
                using (TextFieldParser parser = new TextFieldParser(baseFilePath + @"\" + itemNumber + ".csv"))
                {
                    parser.TextFieldType = FieldType.Delimited;
                    parser.SetDelimiters(",");
                    while (!parser.EndOfData)
                    {
                        string[] fields = parser.ReadFields();

                        string requiredOperation;

                        if (fields[4] == "true")
                        {
                            requiredOperation = fields[1];
                            requiredOperations.Add(requiredOperation);
                        }
                    }
                }
            }
            return requiredOperations;
        }

    }
}
