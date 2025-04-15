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
    public partial class CreateBookingForm : KryptonForm
    {
        private HashSet<string> toggledButtons = new HashSet<string>();
        private string[] buttonNames = new string[]
        {
            "A1", "A2", "A3", "A4", "A5",
            "B1", "B2", "B3", "B4", "B5",
            "C1", "C2", "C3", "C4", "C5",
            "D1", "D2", "D3", "D4", "D5",
            "E1", "E2", "E3", "E4", "E5",
            "F1", "F2", "F3", "F4", "F5",
            "G1", "G2", "G3", "G4", "G5",
            "H1", "H2", "H3", "H4", "H5",
            "I1", "I2", "I3", "I4", "I5"
        };
        private int movieId;
        private decimal ticketPrice;
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

        public CreateBookingForm(string currentUser)
        {
            InitializeComponent();
            _currentUser = currentUser;
        }
        private void BookMovieForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(915, 500);
            movieTitleTbx.Text = MovieTitle;
            LoadShowtimes();
            InitializeSeatToggleTracking();

        }
        private void showtimeDropdown_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                if (showtimeDropdown.SelectedIndex == -1)
                {
                    EnableSeatButtons(false); // Disable buttons if no showtime is selected
                    submitBtn.Enabled = false; // Disable submit button
                    return; // No selection made
                }

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    string query = @"
                    SELECT TicketPrice 
                    FROM Showtimes 
                    WHERE MovieID = @movieId 
                      AND StartTime = @startTime";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@movieId", movieId);

                        // Extract the StartTime from the selected dropdown item
                        string selectedItem = showtimeDropdown.SelectedItem.ToString();
                        string startTimeString = selectedItem.Split('-')[0].Trim(); // Extract StartTime
                        DateTime startTime = DateTime.Parse(startTimeString);

                        // Check if the showtime is in the past
                        if (startTime <= DateTime.Now)
                        {
                            MessageBox.Show("The selected showtime has already started or is in the past.", "Invalid Showtime", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            EnableSeatButtons(false); // Disable seat buttons
                            submitBtn.Enabled = false; // Disable submit button
                            return;
                        }

                        cmd.Parameters.AddWithValue("@startTime", startTime.ToString("yyyy-MM-dd HH:mm:ss"));

                        object result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            ticketPrice = Convert.ToDecimal(result);
                            ticketPriceTbx.Text = ticketPrice.ToString("C"); // Format as currency
                            EnableSeatButtons(true); // Enable buttons if a valid showtime is selected
                            submitBtn.Enabled = true; // Enable submit button
                        }
                        else
                        {
                            ticketPriceTbx.Text = "N/A"; // No price found
                            ticketPrice = 0;
                            EnableSeatButtons(false); // Disable buttons if no price is found
                            submitBtn.Enabled = false; // Disable submit button
                        }

                        resetButtons();
                        CheckAndDisableBookedSeats();
                    }
                }
            }
            catch (SQLiteException ex)
            {
                MessageBox.Show("Database error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error fetching ticket price: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void submitBtn_Click(object sender, EventArgs e)
        {
            if (showtimeDropdown.SelectedIndex == -1 || toggledButtons.Count == 0)
            {
                MessageBox.Show("Please select a showtime and at least one seat.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedShowtime = showtimeDropdown.SelectedItem?.ToString() ?? "No showtime selected";
            int showtimeID = DatabaseHelper.GetShowtimeID(selectedShowtime, movieId);
            string userID = _currentUser;

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

                    using (SQLiteTransaction transaction = conn.BeginTransaction())
                    {
                        try
                        {
                            foreach (string seat in toggledButtons)
                            {
                                string query = @"
                                INSERT INTO Bookings (UserID, ShowtimeID, SeatNumber)
                                VALUES (@userID, @showtimeID, @seatNumber);";

                                using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                                {
                                    cmd.Parameters.AddWithValue("@userID", userID);
                                    cmd.Parameters.AddWithValue("@showtimeID", showtimeID);
                                    cmd.Parameters.AddWithValue("@seatNumber", seat);
                                    cmd.ExecuteNonQuery();
                                }
                            }

                            transaction.Commit();
                            DisplayBookingSummary(showtimeID, selectedShowtime, toggledButtons, toggledButtons.Count * ticketPrice);
                            CheckAndDisableBookedSeats();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            MessageBox.Show("Error while booking: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Unexpected error: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayBookingSummary(int showtimeID, string selectedShowtime, HashSet<string> selectedSeats, decimal totalPrice)
        {
            string seatList = string.Join(", ", selectedSeats);
            string message = $"Booking Summary:\n\n" +
                             $"User: {_currentUser}\n" +
                             $"Showtime: {selectedShowtime}\n" +
                             $"Showtime ID: {showtimeID}\n" +
                             $"Seats: {seatList}\n" +
                             $"Total Price: {totalPrice:C}";

            MessageBox.Show(message, "Booking Summary", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }


        private void LoadShowtimes()
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    string query = @"
                    SELECT StartTime, EndTime 
                    FROM Showtimes 
                    WHERE MovieID = @movieId";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@movieId", movieId);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            showtimeDropdown.Items.Clear(); // Clear existing items

                            while (reader.Read())
                            {
                                DateTime startTime = DateTime.Parse(reader["StartTime"].ToString());
                                DateTime endTime = DateTime.Parse(reader["EndTime"].ToString());

                                // Format the datetime as "StartTime - EndTime"
                                string displayText = $"{startTime:MMMM dd, yyyy hh:mm tt} - {endTime:hh:mm tt}";
                                showtimeDropdown.Items.Add(displayText);
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
                MessageBox.Show("Error loading showtimes: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeSeatToggleTracking()
        {
            // Attach event handlers to each button
            foreach (var buttonName in buttonNames)
            {
                var button = this.Controls.Find(buttonName, true).FirstOrDefault() as SiticoneNetFrameworkUI.SiticoneToggleButton;
                if (button != null)
                {
                    button.Enabled = false; // Initially disable all buttons

                    button.ToggledOn += (sender, e) =>
                    {
                        var toggleButton = sender as SiticoneNetFrameworkUI.SiticoneToggleButton;
                        if (toggleButton != null)
                        {
                            toggledButtons.Add(toggleButton.Text); // Add to toggled set
                            UpdateSeatAndTotalCount();
                        }
                    };

                    button.ToggledOff += (sender, e) =>
                    {
                        var toggleButton = sender as SiticoneNetFrameworkUI.SiticoneToggleButton;
                        if (toggleButton != null)
                        {
                            toggledButtons.Remove(toggleButton.Text); // Remove from toggled set
                            UpdateSeatAndTotalCount();
                        }
                    };
                }
            }
        }

        private void EnableSeatButtons(bool enable)
        {
            foreach (var buttonName in buttonNames)
            {
                var button = this.Controls.Find(buttonName, true).FirstOrDefault() as SiticoneNetFrameworkUI.SiticoneToggleButton;
                if (button != null)
                {
                    button.Enabled = enable;
                }
            }
        }

        private void UpdateSeatAndTotalCount()
        {
            numSeatsTbx.Text = toggledButtons.Count.ToString(); // Update seat count
            totalTbx.Text = (toggledButtons.Count * ticketPrice).ToString("C"); // Update total price
        }

        private void resetButtons()
        {
            foreach (var buttonName in buttonNames)
            {
                var button = this.Controls.Find(buttonName, true).FirstOrDefault() as SiticoneNetFrameworkUI.SiticoneToggleButton;
                if (button != null)
                {
                    button.IsToggled = false; // Untoggle the button
                }
            }

            // Clear the toggled buttons set and update the seat count and total price
            toggledButtons.Clear();
            UpdateSeatAndTotalCount();
        }
        private void CheckAndDisableBookedSeats()
        {
            if (showtimeDropdown.SelectedIndex == -1)
            {
                MessageBox.Show("Please select a showtime first.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string selectedShowtime = showtimeDropdown.SelectedItem.ToString();
            int showtimeID = DatabaseHelper.GetShowtimeID(selectedShowtime, movieId);

            if (showtimeID == -1)
            {
                MessageBox.Show("Invalid showtime selected.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    string query = @"
                    SELECT SeatNumber 
                    FROM Bookings 
                    WHERE ShowtimeID = @showtimeID";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@showtimeID", showtimeID);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                string bookedSeat = reader["SeatNumber"].ToString();

                                // Find the button corresponding to the booked seat
                                var button = this.Controls.Find(bookedSeat, true).FirstOrDefault() as SiticoneNetFrameworkUI.SiticoneToggleButton;
                                if (button != null)
                                {
                                    button.Enabled = false; // Disable the button
                                    button.DisabledBackColor = Color.DimGray; // Set DisabledBackColor
                                    button.DisabledForeColor = Color.LightGray; // Set DisabledForeColor
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
                MessageBox.Show("Error checking booked seats: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
