using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FactoryOrganizerOfficeProgram
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


        string alexConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=aspnet-FactoryOrganizerWebsite-20180119012225;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";
        string connectionUsed;
        string exePath;
        public DatabaseControl()
        {
            exePath = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location);

            string executable = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string path = (System.IO.Path.GetDirectoryName(executable));
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

            try
            {
                conn = new SqlConnection(alexConnection);
                conn.Open();
                conn.Close();
                connectionUsed = alexConnection;
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

        public void SubmitFileLocationForProduct(string customerName, bool isAssignedToCell)
        {
            string sqlQuery = "INSERT INTO dbo.FilePathToWebsiteInformationForProducts VALUES(@CustomerName, @IsAssignedToCell);"; //put name of table here (dbo.HighScores) and change @'s to appropriate terms
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
                        querySaveStaff.Parameters.Add("@IsAssignedToCell", SqlDbType.Bit).Value = isAssignedToCell;

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
    }
}
