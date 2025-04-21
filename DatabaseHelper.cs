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

                // Enable Write-Ahead Logging (WAL) mode
                using (var cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
                {
                    cmd.ExecuteNonQuery();
                }

                // Ensure all tables are created
                CreateTables(conn);

                conn.Close();
            }
        }

        private static void CreateTables(SQLiteConnection conn)
        {
            string[] tableCreationQueries = new string[]
            {
            @"
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
                ProfilePhotoPath TEXT,
                Role TEXT
            );",
            @"
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
            );",
            @"
            CREATE TABLE IF NOT EXISTS Venues (
                VenueID INTEGER PRIMARY KEY AUTOINCREMENT,
                Name TEXT NOT NULL,
                Location TEXT NOT NULL,
                SeatCapacity INTEGER NOT NULL
            );",
            @"
            CREATE TABLE IF NOT EXISTS Showtimes (
                ShowtimeID INTEGER PRIMARY KEY AUTOINCREMENT,
                MovieID INTEGER NOT NULL,
                VenueID INTEGER NOT NULL,
                StartTime DATETIME NOT NULL,
                EndTime DATETIME NOT NULL,
                TicketPrice REAL NOT NULL,
                FOREIGN KEY (MovieID) REFERENCES Movies(Id),
                FOREIGN KEY (VenueID) REFERENCES Venues(VenueID)
            );",
            @"
            CREATE TABLE IF NOT EXISTS Seats (
                SeatID INTEGER PRIMARY KEY AUTOINCREMENT,
                VenueID INTEGER NOT NULL,
                SeatNumber TEXT NOT NULL UNIQUE,
                FOREIGN KEY (VenueID) REFERENCES Venues(VenueID)
            );",
            @"
            CREATE TABLE IF NOT EXISTS Bookings (
                BookingID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID TEXT NOT NULL,
                ShowtimeID INTEGER NOT NULL,
                SeatNumber TEXT NOT NULL,
                BookingTime DATETIME DEFAULT CURRENT_TIMESTAMP,
                FOREIGN KEY (UserID) REFERENCES Users(IDNumber),
                FOREIGN KEY (ShowtimeID) REFERENCES Showtimes(ShowtimeID)
            );",
            @"
            CREATE TABLE IF NOT EXISTS Ratings (
                RatingID INTEGER PRIMARY KEY AUTOINCREMENT,
                UserID TEXT NOT NULL,
                MovieID INTEGER NOT NULL,
                Rating INTEGER NOT NULL CHECK (Rating >= 1 AND Rating <= 5),
                FOREIGN KEY (UserID) REFERENCES Users(IDNumber),
                FOREIGN KEY (MovieID) REFERENCES Movies(Id)
            );"
            };

            foreach (var query in tableCreationQueries)
            {
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public static int GetShowtimeID(string selectedShowtime, int movieId)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // Enable Write-Ahead Logging (WAL) mode
                    using (SQLiteCommand walCmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
                    {
                        walCmd.ExecuteNonQuery();
                    }

                    string query = @"
                        SELECT ShowtimeID 
                        FROM Showtimes 
                        WHERE MovieID = @movieId 
                          AND StartTime = @startTime";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@movieId", movieId);

                        // Extract the StartTime from the selected dropdown item
                        string startTimeString = selectedShowtime.Split('-')[0].Trim(); // Extract StartTime
                        DateTime startTime = DateTime.Parse(startTimeString);

                        cmd.Parameters.AddWithValue("@startTime", startTime.ToString("yyyy-MM-dd HH:mm:ss"));

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            return Convert.ToInt32(result);
                        }
                        else
                        {
                            throw new Exception("Showtime ID not found for the selected showtime.");
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1; // Return an invalid ID to indicate failure
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching Showtime ID: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return -1; // Return an invalid ID to indicate failure
            }
        }

        public static Dictionary<string, int> GetTotalBookingsByMovie(string genre = "Any", string releaseYear = "Any", int maxResults = 10)
        {
            var totalBookingsByMovie = new Dictionary<string, int>();

            string query = @"
            SELECT 
                Movies.Title, 
                COUNT(Bookings.BookingID) AS TotalBookings
            FROM 
                Movies
            LEFT JOIN 
                Showtimes ON Movies.Id = Showtimes.MovieID
            LEFT JOIN 
                Bookings ON Showtimes.ShowtimeID = Bookings.ShowtimeID
            WHERE 
                (@Genre = 'Any' OR Movies.Genre = @Genre)
                AND (@ReleaseYear = 'Any' OR strftime('%Y', Movies.ReleaseDate) = @ReleaseYear)
            GROUP BY 
                Movies.Title
            ORDER BY 
                TotalBookings DESC
            LIMIT 
                @MaxResults";

            try
            {
                using (var conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();
                    using (var cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Genre", genre);
                        cmd.Parameters.AddWithValue("@ReleaseYear", releaseYear);
                        cmd.Parameters.AddWithValue("@MaxResults", maxResults);

                        using (var reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string title = reader["Title"].ToString();
                                int totalBookings = Convert.ToInt32(reader["TotalBookings"]);
                                totalBookingsByMovie[title] = totalBookings;
                            }
                        }
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching total bookings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return totalBookingsByMovie;
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