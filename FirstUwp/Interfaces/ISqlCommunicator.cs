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
        /// <summary>
        /// Ez a függvény visszaadja annak a felhasználónak az azonosítóját, akit a kártya alapján belépésre jogosultnak talált a rendszer
        /// Ha nincs ilyen felhasználó, akkor értéke null
        /// </summary>
        /// <param name="nfcId"></param>
        /// <returns></returns>
        User loginUserByNFC_Id(string nfcId);
        User loginUserByCode(string code);
    }
}
