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
    public partial class MovieDetails : KryptonForm
    {
        private int totalRatings, numberOfRatings;
        public string MovieTitle
        {
            set { movieTitleTbx.Text = value; }
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

        public MovieDetails(string userRole)
        {
            InitializeComponent();

            // Adjust button visibility based on the user's role
            if (userRole == "admin")
            {
                showtimeBtn.Visible = true;
                bookBtn.Visible = false;
                viewShowtimesBtn.Visible = true;
            }
            else
            {
                showtimeBtn.Visible = false;
                bookBtn.Visible = true;
                viewShowtimesBtn.Visible = false;
            }
        }

        private void MovieDetails_Load(object sender, EventArgs e)
        {
            starsTbx.Text = getStars().ToString();
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
                string startTime = startTimePicker.Value.ToString("hh:mm tt"); // Updated to include AM/PM
                string endTime = endTimeTbx.Text;
                string ticketPriceText = ticketPriceTbx.Text.Trim();
                decimal ticketPrice;

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
                            if (IsShowtimeOverlapping(conn, transaction, venueId, $"{date} {startTime}", $"{date} {endTime}"))
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
                                insertCmd.Parameters.AddWithValue("@movieId", 1); // Replace with the actual MovieID
                                insertCmd.Parameters.AddWithValue("@venueId", venueId);
                                insertCmd.Parameters.AddWithValue("@startTime", $"{date} {startTime}");
                                insertCmd.Parameters.AddWithValue("@endTime", $"{date} {endTime}");
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

        private void bookBtn_Click(object sender, EventArgs e)
        {

        }
    }
}
