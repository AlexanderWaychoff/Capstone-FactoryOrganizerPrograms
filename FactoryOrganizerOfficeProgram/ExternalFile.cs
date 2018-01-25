﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerOfficeProgram
{
    public static class ExternalFile
    {
        /// <summary>
        /// Returns a bool.  Checks for a directory at specified path.  If one doesn't exist it creates one.
        /// </summary>
        /// <param name="directoryPath">Enter file path here.  Seperate each directory with @"\".</param>
        /// <returns></returns>
        public static bool CheckForDirectory(string directoryPath)
        {
            bool directoryExists = true;
            string settingsFolderName = @".\" + directoryPath;
            if (Directory.Exists(settingsFolderName))
            {
                directoryExists = true;
                return directoryExists;
            }
            else
            {
                Directory.CreateDirectory(settingsFolderName);
                directoryExists = false;
                return directoryExists;
            }
        }

        /// <summary>
        /// Returns a bool.  Check for specific file inside of a directory
        /// </summary>
        /// <param name="filePath">Enter file path here.  Seperate each directory with @"\".</param>
        /// <param name="fileName">File Name can contain full path; file name is extracted in this method (ComboBoxName.Text).</param>
        /// <returns></returns>
        public static bool CheckForFile(string filePath, string fileName)
        {
            bool fileExists = false;
            string[] files = Directory.GetFiles(@".\" + filePath);
            foreach (string file in files)
            {
                string fileNameOnly = System.IO.Path.GetFileNameWithoutExtension(file);
                if (fileNameOnly == fileName)
                {
                    fileExists = true;
                    return fileExists;
                }
            }
            return fileExists;
        }

        /// <summary>
        /// Retrieves all files in targeted directory, removes file path from file, then returns a string array.
        /// </summary>
        /// <param name="filePath">Enter file path here.  Seperate each directory with @"\".</param>
        /// <returns></returns>
        public static string[] RetrieveAllFileNamesInDirectory(string filePath)
        {
            string[] filesWithPath = Directory.GetFiles(@".\" + filePath);
            List<string> filesNameOnly = new List<string>();
            foreach (string file in filesWithPath)
            {
                string fileNameOnly = System.IO.Path.GetFileNameWithoutExtension(file);
                filesNameOnly.Add(fileNameOnly);
            }
            return filesNameOnly.ToArray();
        }
    }
}