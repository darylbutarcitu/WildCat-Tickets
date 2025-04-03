using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WildCat_Tickets
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool isAdmin = false;
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            // Run Login Form
            // Authenticate user

            if (isAdmin)
            {

            }
            else
            {
                string idNumber = "24-5865-705";
                string password = "09261999!Db";
                Customer customer = new Customer();
                customer.Login(idNumber, password).GetAwaiter().GetResult();

                MessageBox.Show("Login successful!", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // Proceed to the next form or main application window
                Application.Run(new CustomerDashboardForm(customer));
            }
            //Application.Run(new MoviesForm());
        }
    }
}
