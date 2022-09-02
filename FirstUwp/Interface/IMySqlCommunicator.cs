using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Interfaces
{
    internal interface IMySqlCommunicator
    {
        string loginUserByNFC_Id(string nfcId);
        string loginUserByCode(string code);
    }
}
