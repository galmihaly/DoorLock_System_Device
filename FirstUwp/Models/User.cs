using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirstUwp.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string Barcode { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public int LoginId { get; set; }
    }
}
