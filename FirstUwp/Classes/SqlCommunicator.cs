﻿using FirstUwp.Helpers;
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
        private string _dataSource = @"server.logcontrol.hu,4241";
        private string _initialCatalog = "Galmihaly";
        private bool _persistSecurityInfo = true;
        private string _userId = "Galmihaly";
        private string _password = "Gm2022!!!";
        
        SqlConnectionStringBuilder scsb;

        int? UserId = null;
        int? LoginId = null;
        int? isActive = null;

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
            try
            {
                // connect
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();

                    user = new User();

                    string macAddress2 = LocalSettingsHelper.MACAddressConnectWidthCharachter(LocalSettingsHelper.GetMacAddress(), '-');
                    Debug.WriteLine("MAC address:" + macAddress2);

                    using (var cmdSearchUser = conn.CreateCommand())
                    {
                        cmdSearchUser.CommandType = CommandType.StoredProcedure;
                        cmdSearchUser.CommandText = "[dbo].[GateLogin]";
                        cmdSearchUser.Parameters.Clear();
                        cmdSearchUser.Parameters.Add("@GateId", System.Data.SqlDbType.NVarChar).Value = macAddress2;
                        cmdSearchUser.Parameters.Add("@CardData", System.Data.SqlDbType.NVarChar).Value = "";
                        cmdSearchUser.Parameters.Add("@InputCodeData", System.Data.SqlDbType.NVarChar).Value = code;

                        SqlParameter userIdParameter = cmdSearchUser.Parameters.Add("@UserId", System.Data.SqlDbType.Int);
                        SqlParameter loginIdParameter = cmdSearchUser.Parameters.Add("@LoginId", System.Data.SqlDbType.Int);
                        SqlParameter activeParameter = cmdSearchUser.Parameters.Add("@isActive", System.Data.SqlDbType.Int);
                        userIdParameter.Direction = ParameterDirection.Output;
                        loginIdParameter.Direction = ParameterDirection.Output;
                        activeParameter.Direction = ParameterDirection.Output;
                        cmdSearchUser.ExecuteNonQuery();

                        UserId = cmdSearchUser.Parameters["@UserId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@UserId"].Value);
                        LoginId = cmdSearchUser.Parameters["@LoginId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@LoginId"].Value);
                        isActive = cmdSearchUser.Parameters["@isActive"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@isActive"].Value);

                        /*
                        Debug.WriteLine("LC UserId: " + UserId);
                        Debug.WriteLine("LC LoginId: " + LoginId);
                        Debug.WriteLine("LC isActive: " + isActive);
                        */


                        user.messageCode = (int)isActive;
                    }

                    if (UserId != null)
                    {
                        using (var cmdGetUser = conn.CreateCommand())
                        {
                            cmdGetUser.CommandType = CommandType.Text;
                            cmdGetUser.CommandText = $"Select Name, Active From dbo.Users Where Id = {UserId}";
                            using (SqlDataReader reader = cmdGetUser.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    user.Name = reader.GetString(0);
                                    user.Active = reader.GetBoolean(1);
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

        public User loginUserByNFC_Id(string nfcId)
        {
            User user = null;
            try
            {
                using (var conn = new SqlConnection(GetConnectionString()))
                {
                    conn.Open();
                    user = new User();

                    string macAddress2 = LocalSettingsHelper.MACAddressConnectWidthCharachter(LocalSettingsHelper.GetMacAddress(), '-');
                    Debug.WriteLine("MAC address:" + macAddress2);

                    using (var cmdSearchUser = conn.CreateCommand())
                    {
                        cmdSearchUser.CommandType = CommandType.StoredProcedure;
                        cmdSearchUser.CommandText = "[dbo].[GateLogin]";
                        cmdSearchUser.Parameters.Clear();
                        cmdSearchUser.Parameters.Add("@GateId", System.Data.SqlDbType.NVarChar).Value = macAddress2;
                        cmdSearchUser.Parameters.Add("@CardData", System.Data.SqlDbType.NVarChar).Value = nfcId;
                        cmdSearchUser.Parameters.Add("@InputCodeData", System.Data.SqlDbType.NVarChar).Value = "";

                        SqlParameter userIdParameter = cmdSearchUser.Parameters.Add("@UserId", System.Data.SqlDbType.Int);
                        SqlParameter loginIdParameter = cmdSearchUser.Parameters.Add("@LoginId", System.Data.SqlDbType.Int);
                        SqlParameter activeParameter = cmdSearchUser.Parameters.Add("@isActive", System.Data.SqlDbType.Int);
                        userIdParameter.Direction = ParameterDirection.Output;
                        loginIdParameter.Direction = ParameterDirection.Output;
                        activeParameter.Direction = ParameterDirection.Output;
                        cmdSearchUser.ExecuteNonQuery();

                        UserId = cmdSearchUser.Parameters["@UserId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@UserId"].Value);
                        LoginId = cmdSearchUser.Parameters["@LoginId"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@LoginId"].Value);
                        isActive = cmdSearchUser.Parameters["@isActive"].Value == DBNull.Value ? (int?)null : System.Convert.ToInt32(cmdSearchUser.Parameters["@isActive"].Value);

                        /*
                        Debug.WriteLine("LN UserId: " + UserId);
                        Debug.WriteLine("LN LoginId: " + LoginId);
                        Debug.WriteLine("LN isActive: " + isActive);
                        */


                        user.messageCode = (int)isActive;
                    }

                    if (UserId != null)
                    {
                        using (var cmdGetUser = conn.CreateCommand())
                        {
                            cmdGetUser.CommandType = CommandType.Text;
                            cmdGetUser.CommandText = $"Select Name, Active From dbo.Users Where Id = {UserId}";
                            using (SqlDataReader reader = cmdGetUser.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    user.Name = reader.GetString(0);
                                    user.Active = reader.GetBoolean(1);
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