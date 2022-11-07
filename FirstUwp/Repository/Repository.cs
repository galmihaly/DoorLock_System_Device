using FirstUwp.Classes;
using FirstUwp.Interfaces;
using FirstUwp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace FirstUwp.Repository
{
    public static class Repository
    {
        public enum CommunicatorTypeEnum
        {
            MsSqlServer,
            Oracle,
            MySql,
            Xml,
            TextFile
        }

        public static CommunicatorTypeEnum CommunicatorType = CommunicatorTypeEnum.MsSqlServer;
        public static ICommunicator Communicator = null;

        public static User LoggedInUser = null;

        /*public static string VersionInfo
        {
            get
            {
                return "1.0";
            }
        }*/


        public static bool Initialize()
        {
            switch(CommunicatorType)
            {
                case CommunicatorTypeEnum.MsSqlServer:
                    Communicator = new SqlCommunicator();
                    return true;

                default:
                    throw new Exception("A megadott interfész még nincs implementálva!");
                    break;
            }
            
            return false;
        }
    }
}
