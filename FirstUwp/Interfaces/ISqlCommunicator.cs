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
        bool loginUserByNFC_Id(string nfcId);
        bool loginUserByCode(string code);
    }
}
