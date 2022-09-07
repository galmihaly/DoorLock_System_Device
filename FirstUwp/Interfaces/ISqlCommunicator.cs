using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Interfaces
{
    internal interface ISqlCommunicator
    {
        SqlConnection InitializeDb();
        /// <summary>
        /// Ez a függvény visszaadja annak a felhasználónak az azonosítóját, akit a kártya alapján belépésre jogosultnak talált a rendszer
        /// Ha nincs ilyen felhasználó, akkor értéke null
        /// </summary>
        /// <param name="nfcId"></param>
        /// <returns></returns>
        int? loginUserByNFC_Id(string nfcId);
        bool loginUserByCode(string code);
    }
}
