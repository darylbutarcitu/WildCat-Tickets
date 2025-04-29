using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildCat_Tickets
{
    public abstract class Account
    {
       // Singleton instance
        private static Account _currentAccount;

        // Properties for account details
        public string Id { get; protected set; }
        public string Email { get; protected set; }
        public string Role { get; protected set; }

        // Static property to access the current logged-in account
        public static Account CurrentAccount => _currentAccount;

        // Virtual method for login (can be overridden in derived classes)
        public virtual void Login(string id, string email, string role)
        {
            Id = id;
            Email = email;
            Role = role;
            _currentAccount = this;
        }

        // Virtual method for logout (can be overridden in derived classes)
        public virtual void Logout()
        {
            _currentAccount = null;
        }

        // Method to check if an account is logged in
        public static bool IsLoggedIn()
        {
            return _currentAccount != null;
        }
    }
}
