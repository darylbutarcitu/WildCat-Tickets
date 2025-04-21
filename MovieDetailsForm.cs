using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using Krypton.Toolkit;
using System.Data.SQLite;

namespace WildCat_Tickets
{
    public partial class MovieDetailsForm : KryptonForm
    {
        private int totalRatings, numberOfRatings, movieId;
        private string _currentUser;
        public int MovieID
        {
            set { movieId = value; }
            get { return movieId; }
        }
        public string MovieTitle
        {
            set { movieTitleTbx.Text = value; }
            get { return movieTitleTbx.Text; }
        }

        public string MovieDuration
        {
            set { durationTbx.Text = value; }
        }

        public string MovieGenre
        {
            set { genreTbx.Text = value; }
        }

        public string MovieDescription
        {
            set { movieDescriptionTbx.Text = value; }
        }

        public int MovieTotalRatings
        {
            set { totalRatings = value; }
        }

        public int MovieNumberOfRatings
        {
            set { numberOfRatings = value; }
        }

        public string MovieRating
        {
            set { ratingTbx.Text = value; }
        }

        public string MovieReleaseDate
        {
            set
            {
                if (DateTime.TryParse(value, out DateTime parsedDate))
                {
                    releaseDateTbx.Text = parsedDate.ToString("MMMM dd, yyyy");
                }
                else
                {
                    releaseDateTbx.Text = value;
                }
            }
        }

        public string MoviePosterPath
        {
            set
            {
                if (File.Exists(value))
                {
                    moviePosterBox.Image = Image.FromFile(value);
                }
            }
        }

        private double getStars()
        {
            if (numberOfRatings == 0)
            {
                return 0.0;
            }
            return Math.Round((double)totalRatings / numberOfRatings, 2);
        }

        public MovieDetailsForm(string currentUser)
        {
            InitializeComponent();
            this._currentUser = currentUser;
            // Adjust button visibility based on the user's role
            if (currentUser == "admin")
            {
                showtimeBtn.Visible = true;
                bookBtn.Visible = false;
                viewShowtimesBtn.Visible = true;
                starBtn.Visible = false;
                uploadMoviePosterBtn.Visible = true;
                saveBtn.Visible = true;
            }
            else
            {
                showtimeBtn.Visible = false;
                bookBtn.Visible = true;
                viewShowtimesBtn.Visible = false;
                starBtn.Visible = true;
                uploadMoviePosterBtn.Visible = false;
                saveBtn.Visible = false;
            }
        }

