using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Interfaces
{
    internal interface ISqlCommunicator
    {
        void InitializeDb();
        string loginUserByNFC_Id(string nfcId);
        string loginUserByCode(string code);
    }
}
