using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WildCat_Tickets
{
    public class Admin : Account
    {
        // Static property to hold the current admin instance
        public static Admin CurrentAdmin { get; private set; }

        // Override the Login method
        public override void Login(string id, string email, string role)
        {
            // Set the current admin instance
            CurrentAdmin = new Admin
            {
                Id = id,
                Email = email,
                Role = role
            };
        }

        // Override the Logout method
        public override void Logout()
        {
            if (CurrentAdmin != null)
            {
                CurrentAdmin = null; // Clear the current admin instance
            }
        }
    }
}
