using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace WildCat_Tickets
{
    public class Guest
    {
        //public bool RegisterAccount(string idNumber, string email, string password)
        //{
        //    try
        //    {
        //        if (!IsValidIdNumber(idNumber))
        //        {
        //            Console.WriteLine("Invalid ID number format.");
        //            return false;
        //        }

        //        // Implement the registration logic here
        //        // For example, you can call a method from FireBaseHelper to register the account
        //        var result = FireBaseHelper.RegisterUser(idNumber, email, password).Result;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error registering account: " + ex.Message);
        //        return false;
        //    }
        //}

        //private bool IsValidIdNumber(string idNumber)
        //{
        //    // Validate the ID number format (xx-xxxx-xxx)
        //    string pattern = @"^\d{2}-\d{4}-\d{3}$";
        //    return Regex.IsMatch(idNumber, pattern);
        //}
    }
}
