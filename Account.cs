using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildCat_Tickets
{
    public abstract class Account
    {
        public string Id { get; set; }
        public string Email { get; set; }

        public abstract Task<string> Login(string idNumber, string password);
    }
}
