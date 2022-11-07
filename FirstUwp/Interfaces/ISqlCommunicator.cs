using FirstUwp.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Interfaces
{
    public interface ICommunicator
    {
        User loginUserByNFC_Id(string nfcId);
        User loginUserByCode(string code);
    }
}
