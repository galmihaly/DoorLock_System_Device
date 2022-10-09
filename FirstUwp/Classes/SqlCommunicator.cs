using FirstUwp.Interfaces;
using FirstUwp.Models;
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
using User = FirstUwp.Models.User;

namespace FirstUwp.Classes
{
    internal class SqlCommunicator : ICommunicator
    {

        /// <summary>
        /// Egy darab Connection Stringet használunk az adatbázishoz
        /// </summary>
        //private string _dataSource = @"172.16.1.6\SQLEXPRESS,1433";
        private string _dataSource = @"192.168.1.69\SQLEXPRESS,1433";
        private string _initialCatalog = "test";
        private bool _persistSecurityInfo = true;
        private string _userId = "sa";
        private string _password = "0207";
        
        SqlConnectionStringBuilder scsb;

        int? UserId = null;
        int? LoginId = null;
        bool isActive = false;

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

        
        public User loginUserByCode(string code)
        {
            User user = null;
            UserId = null;
            try
            {
                // connect
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    user = new User();

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
                        SqlParameter activeParameter = cmdSearchUser.Parameters.Add("@isActive", System.Data.SqlDbType.Bit);
                        userIdParameter.Direction = ParameterDirection.Output;
                        loginIdParameter.Direction = ParameterDirection.Output;
                        activeParameter.Direction = ParameterDirection.Output;
                        cmdSearchUser.ExecuteNonQuery();

                        UserId = cmdSearchUser.Parameters["@UserId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@UserId"].Value);
                        LoginId = cmdSearchUser.Parameters["@LoginId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@LoginId"].Value);
                        isActive = cmdSearchUser.Parameters["@isActive"].Value == DBNull.Value ? (bool)false : System.Convert.ToBoolean(cmdSearchUser.Parameters["@isActive"].Value);

                        //Debug.WriteLine("LC UserId: " + UserId);
                        //Debug.WriteLine("LC LoginId: " + LoginId);
                        Debug.WriteLine("LC isActive: " + isActive);
                        user.IsActive = isActive;

                    }

                    if (UserId != null)
                    {
                        using (var cmdGetUser = conn.CreateCommand())
                        {
                            cmdGetUser.CommandType = CommandType.Text;
                            cmdGetUser.CommandText = $"Select Id, Name, Account, Password, Barcode, Address, Active From dbo.Users Where Id = {UserId}";
                            using (SqlDataReader reader = cmdGetUser.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    user.Id = reader.GetInt32(0);
                                    user.Name = reader.GetString(1);
                                    user.Account = reader.GetString(2);
                                    user.Password = reader.GetString(3);
                                    user.Barcode = reader.GetString(4);
                                    user.Address = reader.GetString(5);
                                    user.Active = reader.GetBoolean(6);
                                    user.LoginId = Convert.ToInt32(LoginId);

                                    //Debug.WriteLine("UserLoginId in SqlCommunicator: " + user.LoginId);
                                }
                            }                               
                        }
                    }

                    conn.Close();
                }
            }
            catch (SqlException sex)
            {
                Debug.WriteLine(sex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

            return user;
        }

        /// <summary>
        /// Itt csatlakozunk az SQL adatbázishoz és hívjuk meg a tárolt eljárást (Stored Procedure)
        /// </summary>
        /// <param name="nfcId"></param>
        /// <returns></returns>
        public User loginUserByNFC_Id(string nfcId)
        {
            User user = null;
            UserId = null;
            try
            {
                // connect
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    user = new User();

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
                        SqlParameter activeParameter = cmdSearchUser.Parameters.Add("@isActive", System.Data.SqlDbType.Bit);
                        userIdParameter.Direction = ParameterDirection.Output;
                        loginIdParameter.Direction = ParameterDirection.Output;
                        activeParameter.Direction = ParameterDirection.Output;
                        cmdSearchUser.ExecuteNonQuery();

                        UserId = cmdSearchUser.Parameters["@UserId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@UserId"].Value);
                        LoginId = cmdSearchUser.Parameters["@LoginId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@LoginId"].Value);
                        isActive = cmdSearchUser.Parameters["@isActive"].Value == DBNull.Value ? (bool)false : System.Convert.ToBoolean(cmdSearchUser.Parameters["@isActive"].Value);

                        Debug.WriteLine("LC UserId: " + UserId);
                        Debug.WriteLine("LC LoginId: " + LoginId);
                        Debug.WriteLine("LC isActive: " + isActive);
                        user.IsActive = isActive;
                    }

                    if (UserId != null)
                    {
                        using (var cmdGetUser = conn.CreateCommand())
                        {
                            cmdGetUser.CommandType = CommandType.Text;
                            cmdGetUser.CommandText = $"Select Id, Name, Account, Password, Barcode, Address, Active From dbo.Users Where Id = {UserId}";
                            using (SqlDataReader reader = cmdGetUser.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    user.Id = reader.GetInt32(0);
                                    user.Name = reader.GetString(1);
                                    user.Account = reader.GetString(2);
                                    user.Password = reader.GetString(3);
                                    user.Barcode = reader.GetString(4);
                                    user.Address = reader.GetString(5);
                                    user.Active = reader.GetBoolean(6);
                                    user.LoginId = Convert.ToInt32(LoginId);
                                }
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (SqlException sex)
            {
                Debug.WriteLine(sex.Message);
                return null;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return null;
            }

            return user;
        }
    }
}
