using FirstUwp.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Classes
{
    internal class SqlCommunicator : ISqlCommunicator
    {
        private string _dataSource = "Servername";
        private string _initialCatalog = "DatabaseName";
        private string _userId = "Username";
        private string _password = "Password";

        StringBuilder sb;

        private string conStr;
        SqlConnection dbcon;
        SqlCommand cmd;
        

        public void InitializeDb()
        {
            sb = new StringBuilder();
            conStr = sb.Append("Data Source=").Append(_dataSource).Append(";")
                       .Append("Initial Catalog=").Append(_initialCatalog).Append(";")
                       .Append("User ID=").Append(_userId).Append(";")
                       .Append("Password=").Append(_password).Append(";")
                       .ToString();

            dbcon = new SqlConnection(conStr);

        }

        public string loginUserByCode(string code)
        {
            InitializeDb();

            try
            {
                dbcon.Open();
                cmd = new SqlCommand();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Nem sikerült megnyitni a MySql adatbázist!");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                dbcon.Close();
            }

            return null;
        }

        public string loginUserByNFC_Id(string nfcId)
        {
            InitializeDb();

            try
            {
                dbcon.Open();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Nem sikerült megnyitni a MySql adatbázist!");
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                dbcon.Close();
            }

            return null;
        }
    }
}
