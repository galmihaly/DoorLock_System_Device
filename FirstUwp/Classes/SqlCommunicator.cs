using FirstUwp.Interfaces;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Encoders;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnitsNet;

namespace FirstUwp.Classes
{
    internal class SqlCommunicator : ISqlCommunicator
    {
        private string _dataSource = @"172.16.1.6\SQLEXPRESS";
        private string _initialCatalog = "test";
        private bool _persistSecurityInfo = true;
        private string _userId = "sa";
        private string _password = "0207";
        
        SqlConnection dbcon;
        SqlConnectionStringBuilder scsb;
        

        public SqlConnection InitializeDb()
        {
            scsb = new SqlConnectionStringBuilder();
            scsb.DataSource = _dataSource;
            scsb.InitialCatalog = _initialCatalog;
            scsb.PersistSecurityInfo = _persistSecurityInfo;
            scsb.UserID = _userId;
            scsb.Password = _password;

            var seged = new SqlConnection(scsb.ConnectionString);

            return seged;
            
        }

        public string GetConnectionString()
        {
            scsb = new SqlConnectionStringBuilder();
            scsb.DataSource = _dataSource;
            scsb.InitialCatalog = _initialCatalog;
            scsb.PersistSecurityInfo = _persistSecurityInfo;
            scsb.UserID = _userId;
            scsb.Password = _password;

            return scsb.ConnectionString;
        }

        public bool loginUserByCode(string code)
        {
            dbcon = InitializeDb();

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

        public int?[] loginUserByNFC_Id(string nfcId)
        {
            int? UserId = null;
            int? LoginId = null;

            int?[] answers = null;

            try
            {
                // connect
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    // loading user
                    using (var cmdSearchUser = conn.CreateCommand())
                    {
                        cmdSearchUser.CommandType = CommandType.StoredProcedure;
                        cmdSearchUser.CommandText = "[dbo].[GateLogin]";
                        cmdSearchUser.Parameters.Clear();
                        cmdSearchUser.Parameters.Add("@GateId", System.Data.SqlDbType.Int).Value = Settings.GateId;
                        cmdSearchUser.Parameters.Add("@CardData", System.Data.SqlDbType.NVarChar).Value = nfcId;
                        
                        SqlParameter userIdParameter = cmdSearchUser.Parameters.Add("@UserId", System.Data.SqlDbType.Int);
                        SqlParameter userIdParameter2 = cmdSearchUser.Parameters.Add("@LoginId", System.Data.SqlDbType.Int);
                        userIdParameter.Direction = ParameterDirection.Output;                        
                        userIdParameter2.Direction = ParameterDirection.Output;                        
                        cmdSearchUser.ExecuteNonQuery();

                        UserId = cmdSearchUser.Parameters["@UserId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@UserId"].Value);
                        LoginId = cmdSearchUser.Parameters["@LoginId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@LoginId"].Value);

                        answers = new int?[2];

                        answers[0] = UserId;
                        answers[1] = LoginId;

                    }

                    conn.Close();
                }
            }
            catch (SqlException sex)
            {
                Debug.WriteLine(sex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return answers;
        }
    }
}
