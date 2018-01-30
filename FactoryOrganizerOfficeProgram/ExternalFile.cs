using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FactoryOrganizerOfficeProgram
{
    public static class ExternalFile
    {
        /// <summary>
        /// Returns a bool.  Checks for a directory at specified path.  If one doesn't exist it creates one.
        /// </summary>
        /// <param name="directoryPath">Enter file path here.  Seperate each directory with: @"\"</param>
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
        /// <param name="filePath">Enter file path here.  Seperate each directory with: @"\"</param>
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

        /// <summary>
        /// Deletes all files in target folder.  Do not use on a directory (folder with folders inside).
        /// </summary>
        /// <param name="filePath">Enter file path here.  Seperate each directory with @"\".</param>
        public static void RemoveAllFilesFromFolder(string filePath)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(filePath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }

        /// <summary>
        /// Retrieves all folders in targeted directory, removes folder path from folder, then returns a string array.
        /// </summary>
        /// <param name="filePath">Enter folder path here.  Seperate each directory with @"\".</param>
        /// <returns></returns>
        public static string[] RetrieveAllFolderNamesInDirectory(string filePath)
        {
            string[] foldersWithPath = Directory.GetDirectories(@".\" + filePath);
            List<string> filesNameOnly = new List<string>();
            foreach (string folder in foldersWithPath)
            {
                string fileNameOnly = System.IO.Path.GetFileNameWithoutExtension(folder);
                filesNameOnly.Add(fileNameOnly);
            }
            return filesNameOnly.ToArray();
        }

        public static void CopyWebsiteImageFile(OpenFileDialog openFileDialog, string programFilePath)
        {
            var filePath = openFileDialog.FileName;
            string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            try
            {
                System.IO.File.Copy(filePath.ToString(), exePath + @"\" + programFilePath + @"\" + "Image.png");
            }
            catch
            {
                MessageBox.Show("This file already exists.  Application may have closed prematurely.  Try again.", "Duplicate File Error");
            }
        }

        public static void CopyFile(OpenFileDialog openFileDialog, string programFilePath, string description = "")
        {
            var filePath = openFileDialog.FileName;
            string fileName = System.IO.Path.GetFileName(openFileDialog.FileName);
            string exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            try
            {
                if (description == "")
                {
                    System.IO.File.Copy(filePath.ToString(), exePath + @"\" + programFilePath + @"\" + fileName);
                }
                else
                {
                    System.IO.File.Copy(filePath.ToString(), exePath + @"\" + programFilePath + @"\" + description + ".txt");
                }
            }
            catch
            {
                MessageBox.Show("This file already exists.  Application may have closed prematurely.  Try again.", "Duplicate File Error");
            }
        }

        public static void CombineFilesForPrint(StringBuilder sb, string filePath)
        {
            string[] fileNames = Directory.GetFiles(@".\" + filePath, "*.txt");

            string fileTitle;

            //System.Text.StringBuilder sb = new System.Text.StringBuilder();

            for (int nCount = 0; nCount < fileNames.Count(); nCount++)
            {
                fileTitle = Path.GetFileNameWithoutExtension(fileNames[nCount]);

                sb.Append("\n\n**" + fileTitle + "**\n\n");

                sb.Append(System.IO.File.ReadAllText(fileNames[nCount]));
            }

            string output = sb.ToString();

            string outputFilePath = @".\" + filePath + @"\" + "Print.rtf";
            System.IO.File.WriteAllText(outputFilePath, output);

            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo(@".\" + filePath + @"\" + "Print.rtf");
            proc.Start();
        }    
        
        public static void MoveFilesAndFoldersFromTemporary(string STABLE_FOLDER, string UPDATE_FOLDER)
        {
            // This can be handled any way you want, I prefer constants
            //const string STABLE_FOLDER = @"C:\temp\stable\";
            //const string UPDATE_FOLDER = @"C:\temp\updated\";

            // Get our files (recursive and any of them, based on the 2nd param of the Directory.GetFiles() method
            string[] originalFiles = Directory.GetFiles(STABLE_FOLDER, "*", SearchOption.AllDirectories);

            // Dealing with a string array, so let's use the actionable Array.ForEach() with a anonymous method
            Array.ForEach(originalFiles, (originalFileLocation) =>
            {
                // Get the FileInfo for both of our files
                FileInfo originalFile = new FileInfo(originalFileLocation);
                FileInfo destFile = new FileInfo(originalFileLocation.Replace(STABLE_FOLDER, UPDATE_FOLDER));
                // ^^ We can fill the FileInfo() constructor with files that don't exist...

                // ... because we check it here
                if (destFile.Exists)
                {
                    // Logic for files that exist applied here; if the original is larger, replace the updated files...
                    if (originalFile.Length > destFile.Length)
                    {
                        originalFile.CopyTo(destFile.FullName, true);
                    }
                }
                else // ... otherwise create any missing directories and copy the folder over
                {
                    Directory.CreateDirectory(destFile.DirectoryName); // Does nothing on directories that already exist
                    originalFile.CopyTo(destFile.FullName, false); // Copy but don't over-write  
                }

            });
        }    

        public static void RemoveAllFoldersAndFilesInDirectory(string directoryPath)
        {
            System.IO.DirectoryInfo di = new DirectoryInfo(@".\" + directoryPath);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in di.GetDirectories())
            {
                dir.Delete(true);
            }
        }
    }
}
