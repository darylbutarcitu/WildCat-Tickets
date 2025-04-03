using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WildCat_Tickets
{
    public class Admin : Account
    {
        public string Role { get; set; }

        // Implement the abstract Login method from Person class
        public override async Task<string> Login(string idNumber, string password)
        {
            // Implementation of the login logic
            return await Task.FromResult("Login successful");
        }

        //// Add a movie to the database
        //public bool AddMovie(string movieId, string title, string genre, string director, DateTime releaseDate)
        //{
        //    //try
        //    //{
        //    //    var result = FireBaseHelper.AddMovie(movieId, title, genre, director, releaseDate).Result;
        //    //    return result;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Console.WriteLine("Error adding movie: " + ex.Message);
        //    //    return false;
        //    //}
        //}

        //// Add a showtime to the database
        //public bool AddShowTime(string showTimeId, string movieId, DateTime showTime)
        //{
        //    //try
        //    //{
        //    //    var result = FireBaseHelper.AddShowTime(showTimeId, movieId, showTime).Result;
        //    //    return result;
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    Console.WriteLine("Error adding showtime: " + ex.Message);
        //    //    return false;
        //    //}
        //}
    }
}
