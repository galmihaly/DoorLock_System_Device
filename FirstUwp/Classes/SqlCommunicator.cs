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
        private string _dataSource = @"(LocalDB)\MSSQLLocalDB";
        private string _attachDbFilename = @"C:\Users\misi0\Documents\Peoples.mdf";
        private string _integratedSecurity = "true";
        private string _connectTimeOut = @"30";

        StringBuilder sb;

        string vs = @"Server=(LocalDB)\MSSQLLocalDB;Database=C:\Users\misi0\Documents\Peoples.mdf;Trusted_Connection=True;";
        
        private string conStr;
        SqlConnection dbcon;
        SqlCommand cmd;
        SqlDataReader sdReader;
        

        public void InitializeDb()
        {
            sb = new StringBuilder();
            conStr = sb.Append("data source=").Append(_dataSource).Append(";")
                       .Append("Database=").Append(_attachDbFilename).Append(";")
                       .Append("trusted_connection=").Append(_integratedSecurity).Append(";")
                       .Append("Password=").Append(_connectTimeOut).Append(";")
                       .ToString();

            dbcon = new SqlConnection(vs);
            
        }

        public bool loginUserByCode(string code)
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

            return true;
        }

        public bool loginUserByNFC_Id(string nfcId)
        {
            InitializeDb();

            try
            {
                dbcon.Open();
                Debug.WriteLine("Siker");
                string query = "Select * From Peoples";
                cmd = new SqlCommand(query, dbcon);
                sdReader = cmd.ExecuteReader();
                while (sdReader.Read())
                {
                    Debug.WriteLine(sdReader.GetString(0) + " " + sdReader.GetString(1) + " " + sdReader.GetString(2));
                }
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

            return true;
        }
    }
}
