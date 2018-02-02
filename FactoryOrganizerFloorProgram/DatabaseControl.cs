﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FactoryOrganizerFloorProgram
{
    public class DatabaseControl
    {
        public SqlConnection conn;
        public SqlTransaction transaction;
        public SqlDataReader myDataReader = null;
        public SqlDataAdapter databaseCommand;
        public SqlCommandBuilder scb;
        public SqlCommand mySqlCommand;
        DataTable fillerTable;
        FolderNames folderNames = new FolderNames();


        string websiteConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=aspnet-FactoryOrganizerWebsite-20180119012225;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        string factoryConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=GenericFactory;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        string connectionUsed;
        string exePath;
        public DatabaseControl()
        {

            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            try
            {
                conn = new SqlConnection(websiteConnection);
                conn.Open();
                conn.Close();
                connectionUsed = websiteConnection;
            }
            catch
            {
                MessageBox.Show("Connection to database failed.  Restart the program to connect.", "Failed Connection");
            }
            finally
            {
                conn.Close();
            }
        }

        public void SubmitProgramFolderLocation()
        {
            string sqlQuery = "INSERT INTO dbo.FilePathToPrograms VALUES(@ProgramType, @FilePath);"; //put name of table here (dbo.HighScores) and change @'s to appropriate terms
            using (SqlConnection openCon = new SqlConnection(connectionUsed))
            {

                using (SqlCommand querySaveStaff = new SqlCommand(sqlQuery))
                {
                    //office program, file path to files
                    try
                    {
                        //
                        openCon.Open();
                        querySaveStaff.Connection = openCon;
                        //querySaveStaff.Parameters.Add("@FilePathToProgramID", SqlDbType.Int, 50).Value = 1;
                        querySaveStaff.Parameters.Add("@ProgramType", SqlDbType.VarChar, 50).Value = "Office";
                        querySaveStaff.Parameters.Add("@FilePath", SqlDbType.VarChar).Value = exePath;

                        querySaveStaff.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                    finally
                    {
                        if (openCon.State == System.Data.ConnectionState.Open)
                        {
                            openCon.Close();
                        }
                    }
                }
            }
        }

        public void SubmitFileLocationForProduct(string customerName, string itemNumber, bool isAssignedToCell, string cellNumber = "")
        {
            exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);
            exePath += @"\" + folderNames.CustomersFolder + @"\" + customerName;
            if (isAssignedToCell == true)
            {
                exePath += @"\" + folderNames.CellsFolder + @"\" + cellNumber;
            }
            exePath += @"\" + itemNumber + @"\" + folderNames.WebsiteFolder + @"\";
            //C:\Users\Andross\Desktop\school_projects\C#\FactoryOrganizerOfficeAndFloorPrograms\FactoryOrganizerOfficeProgram\bin\Debug\Customers\Cells\210\4774\Website\
            string sqlQuery = "INSERT INTO dbo.FilePathToWebsiteInformationForProducts VALUES(@CustomerName, @ItemNumber ,@IsAssignedToCell, @CellNumber, @WholeFilePath);"; //put name of table here (dbo.HighScores) and change @'s to appropriate terms
            using (SqlConnection openCon = new SqlConnection(connectionUsed))
            {

                using (SqlCommand querySaveStaff = new SqlCommand(sqlQuery))
                {
                    //office program, file path to files
                    try
                    {
                        //
                        openCon.Open();
                        querySaveStaff.Connection = openCon;
                        //querySaveStaff.Parameters.Add("@FilePathToProgramID", SqlDbType.Int, 50).Value = 1;
                        querySaveStaff.Parameters.Add("@CustomerName", SqlDbType.VarChar, 50).Value = customerName;
                        querySaveStaff.Parameters.Add("@ItemNumber", SqlDbType.VarChar, 50).Value = itemNumber;
                        querySaveStaff.Parameters.Add("@IsAssignedToCell", SqlDbType.Bit).Value = isAssignedToCell;
                        if (cellNumber != "" && cellNumber != null)
                        {
                            querySaveStaff.Parameters.Add("@CellNumber", SqlDbType.VarChar).Value = cellNumber;
                        }
                        else
                        {
                            querySaveStaff.Parameters.Add("@CellNumber", SqlDbType.VarChar).Value = "";
                        }
                        querySaveStaff.Parameters.Add("@WholeFilePath", SqlDbType.VarChar).Value = exePath;

                        querySaveStaff.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                    finally
                    {
                        if (openCon.State == System.Data.ConnectionState.Open)
                        {
                            openCon.Close();
                        }
                    }
                }
            }
        }

        public List<StoreEntry> RetrieveProductsToConfirm()
        {
            List<StoreEntry> confirmProductionList = new List<StoreEntry>();
            try
            {
                SqlConnection mySqlConnection = new SqlConnection(connectionUsed);
                mySqlCommand = new SqlCommand("SELECT * FROM dbo.ProductAwaitingConfirmations ORDER BY CustomerName DESC", mySqlConnection); //put table name to search from, specify search
                mySqlConnection.Open();
                myDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

                while (myDataReader.Read())
                {
                    StoreEntry confirmProduction = new StoreEntry();
                    confirmProduction.ProductAwaitingConfirmationID = myDataReader.GetInt32(0);
                    confirmProduction.Customer = myDataReader.GetString(1);
                    confirmProduction.ItemNumber = myDataReader.GetString(2);
                    confirmProduction.TotalOrder = myDataReader.GetInt32(3);
                    if (myDataReader.IsDBNull(4))
                    {
                        confirmProduction.CellNumber = null;
                    }
                    else
                    {
                        confirmProduction.CellNumber = myDataReader.GetString(4);
                    }

                    confirmProductionList.Add(confirmProduction);
                }
                myDataReader.Close();
                conn.Close();

                return confirmProductionList;
            }
            catch
            {

            }
            return confirmProductionList;
        }

        public List<StoreEntry> RetrieveProductsForPrint()
        {
            List<StoreEntry> printList = new List<StoreEntry>();
            try
            {
                SqlConnection mySqlConnection = new SqlConnection(factoryConnection);
                mySqlCommand = new SqlCommand("SELECT * FROM dbo.JobProductions ORDER BY Customer DESC", mySqlConnection); //put table name to search from, specify search
                mySqlConnection.Open();
                myDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

                while (myDataReader.Read())
                {
                    StoreEntry confirmProduction = new StoreEntry();
                    confirmProduction.ProductAwaitingConfirmationID = myDataReader.GetInt32(0);
                    confirmProduction.Customer = myDataReader.GetString(1);
                    confirmProduction.ReportCode = myDataReader.GetString(2);
                    confirmProduction.ItemNumber = myDataReader.GetString(3);
                    confirmProduction.TotalOrder = myDataReader.GetInt32(4);
                    confirmProduction.CellNumber = "-";

                    printList.Add(confirmProduction);
                }
                myDataReader.Close();
                conn.Close();

                return printList;
            }
            catch
            {

            }
            return printList;
        }

        public List<StoreEntry> RetrieveScrapCodes()
        {
            List<StoreEntry> printList = new List<StoreEntry>();
            try
            {
                SqlConnection mySqlConnection = new SqlConnection(factoryConnection);
                mySqlCommand = new SqlCommand("SELECT * FROM dbo.ScrapCodes", mySqlConnection); //put table name to search from, specify search
                mySqlConnection.Open();
                myDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

                while (myDataReader.Read())
                {
                    StoreEntry confirmProduction = new StoreEntry();
                    confirmProduction.ScrapReason = myDataReader.GetString(1);

                    printList.Add(confirmProduction);
                }
                myDataReader.Close();
                conn.Close();

                return printList;
            }
            catch
            {

            }
            return printList;
        }

        public void SubmitCell(StoreEntry job)
        {
            string sqlQuery = "INSERT INTO dbo.AllCells VALUES(@CellNumber, @EmployeesInCell, @IsCellActive, @CellStartTime);"; //put name of table here (dbo.HighScores) and change @'s to appropriate terms
            using (SqlConnection openCon = new SqlConnection(factoryConnection))
            {

                using (SqlCommand querySaveStaff = new SqlCommand(sqlQuery))
                {
                    //office program, file path to files
                    try
                    {
                        //
                        openCon.Open();
                        querySaveStaff.Connection = openCon;
                        querySaveStaff.Parameters.Add("@CellNumber", SqlDbType.VarChar, 50).Value = job.CellNumber;
                        querySaveStaff.Parameters.Add("@EmployeesInCell", SqlDbType.VarChar, 50).Value = "";
                        querySaveStaff.Parameters.Add("@IsCellActive", SqlDbType.Bit).Value = 0;
                        querySaveStaff.Parameters.Add("@CellStartTime", SqlDbType.DateTime).Value = job.TimeOfReporting;
  
                        querySaveStaff.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                    finally
                    {
                        if (openCon.State == System.Data.ConnectionState.Open)
                        {
                            openCon.Close();
                        }
                    }
                }
            }
        }

        public void SubmitCellJob(StoreEntry job)
        {
            string sqlQuery = "INSERT INTO dbo.CellProducts VALUES(@Customer, @CellNumber, @ItemNumber, @TotalPieces, @ReportedPieces, @EmployeesInCell, @TimeOfReporting);"; //put name of table here (dbo.HighScores) and change @'s to appropriate terms
            using (SqlConnection openCon = new SqlConnection(factoryConnection))
            {

                using (SqlCommand querySaveStaff = new SqlCommand(sqlQuery))
                {
                    //office program, file path to files
                    try
                    {
                        //
                        openCon.Open();
                        querySaveStaff.Connection = openCon;
                        querySaveStaff.Parameters.Add("@Customer", SqlDbType.VarChar, 50).Value = job.Customer;
                        querySaveStaff.Parameters.Add("@CellNumber", SqlDbType.VarChar, 50).Value = job.CellNumber;
                        querySaveStaff.Parameters.Add("@ItemNumber", SqlDbType.VarChar, 50).Value = job.ItemNumber;
                        querySaveStaff.Parameters.Add("@TotalPieces", SqlDbType.Int).Value = job.TotalOrder;
                        querySaveStaff.Parameters.Add("@ReportedPieces", SqlDbType.Int).Value = 0;
                        querySaveStaff.Parameters.Add("@EmployeesInCell", SqlDbType.VarChar, 50).Value = null;
                        querySaveStaff.Parameters.Add("@TimeOfReporting", SqlDbType.DateTime).Value = job.TimeOfReporting;

                        querySaveStaff.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                    finally
                    {
                        if (openCon.State == System.Data.ConnectionState.Open)
                        {
                            openCon.Close();
                        }
                    }
                }
            }
        }

        public void SubmitCellEntry(StoreEntry job)
        {
            string sqlQuery = "INSERT INTO dbo.CellEntries VALUES(@CellNumber, @EmployeesInCell, @ItemNumber, @AmountReported, @ScrapAmount, @ScrapReason, @TimeOfCompletion);"; //put name of table here (dbo.HighScores) and change @'s to appropriate terms
            using (SqlConnection openCon = new SqlConnection(factoryConnection))
            {

                using (SqlCommand querySaveStaff = new SqlCommand(sqlQuery))
                {
                    //office program, file path to files
                    try
                    {
                        //
                        openCon.Open();
                        querySaveStaff.Connection = openCon;
                        querySaveStaff.Parameters.Add("@CellNumber", SqlDbType.VarChar, 50).Value = job.CellNumber;
                        querySaveStaff.Parameters.Add("@EmployeesInCell", SqlDbType.VarChar, 100).Value = job.AllEmployeesInCell;
                        querySaveStaff.Parameters.Add("@ItemNumber", SqlDbType.VarChar, 50).Value = job.ItemNumber;
                        querySaveStaff.Parameters.Add("@AmountReported", SqlDbType.Int).Value = job.AmountCompleted;
                        querySaveStaff.Parameters.Add("@ScrapAmount", SqlDbType.Int).Value = job.ScrapAmount;
                        querySaveStaff.Parameters.Add("@ScrapReason", SqlDbType.VarChar, 50).Value = job.ScrapReason;
                        querySaveStaff.Parameters.Add("@TimeOfCompletion", SqlDbType.DateTime).Value = job.TimeOfReporting;

                        querySaveStaff.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                    finally
                    {
                        if (openCon.State == System.Data.ConnectionState.Open)
                        {
                            openCon.Close();
                        }
                    }
                }
            }
        }

        public void SubmitJobEntry(StoreEntry job)
        {
            string sqlQuery = "INSERT INTO dbo.JobEntries VALUES(@EmployeeNumber, @ReportCode, @ItemNumber, @AmountReported, @Operation, @ScrapAmount, @ScrapReason, @TimeOfCompletion);"; //put name of table here (dbo.HighScores) and change @'s to appropriate terms
            using (SqlConnection openCon = new SqlConnection(factoryConnection))
            {

                using (SqlCommand querySaveStaff = new SqlCommand(sqlQuery))
                {
                    //office program, file path to files
                    try
                    {
                        //
                        openCon.Open();
                        querySaveStaff.Connection = openCon;
                        querySaveStaff.Parameters.Add("@EmployeeNumber", SqlDbType.VarChar, 50).Value = job.EmployeeNumber;
                        querySaveStaff.Parameters.Add("@ReportCode", SqlDbType.VarChar, 100).Value = job.ReportCode;
                        querySaveStaff.Parameters.Add("@ItemNumber", SqlDbType.VarChar, 50).Value = job.ItemNumber;
                        querySaveStaff.Parameters.Add("@AmountReported", SqlDbType.Int).Value = job.AmountCompleted;
                        querySaveStaff.Parameters.Add("@Operation", SqlDbType.Int).Value = job.Operation;
                        querySaveStaff.Parameters.Add("@ScrapAmount", SqlDbType.Int).Value = job.ScrapAmount;
                        querySaveStaff.Parameters.Add("@ScrapReason", SqlDbType.VarChar, 50).Value = job.ScrapReason;
                        querySaveStaff.Parameters.Add("@TimeOfCompletion", SqlDbType.DateTime).Value = job.TimeOfReporting;

                        querySaveStaff.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                    finally
                    {
                        if (openCon.State == System.Data.ConnectionState.Open)
                        {
                            openCon.Close();
                        }
                    }
                }
            }
        }

        public void ChangeSingleValue<T>(string tableName, string columnValueToChange, T valueToInsert, string columnToVerifyWith, T verifyColumnValue)
        {
            string queryToLaunch = "UPDATE dbo." + tableName + " SET " + columnValueToChange + " = " + valueToInsert + " WHERE " + columnToVerifyWith + " = " + verifyColumnValue + ";";
            using (SqlConnection openCon = new SqlConnection(factoryConnection))
            {
                using (SqlCommand querySaveStaff = new SqlCommand(queryToLaunch))
                {
                    try
                    {
                        openCon.Open();
                        querySaveStaff.Connection = openCon;

                        querySaveStaff.ExecuteNonQuery();
                        openCon.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                    finally
                    {
                        if (openCon.State == System.Data.ConnectionState.Open)
                        {
                            openCon.Close();
                        }
                    }
                }
            }
        }

        public void ChangeSingleValueOperation(string tableName, string columnValueToChange, int valueToInsert, string columnToVerifyWith, string verifyColumnValue)
        {
            string queryToLaunch = "UPDATE dbo." + tableName + " SET " + columnValueToChange + " = " + valueToInsert + " WHERE " + columnToVerifyWith + " = " + verifyColumnValue + ";";
            using (SqlConnection openCon = new SqlConnection(factoryConnection))
            {
                using (SqlCommand querySaveStaff = new SqlCommand(queryToLaunch))
                {
                    try
                    {
                        openCon.Open();
                        querySaveStaff.Connection = openCon;

                        querySaveStaff.ExecuteNonQuery();
                        openCon.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                    finally
                    {
                        if (openCon.State == System.Data.ConnectionState.Open)
                        {
                            openCon.Close();
                        }
                    }
                }
            }
        }

        public StoreEntry CheckSingleRow<T>(string tableName, string columnToVerifyWith, T verifyColumnValue)
        {
            StoreEntry cellCheck = new StoreEntry();
            string queryToLaunch = "SELECT * FROM dbo." + tableName +  " WHERE " + columnToVerifyWith + " = " + verifyColumnValue + ";";
            using (SqlConnection openCon = new SqlConnection(factoryConnection))
            {
                try
                {
                    mySqlCommand = new SqlCommand(queryToLaunch, openCon); //put table name to search from, specify search
                    openCon.Open();
                    myDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    while (myDataReader.Read())
                    {
                        cellCheck.CellNumber = myDataReader.GetString(1);
                        if (myDataReader.IsDBNull(2))
                        {
                            cellCheck.AllEmployeesInCell = "";
                        }
                        else
                        {
                            cellCheck.AllEmployeesInCell = myDataReader.GetString(2);
                        }
                        try
                        {
                            cellCheck.IsCellActive = myDataReader.GetBoolean(3);
                        }
                        catch
                        {
                            cellCheck.ItemNumber = myDataReader.GetString(3);
                            cellCheck.TotalOrder = myDataReader.GetInt32(4);
                        }
                        //cellCheck.TimeOfReporting = myDataReader.GetDateTime(4);
                    }
                    myDataReader.Close();
                    openCon.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: '{0}'", e);
                }
                finally
                {
                    if (openCon.State == System.Data.ConnectionState.Open)
                    {
                        openCon.Close();
                    }
                }
            }
            return cellCheck;
        }

        public StoreEntry CheckSingleRowForCellRun<T>(StoreEntry cellCheck, string tableName, string columnToVerifyWith, T verifyColumnValue)
        {
            string queryToLaunch = "SELECT * FROM dbo." + tableName + " WHERE " + columnToVerifyWith + " = " + verifyColumnValue + ";";
            using (SqlConnection openCon = new SqlConnection(factoryConnection))
            {
                try
                {
                    mySqlCommand = new SqlCommand(queryToLaunch, openCon); //put table name to search from, specify search
                    openCon.Open();
                    myDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    while (myDataReader.Read())
                    {
                        cellCheck.Customer = myDataReader.GetString(1);
                        
                        cellCheck.ItemNumber = myDataReader.GetString(3);
                        cellCheck.TotalOrder = myDataReader.GetInt32(4);
                        
                        //cellCheck.TimeOfReporting = myDataReader.GetDateTime(4);
                    }
                    myDataReader.Close();
                    openCon.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: '{0}'", e);
                }
                finally
                {
                    if (openCon.State == System.Data.ConnectionState.Open)
                    {
                        openCon.Close();
                    }
                }
            }
            return cellCheck;
        }

        public StoreEntry CheckSingleRowForJob(string tableName, string columnToVerifyWith, string verifyColumnValue)
        {
            StoreEntry cellCheck = new StoreEntry();
            string queryToLaunch = "SELECT * FROM dbo." + tableName + " WHERE " + columnToVerifyWith + " = " + verifyColumnValue + ";";
            using (SqlConnection openCon = new SqlConnection(factoryConnection))
            {
                try
                {
                    mySqlCommand = new SqlCommand(queryToLaunch, openCon); //put table name to search from, specify search
                    openCon.Open();
                    myDataReader = mySqlCommand.ExecuteReader(CommandBehavior.CloseConnection);

                    while (myDataReader.Read())
                    {
                        cellCheck.Customer = myDataReader.GetString(1);
                        cellCheck.ReportCode = myDataReader.GetString(2);
                        cellCheck.ItemNumber = myDataReader.GetString(3);
                        cellCheck.TotalOrder = myDataReader.GetInt32(4);
                        cellCheck.AmountCompleted = myDataReader.GetInt32(5);
                        cellCheck.Operation = myDataReader.GetInt32(6);
                        cellCheck.RequiredOperations = myDataReader.GetString(7);
                        cellCheck.TimeOfReporting = myDataReader.GetDateTime(8);

                        //cellCheck.TimeOfReporting = myDataReader.GetDateTime(4);
                    }
                    myDataReader.Close();
                    openCon.Close();
                }
                catch (Exception e)
                {
                    Console.WriteLine("An error occurred: '{0}'", e);
                }
                finally
                {
                    if (openCon.State == System.Data.ConnectionState.Open)
                    {
                        openCon.Close();
                    }
                }
            }
            return cellCheck;
        }

        public void DeleteDefinedRow<T>(string tableName, string columnToVerifyWith, T verifyColumnValue)
        {
            string queryToLaunch = "DELETE dbo." + tableName + " WHERE " + columnToVerifyWith + " = " + verifyColumnValue + ";";
            using (SqlConnection openCon = new SqlConnection(connectionUsed))
            {
                using (SqlCommand querySaveStaff = new SqlCommand(queryToLaunch))
                {
                    try
                    {
                        openCon.Open();
                        querySaveStaff.Connection = openCon;

                        querySaveStaff.ExecuteNonQuery();
                        openCon.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("An error occurred: '{0}'", e);
                    }
                    finally
                    {
                        if (openCon.State == System.Data.ConnectionState.Open)
                        {
                            openCon.Close();
                        }
                    }
                }
            }
        }

    }
}
