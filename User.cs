using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WildCat_Tickets
{
    public class User : Account
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ProfileUrl { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string Program { get; set; }
        public string Year { get; set; }
    }
}
