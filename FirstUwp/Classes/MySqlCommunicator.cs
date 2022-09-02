using FirstUwp.Interfaces;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Classes
{
    internal class MySqlCommunicator : IMySqlCommunicator
    {
        private string conStr;

        MySqlConnection msc = new MySqlConnection();
        public string loginUserByCode(string code)
        {
            throw new NotImplementedException();
        }

        public string loginUserByNFC_Id(string nfcId)
        {
            throw new NotImplementedException();  //
        }
    }
}
