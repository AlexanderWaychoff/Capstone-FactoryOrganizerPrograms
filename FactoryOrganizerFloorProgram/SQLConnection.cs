using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Sql;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FactoryOrganizerFloorProgram
{
    public static class SQLConnection
    {
        public static DataTable SqlTestInfo()
        {
            SqlDataSourceEnumerator instance = SqlDataSourceEnumerator.Instance;
            DataTable table = instance.GetDataSources();
            return table;
            //DisplayData(table);
        }

        private static bool IsServerConnected(string connectionString)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        public static string GetServerName()
        {
            // https://msdn.microsoft.com/en-us/library/a6t1z9x2%28v=vs.110%29.aspx?f=255&MSPPError=-2147217396

            DataTable dt = SqlDataSourceEnumerator.Instance.GetDataSources();
            DataRow[] dr = dt.Select("InstanceName='myInstanceName'");

            if (dr.Length == 0)
                return null;

            return dr[0]["ServerName"].ToString();
        }

        //string connectionString;
        //if (this.windowsAuthentication.Checked)
        //    connectionString = string.Format("Server={0}; Integrated Security=SSPI;", sqlServerComboBox.Text);
        //else
        //    connectionString = string.Format("Server={0}; User ID={1}; Password={2};", sqlServerComboBox.Text, usernameTextBox.Text, passwordTextBox.Text);
    }
}