        private void MovieDetails_Load(object sender, EventArgs e)
        {
            starsTbx.Text = getStars().ToString();
            UpdateStarButton();
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private bool IsShowtimeOverlapping(SQLiteConnection conn, SQLiteTransaction transaction, int venueId, string startTime, string endTime)
        {
            string query = @"
            SELECT COUNT(*) 
            FROM Showtimes 
            WHERE VenueID = @venueId 
              AND StartTime < @endTime 
              AND EndTime > @startTime";

            using (SQLiteCommand cmd = new SQLiteCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@venueId", venueId);
                cmd.Parameters.AddWithValue("@startTime", startTime);
                cmd.Parameters.AddWithValue("@endTime", endTime);

                int conflictCount = Convert.ToInt32(cmd.ExecuteScalar());
                return conflictCount > 0;
            }
        }

        private void showtimeBtn_Click(object sender, EventArgs e)
        {
            // Create a new form
            Form addShowtimeForm = new Form
            {
                Text = "Add Showtime for CINEMA 1",
                Size = new Size(400, 300),
                StartPosition = FormStartPosition.CenterParent
            };

            // Create and configure controls
            Label dateLabel = new Label { Text = "Date:", Location = new Point(20, 20), AutoSize = true };
            DateTimePicker datePicker = new DateTimePicker { Location = new Point(100, 20), Format = DateTimePickerFormat.Short };

            Label startTimeLabel = new Label { Text = "Start Time:", Location = new Point(20, 60), AutoSize = true };
            DateTimePicker startTimePicker = new DateTimePicker { Location = new Point(100, 60), Format = DateTimePickerFormat.Time, ShowUpDown = true };

            Label endTimeLabel = new Label { Text = "End Time:", Location = new Point(20, 100), AutoSize = true };
            TextBox endTimeTbx = new TextBox { Location = new Point(100, 100), Width = 100, ReadOnly = true };

            Label ticketPriceLabel = new Label { Text = "Ticket Price:", Location = new Point(20, 140), AutoSize = true };
            TextBox ticketPriceTbx = new TextBox { Location = new Point(100, 140), Width = 100 };

            Button addShowtimeBtn = new Button
            {
                Text = "Add Showtime",
                Location = new Point(100, 200),
                Width = 100
            };

            // Update end time dynamically when start time changes
            startTimePicker.ValueChanged += (s, args) =>
            {
                if (int.TryParse(durationTbx.Text, out int movieDuration))
                {
                    DateTime startTime = startTimePicker.Value;

                    // Ensure start time is within operating hours
                    if (startTime.TimeOfDay < TimeSpan.FromHours(9) || startTime.TimeOfDay >= TimeSpan.FromHours(21))
                    {
                        MessageBox.Show("Start time must be between 9:00 AM and 9:00 PM.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        startTimePicker.Value = DateTime.Today.AddHours(9); // Reset to 9:00 AM
                        return;
                    }

                    DateTime endTime = startTime.AddMinutes(movieDuration + 15); // Add movie duration and 15 minutes allowance

                    // Ensure end time is within operating hours
                    if (endTime.TimeOfDay > TimeSpan.FromHours(21))
                    {
                        MessageBox.Show("End time exceeds operating hours (9:00 PM). Please adjust the start time.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        endTimeTbx.Text = "Invalid Time";
                    }
                    else
                    {
                        endTimeTbx.Text = endTime.ToString("hh:mm tt"); // Display in 12-hour format with AM/PM
                    }
                }
                else
                {
                    endTimeTbx.Text = "Invalid Duration";
                }
            };

            // Add click event for the button
            addShowtimeBtn.Click += (s, args) =>
            {
                string date = datePicker.Value.ToString("yyyy-MM-dd");
                string startTime = startTimePicker.Value.ToString("HH:mm:ss"); // 24-hour format for SQLite
                string endTime = DateTime.Parse(endTimeTbx.Text).ToString("HH:mm:ss"); // Convert to 24-hour format
                string ticketPriceText = ticketPriceTbx.Text.Trim();
                decimal ticketPrice;

                // Combine date and start time to check if it's in the future
                DateTime selectedStartDateTime = DateTime.Parse($"{date} {startTime}");

                if (selectedStartDateTime <= DateTime.Now)
                {
                    MessageBox.Show("The showtime must be in the future.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (!decimal.TryParse(ticketPriceText, out ticketPrice) || ticketPrice <= 0)
                {
                    MessageBox.Show("Please enter a valid ticket price.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                    {
                        conn.Open();

                        // Enable WAL mode
                        using (SQLiteCommand cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
                        {
                            cmd.ExecuteNonQuery();
                        }

                        using (var transaction = conn.BeginTransaction())
                        {
                            // Retrieve VenueID for "Cinema1"
                            string getVenueIdQuery = "SELECT VenueID FROM Venues WHERE Name = 'Cinema1'";
                            int venueId;
                            using (SQLiteCommand cmd = new SQLiteCommand(getVenueIdQuery, conn, transaction))
                            {
                                object result = cmd.ExecuteScalar();
                                if (result == null)
                                {
                                    MessageBox.Show("Cinema1 does not exist in the database.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    transaction.Rollback();
                                    return;
                                }
                                venueId = Convert.ToInt32(result);
                            }

                            // Check for overlapping showtimes
                            string startDateTime = $"{date} {startTime}"; // Combine date and time
                            string endDateTime = $"{date} {endTime}";     // Combine date and time
                            if (IsShowtimeOverlapping(conn, transaction, venueId, startDateTime, endDateTime))
                            {
                                MessageBox.Show("A conflicting showtime already exists.", "Conflict Detected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                transaction.Rollback();
                                return;
                            }

                            // Insert the new showtime
                            string insertQuery = @"
                INSERT INTO Showtimes (MovieID, VenueID, StartTime, EndTime, TicketPrice) 
                VALUES (@movieId, @venueId, @startTime, @endTime, @ticketPrice)";

                            using (SQLiteCommand insertCmd = new SQLiteCommand(insertQuery, conn, transaction))
                            {
                                insertCmd.Parameters.AddWithValue("@movieId", this.movieId); // Replace with the actual MovieID
                                insertCmd.Parameters.AddWithValue("@venueId", venueId);
                                insertCmd.Parameters.AddWithValue("@startTime", startDateTime);
                                insertCmd.Parameters.AddWithValue("@endTime", endDateTime);
                                insertCmd.Parameters.AddWithValue("@ticketPrice", ticketPrice);

                                insertCmd.ExecuteNonQuery();
                            }

                            transaction.Commit();
                            MessageBox.Show("Showtime added successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            addShowtimeForm.Close();
                        }
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error adding showtime: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Add controls to the form
            addShowtimeForm.Controls.Add(dateLabel);
            addShowtimeForm.Controls.Add(datePicker);
            addShowtimeForm.Controls.Add(startTimeLabel);
            addShowtimeForm.Controls.Add(startTimePicker);
            addShowtimeForm.Controls.Add(endTimeLabel);
            addShowtimeForm.Controls.Add(endTimeTbx);
            addShowtimeForm.Controls.Add(ticketPriceLabel);
            addShowtimeForm.Controls.Add(ticketPriceTbx);
            addShowtimeForm.Controls.Add(addShowtimeBtn);

            // Show the form as a dialog
            addShowtimeForm.ShowDialog();
        }

        private void starBtn_Click(object sender, EventArgs e)
        {
            // Create a new form for rating input
            Form ratingForm = new Form
            {
                Text = "Rate the Movie",
                Size = new Size(300, 200),
                StartPosition = FormStartPosition.CenterParent
            };

            // Create and configure controls
            Label ratingLabel = new Label
            {
                Text = "Enter your rating (1 to 5):",
                Location = new Point(20, 20),
                AutoSize = true
            };

            NumericUpDown ratingInput = new NumericUpDown
            {
                Minimum = 1,
                Maximum = 5,
                Location = new Point(20, 50),
                Width = 50
            };

            Button submitButton = new Button
            {
                Text = "Submit",
                Location = new Point(20, 100),
                Width = 80
            };

            Button cancelButton = new Button
            {
                Text = "Cancel",
                Location = new Point(120, 100),
                Width = 80
            };

            // Add controls to the form
            ratingForm.Controls.Add(ratingLabel);
            ratingForm.Controls.Add(ratingInput);
            ratingForm.Controls.Add(submitButton);
            ratingForm.Controls.Add(cancelButton);

            // Handle the Submit button click
            submitButton.Click += (s, args) =>
            {
                int rating = (int)ratingInput.Value;

                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                    {
                        conn.Open();

                        using (var transaction = conn.BeginTransaction())
                        {
                            // Insert the new rating into the Ratings table
                            string insertQuery = @"
                                INSERT INTO Ratings (UserID, MovieID, Rating) 
                                VALUES (@userID, @movieID, @rating)";

                            using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@userID", _currentUser);
                                cmd.Parameters.AddWithValue("@movieID", MovieID);
                                cmd.Parameters.AddWithValue("@rating", rating);

                                cmd.ExecuteNonQuery();
                            }

                            // Update the Movies table to increment NumberOfRatings and add the new rating to TotalRatings
                            string updateMovieQuery = @"
                                UPDATE Movies
                                SET NumberOfRatings = NumberOfRatings + 1,
                                    TotalRatings = TotalRatings + @rating
                                WHERE Id = @movieID";

                            using (SQLiteCommand cmd = new SQLiteCommand(updateMovieQuery, conn, transaction))
                            {
                                cmd.Parameters.AddWithValue("@rating", rating);
                                cmd.Parameters.AddWithValue("@movieID", MovieID);

                                cmd.ExecuteNonQuery();
                            }

                            // Commit the transaction
                            transaction.Commit();
                        }

                        // Update the star button after successful submission
                        MessageBox.Show("Rating submitted successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        UpdateStarButton();
                        ratingForm.Close();
                    }
                }
                catch (SQLiteException ex)
                {
                    MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error submitting rating: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            };

            // Handle the Cancel button click
            cancelButton.Click += (s, args) =>
            {
                ratingForm.Close();
            };

            // Show the form as a dialog
            ratingForm.ShowDialog();
        }

        private void viewShowtimesBtn_Click(object sender, EventArgs e)
        {
            // Validate the movie title input
            if (string.IsNullOrWhiteSpace(movieTitleTbx.Text))
            {
                MessageBox.Show("Please enter or select a movie title.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Create a new form to display showtimes
            Form showtimesForm = new Form
            {
                Text = "Showtimes",
                Size = new Size(600, 400),
                StartPosition = FormStartPosition.CenterParent
            };

            // Create a DataGridView to display the showtimes
            DataGridView showtimesGrid = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                ReadOnly = true,
                AllowUserToAddRows = false
            };

            // Add the DataGridView to the form
            showtimesForm.Controls.Add(showtimesGrid);

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // Enable WAL mode
                    using (SQLiteCommand cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    // Query to fetch showtimes for the current movie
                    string query = @"
                    SELECT s.ShowtimeID, v.Name AS Venue, s.StartTime, s.EndTime, s.TicketPrice
                    FROM Showtimes s
                    INNER JOIN Venues v ON s.VenueID = v.VenueID
                    WHERE s.MovieID = (SELECT Id FROM Movies WHERE Title = @MovieTitle COLLATE NOCASE)";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@MovieTitle", movieTitleTbx.Text.Trim());

                        using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(cmd))
                        {
                            DataTable showtimesTable = new DataTable();
                            adapter.Fill(showtimesTable);

                            if (showtimesTable.Rows.Count == 0)
                            {
                                MessageBox.Show("No showtimes found for the selected movie.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return;
                            }

                            // Add formatted columns for Date, Start Time, and End Time
                            showtimesTable.Columns.Add("FormattedDate", typeof(string));
                            showtimesTable.Columns.Add("FormattedStartTime", typeof(string));
                            showtimesTable.Columns.Add("FormattedEndTime", typeof(string));

                            foreach (DataRow row in showtimesTable.Rows)
                            {
                                // Parse the StartTime and EndTime to ensure they are valid DateTime objects
                                if (DateTime.TryParse(row["StartTime"].ToString(), out DateTime startTime))
                                {
                                    row["FormattedDate"] = startTime.ToString("MMMM dd, yyyy"); // Add the date
                                    row["FormattedStartTime"] = startTime.ToString("hh:mm tt"); // Add the time
                                }

                                if (DateTime.TryParse(row["EndTime"].ToString(), out DateTime endTime))
                                {
                                    row["FormattedEndTime"] = endTime.ToString("hh:mm tt"); // Add the time
                                }
                            }

                            // Remove original StartTime and EndTime columns from the grid and display the formatted ones
                            showtimesTable.Columns.Remove("StartTime");
                            showtimesTable.Columns.Remove("EndTime");

                            // Rename the formatted columns for display purposes
                            showtimesTable.Columns["FormattedDate"].ColumnName = "Date";
                            showtimesTable.Columns["FormattedStartTime"].ColumnName = "Start Time";
                            showtimesTable.Columns["FormattedEndTime"].ColumnName = "End Time";

                            // Bind the DataTable to the DataGridView
                            showtimesGrid.DataSource = showtimesTable;
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
                MessageBox.Show("Error fetching showtimes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Show the form as a dialog
            showtimesForm.ShowDialog();
        }

        private void uploadMoviePosterBtn_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Title = "Select a Movie Poster",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFile = openFileDialog.FileName;

                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                    {
                        conn.Open();
                        string updateQuery = "UPDATE Movies SET PosterPath = @posterPath WHERE Id = @movieId";
                        using (SQLiteCommand cmd = new SQLiteCommand(updateQuery, conn))
                        {
                            cmd.Parameters.AddWithValue("@posterPath", selectedFile);
                            cmd.Parameters.AddWithValue("@movieId", this.MovieID);

                            int rowsAffected = cmd.ExecuteNonQuery();
                            if (rowsAffected > 0)
                            {
                                // Update the movie poster in the UI if the file exists
                                if (File.Exists(selectedFile))
                                {
                                    moviePosterBox.Image = Image.FromFile(selectedFile);
                                }
                                MessageBox.Show("Movie poster updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                MessageBox.Show("Failed to update movie poster.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error updating movie poster: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void bookBtn_Click(object sender, EventArgs e)
        {
            CreateBookingForm bookMovieForm = new CreateBookingForm(_currentUser)
            {
                MovieID = this.MovieID,
                MovieTitle = this.MovieTitle
            };

            bookMovieForm.ShowDialog();
        }

        private void UpdateStarButton()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // Check if the user has already rated the movie
                    string userRatingQuery = @"
                        SELECT Rating 
                        FROM Ratings 
                        WHERE UserID = @userID AND MovieID = @movieID";

                    using (SQLiteCommand cmd = new SQLiteCommand(userRatingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@userID", _currentUser); // Fixed parameter name
                        cmd.Parameters.AddWithValue("@movieID", MovieID);

                        object userRatingResult = cmd.ExecuteScalar();

                        if (userRatingResult != null)
                        {
                            // User has already rated the movie
                            int userRating = Convert.ToInt32(userRatingResult);
                            starBtn.Text = $"Your Rating: {userRating} ★";
                            starBtn.TextColor = Color.White;
                            starBtn.Enabled = false; // Disable the button
                            return;
                        }
                    }

                    // Calculate the average rating for the movie
                    string averageRatingQuery = @"
                        SELECT TotalRatings, NumberOfRatings 
                        FROM Movies 
                        WHERE Id = @movieID";

                    using (SQLiteCommand cmd = new SQLiteCommand(averageRatingQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@movieID", MovieID);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int totalRatings = reader.GetInt32(reader.GetOrdinal("TotalRatings"));
                                int numberOfRatings = reader.GetInt32(reader.GetOrdinal("NumberOfRatings"));

                                if (numberOfRatings > 0)
                                {
                                    double averageRating = (double)totalRatings / numberOfRatings;
                                    starBtn.Text = $"Avg Rating: {averageRating:F1} ★";
                                }
                                else
                                {
                                    starBtn.Text = "⭐Add Stars⭐";
                                }
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
                MessageBox.Show("Error updating star button: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
