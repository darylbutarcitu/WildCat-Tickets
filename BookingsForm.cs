using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;
using System.IO;

namespace WildCat_Tickets
{
    public partial class BookingsForm : TabForm
    {
        private string userID;

        public BookingsForm(string currentUser)
        {
            InitializeComponent();
            userID = currentUser;
        }

        private void BookingsForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 675);
            LoadBookingsFromDatabase();
        }
        private void searchTbx_TextChanged(object sender, EventArgs e)
        {
            string searchText = searchTbx.Text.Trim().ToLower();

            foreach (Control card in bookingsFlowLayoutPanel.Controls)
            {
                if (card is Panel cardPanel)
                {
                    bool isMatch = false;

                    foreach (Control child in cardPanel.Controls)
                    {
                        if (child is Label label)
                        {
                            // Check if the label's text contains the search keyword
                            if (label.Text.ToLower().Contains(searchText))
                            {
                                isMatch = true;
                                break;
                            }
                        }
                    }

                    // Show or hide the card based on the match
                    cardPanel.Visible = isMatch;
                }
            }
        }

        private void LoadBookingsFromDatabase()
        {
            bookingsFlowLayoutPanel.Controls.Clear(); // Clear previous bookings

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    string query = @"
            SELECT 
                s.ShowtimeID, 
                m.Title, 
                m.PosterPath, 
                s.StartTime, 
                s.EndTime, 
                GROUP_CONCAT(b.SeatNumber, ', ') AS SeatNames, 
                COUNT(b.SeatNumber) AS SeatCount,
                s.TicketPrice
            FROM 
                Bookings b
            INNER JOIN 
                Showtimes s ON b.ShowtimeID = s.ShowtimeID
            INNER JOIN 
                Movies m ON s.MovieID = m.Id
            WHERE (@IsAdmin = 1 OR b.UserID = @UserID)
            GROUP BY 
                s.ShowtimeID, 
                m.Title, 
                m.PosterPath, 
                s.StartTime, 
                s.EndTime, 
                s.TicketPrice";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        // Pass parameters for admin or specific user
                        cmd.Parameters.AddWithValue("@IsAdmin", userID.ToLower() == "admin" ? 1 : 0);
                        cmd.Parameters.AddWithValue("@UserID", userID);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int showtimeId = Convert.ToInt32(reader["ShowtimeID"]);
                                string movieTitle = reader["Title"].ToString();
                                string posterPath = reader["PosterPath"].ToString();
                                DateTime startTime = Convert.ToDateTime(reader["StartTime"]);
                                DateTime endTime = Convert.ToDateTime(reader["EndTime"]);
                                string seatNames = reader["SeatNames"].ToString();
                                int seatCount = Convert.ToInt32(reader["SeatCount"]);
                                decimal ticketPrice = Convert.ToDecimal(reader["TicketPrice"]);
                                decimal totalPrice = seatCount * ticketPrice;

                                AddShowtimeCard(posterPath, showtimeId, movieTitle, startTime, endTime, seatNames, seatCount, ticketPrice, totalPrice);
                            }
                        }
                    }

                    conn.Close();
                }

                AdjustBookingLayout(); // Adjust layout after loading bookings
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading bookings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AddShowtimeCard(string imagePath, int showtimeId, string movieTitle, DateTime startTime, DateTime endTime, string seatNames, int seatCount, decimal ticketPrice, decimal totalPrice)
        {
            // Create a container panel for the card
            Panel cardPanel = new Panel
            {
                Size = new Size(250, 500), // Adjusted size for the card to accommodate the new label
                Margin = new Padding(10),
                BackColor = Color.FromArgb(86, 0, 0),
                BorderStyle = BorderStyle.None,
                Tag = showtimeId // Store the showtime ID in the Tag property
            };

            // Create the PictureBox for the movie poster
            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(230, 200), // Fixed size for the poster
                Location = new Point(10, 10),
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand,
                Image = File.Exists(imagePath) ? Image.FromFile(imagePath) : Image.FromFile("placeholder.png"), // Fallback image
                Tag = showtimeId // Store the showtime ID in the Tag property
            };

            // Create a Label for the movie title
            Label titleLabel = new Label
            {
                Text = movieTitle,
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(10, 220),
                Size = new Size(230, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create a Label for the showtime details
            Label showtimeLabel = new Label
            {
                Text = $"Showtime: {startTime:MMMM dd, yyyy hh:mm tt} - {endTime:hh:mm tt}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(10, 270),
                Size = new Size(230, 0), // Set width, height will adjust automatically
                MaximumSize = new Size(230, 0), // Limit the width to 230px
                AutoSize = true, // Allow height to adjust based on content
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create a Label for the seat names
            Label seatNamesLabel = new Label
            {
                Text = $"Seats: {seatNames}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(10, 320),
                Size = new Size(230, 0), // Set width, height will adjust automatically
                MaximumSize = new Size(230, 0), // Limit the width to 230px
                AutoSize = true, // Allow height to adjust based on content
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create a Label for the total number of seats
            Label seatCountLabel = new Label
            {
                Text = $"Total Seats: {seatCount}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(10, 370),
                Size = new Size(230, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create a Label for the individual ticket price
            Label ticketPriceLabel = new Label
            {
                Text = $"Ticket Price: {ticketPrice:C}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(10, 400),
                Size = new Size(230, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Create a Label for the total price
            Label priceLabel = new Label
            {
                Text = $"Total Price: {totalPrice:C}",
                Font = new Font("Segoe UI", 10),
                ForeColor = Color.White,
                Location = new Point(10, 430),
                Size = new Size(230, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Determine the status of the showtime
            bool isActive = endTime > DateTime.Now;
            string statusText = isActive ? "Active" : "Inactive";
            Color statusColor = isActive ? Color.Yellow : Color.Pink;

            // Create a Label for the status
            Label statusLabel = new Label
            {
                Text = $"Status: {statusText}",
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                ForeColor = statusColor,
                Location = new Point(10, 460),
                Size = new Size(230, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Add the PictureBox and Labels to the card panel
            cardPanel.Controls.Add(pictureBox);
            cardPanel.Controls.Add(titleLabel);
            cardPanel.Controls.Add(showtimeLabel);
            cardPanel.Controls.Add(seatNamesLabel);
            cardPanel.Controls.Add(seatCountLabel);
            cardPanel.Controls.Add(ticketPriceLabel);
            cardPanel.Controls.Add(priceLabel);
            cardPanel.Controls.Add(statusLabel);

            // Add a context menu strip for non-admin users
            if (userID.ToLower() != "admin")
            {
                ContextMenuStrip contextMenu = new ContextMenuStrip();
                ToolStripMenuItem deleteMenuItem = new ToolStripMenuItem("Delete");

                // Disable the delete option if the showtime is inactive
                if (!isActive)
                {
                    deleteMenuItem.Enabled = false;
                }

                deleteMenuItem.Click += (s, e) =>
                {
                    // Confirm deletion
                    DialogResult result = MessageBox.Show("Are you sure you want to delete all bookings for this showtime?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (result == DialogResult.Yes)
                    {
                        DeleteBookingsForShowtime(showtimeId);
                    }
                };

                contextMenu.Items.Add(deleteMenuItem);
                cardPanel.ContextMenuStrip = contextMenu;
            }

            // Add the card panel to the FlowLayoutPanel
            bookingsFlowLayoutPanel.Controls.Add(cardPanel);
        }

        private void DeleteBookingsForShowtime(int showtimeId)
        {
            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    string query = "DELETE FROM Bookings WHERE ShowtimeID = @ShowtimeID AND UserID = @UserID";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@ShowtimeID", showtimeId);
                        cmd.Parameters.AddWithValue("@UserID", userID);
                        cmd.ExecuteNonQuery();
                    }

                    MessageBox.Show("Bookings deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    // Reload the bookings to reflect the changes
                    LoadBookingsFromDatabase();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error deleting bookings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void AdjustBookingLayout()
        {
            int panelWidth = bookingsFlowLayoutPanel.ClientSize.Width;
            int bookingWidth = 260; // 250px booking + 10px margin
            int columns = panelWidth / bookingWidth;

            int totalBookingWidth = columns * bookingWidth;
            int padding = (panelWidth - totalBookingWidth) / 2;

            bookingsFlowLayoutPanel.SuspendLayout();
            bookingsFlowLayoutPanel.Padding = new Padding(padding, 0, padding, 0); // Center the bookings and ensure padding is balanced
            bookingsFlowLayoutPanel.ResumeLayout();
        }

    }
}
