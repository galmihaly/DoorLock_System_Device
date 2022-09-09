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
using Ubiety.Dns.Core;
using UnitsNet;
using Windows.System;

namespace FirstUwp.Classes
{
    internal class SqlCommunicator : ISqlCommunicator
    {

        /// <summary>
        /// Egy darab Connection Stringet használunk az adatbázishoz
        /// </summary>
        private string _dataSource = @"172.16.1.6\SQLEXPRESS";
        private string _initialCatalog = "test";
        private bool _persistSecurityInfo = true;
        private string _userId = "sa";
        private string _password = "0207";
        
        SqlConnection dbcon;
        SqlConnectionStringBuilder scsb;

        int? UserId = null;
        int? LoginId = null;
        int?[] answers = null;

        /// <summary>
        /// Itt adjuk vissza a Connection Stringet
        /// </summary>
        /// <returns></returns>
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
                        cmdSearchUser.Parameters.Add("@CardData", System.Data.SqlDbType.NVarChar).Value = "";
                        cmdSearchUser.Parameters.Add("@InputCodeData", System.Data.SqlDbType.NVarChar).Value = code;

                        SqlParameter userIdParameter = cmdSearchUser.Parameters.Add("@UserId", System.Data.SqlDbType.Int);
                        SqlParameter loginIdParameter = cmdSearchUser.Parameters.Add("@LoginId", System.Data.SqlDbType.Int);
                        userIdParameter.Direction = ParameterDirection.Output;
                        loginIdParameter.Direction = ParameterDirection.Output;
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

            return true;
        }

        /// <summary>
        /// Itt csatlakozunk az SQL adatbázishoz és hívjuk meg a tárolt eljárást (Stored Procedure)
        /// </summary>
        /// <param name="nfcId"></param>
        /// <returns></returns>
        public int?[] loginUserByNFC_Id(string nfcId)
        {

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
                        cmdSearchUser.Parameters.Add("@InputCodeData", System.Data.SqlDbType.NVarChar).Value = "";
                        
                        SqlParameter userIdParameter = cmdSearchUser.Parameters.Add("@UserId", System.Data.SqlDbType.Int);
                        SqlParameter loginIdParameter = cmdSearchUser.Parameters.Add("@LoginId", System.Data.SqlDbType.Int);
                        userIdParameter.Direction = ParameterDirection.Output;                        
                        loginIdParameter.Direction = ParameterDirection.Output;                        
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
