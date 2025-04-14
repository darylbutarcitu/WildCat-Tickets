using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;
using System.Security.Cryptography;

namespace WildCat_Tickets
{
    internal class DatabaseHelper
    {
        public static void InitializeDatabase()
        {
            string dbFile = "wildcattickets.db";

            if (!File.Exists(dbFile))
            {
                SQLiteConnection.CreateFile(dbFile);
                MessageBox.Show("Database file created successfully.");
            }

            using (var conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
            {
                conn.Open();

                // Enable WAL mode
                using (var cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Check and create the Users table if it doesn't exist
                string createUsersTableQuery = @"
                CREATE TABLE IF NOT EXISTS Users (
                    IDNumber TEXT PRIMARY KEY,
                    FirstName TEXT,
                    MiddleName TEXT,
                    LastName TEXT,
                    BirthDate TEXT,
                    Program TEXT,
                    Year TEXT,
                    Phone TEXT,
                    Email TEXT UNIQUE,
                    Password TEXT,
                    ProfilePhotoPath TEXT
                    Role TEXT
                );";

                using (var cmd = new SQLiteCommand(createUsersTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Check and create the Movies table if it doesn't exist
                string createMoviesTableQuery = @"
                CREATE TABLE IF NOT EXISTS Movies (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Title TEXT NOT NULL,
                    Duration TEXT NOT NULL,
                    Genre TEXT NOT NULL,
                    Rating TEXT NOT NULL,
                    ReleaseDate TEXT NOT NULL,
                    Description TEXT NOT NULL,
                    PosterPath TEXT NOT NULL,
                    Status TEXT NOT NULL,
                    NumberOfRatings INTEGER DEFAULT 0,
                    TotalRatings INTEGER DEFAULT 0
                );";

                using (var cmd = new SQLiteCommand(createMoviesTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Check and create the Venues table if it doesn't exist
                string createVenuesTableQuery = @"
                CREATE TABLE IF NOT EXISTS Venues (
                    VenueID INTEGER PRIMARY KEY AUTOINCREMENT,
                    Name TEXT NOT NULL,
                    Location TEXT NOT NULL,
                    SeatCapacity INTEGER NOT NULL
                );";

                using (var cmd = new SQLiteCommand(createVenuesTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Check and create the Showtimes table if it doesn't exist
                string createShowtimesTableQuery = @"
                CREATE TABLE IF NOT EXISTS Showtimes (
                    ShowtimeID INTEGER PRIMARY KEY AUTOINCREMENT,
                    MovieID INTEGER NOT NULL,
                    VenueID INTEGER NOT NULL,
                    StartTime DATETIME NOT NULL,
                    EndTime DATETIME NOT NULL,
                    TicketPrice REAL NOT NULL,
                    FOREIGN KEY (MovieID) REFERENCES Movies(Id),
                    FOREIGN KEY (VenueID) REFERENCES Venues(VenueID)
                );";

                using (var cmd = new SQLiteCommand(createShowtimesTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Check and create the Bookings table if it doesn't exist
                string createBookingsTableQuery = @"
                CREATE TABLE IF NOT EXISTS Bookings (
                    BookingID INTEGER PRIMARY KEY AUTOINCREMENT,
                    UserID TEXT NOT NULL,
                    ShowtimeID INTEGER NOT NULL,
                    SeatNumber TEXT NOT NULL,
                    BookingTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                    FOREIGN KEY (UserID) REFERENCES Users(IDNumber),
                    FOREIGN KEY (ShowtimeID) REFERENCES Showtimes(ShowtimeID)
                );";

                using (var cmd = new SQLiteCommand(createBookingsTableQuery, conn))
                {
                    cmd.ExecuteNonQuery();
                }

                conn.Close();
            }

        }

        internal static string HashPassword(string password)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
