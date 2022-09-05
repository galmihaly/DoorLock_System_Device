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
        private string _dataSource = @"172.16.1.6\SQLEXPRESS";
        private string _initialCatalog = "test";
        private bool _persistSecurityInfo = true;
        private string _userId = "sa";
        private string _password = "0207";
        private string _nfcId;

        StringBuilder sb;
        
        string vs = @"Data Source=172.16.1.6\SQLEXPRESS;Initial Catalog=test;Persist Security Info=True;User ID=sa;Password=0207";
        
        SqlConnection dbcon;
        SqlCommand cmd;
        SqlDataReader sdReader;
        SqlConnectionStringBuilder scsb;
        

        public void InitializeDb()
        {
            scsb = new SqlConnectionStringBuilder();
            scsb.DataSource = _dataSource;
            scsb.InitialCatalog = _initialCatalog;
            scsb.PersistSecurityInfo = _persistSecurityInfo;
            scsb.UserID = _userId;
            scsb.Password = _password;


            dbcon = new SqlConnection(scsb.ConnectionString);
            
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
                string query = "Select id, Firstname, Lastname, NfcId, codeNumber from Peoples";
                cmd = new SqlCommand(query, dbcon);
                sdReader = cmd.ExecuteReader();

                while (sdReader.Read())
                {
                    _nfcId = sdReader.GetString(3);

                    if (_nfcId.Equals(nfcId))
                    {
                        return true;
                    }
                    else
                        return false;
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
