using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SQLite;

namespace WildCat_Tickets
{
    public partial class CatalogForm : TabForm
    {
        private int previousFormWidth;
        private string currentUser;

        public CatalogForm(string idNumber)
        {
            InitializeComponent();
            this.Resize += MoviesForm_Resize;
            previousFormWidth = this.ClientSize.Width;
            currentUser = idNumber; // Store the current user ID
        }


        private void MoviesForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 675);
            LoadMoviesFromDatabase();
        }

        private void LoadMoviesFromDatabase()
        {
            moviesFlowLayoutPanel.Controls.Clear(); // Clear previous images

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // Load all movies from the Movies table and sort them alphabetically by Title
                    string query = "SELECT Id, PosterPath, TotalRatings, NumberOfRatings, Title, ReleaseDate FROM Movies ORDER BY Title ASC";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int movieId = Convert.ToInt32(reader["Id"]);
                            string posterPath = reader["PosterPath"].ToString();
                            int totalRatings = Convert.ToInt32(reader["TotalRatings"]);
                            int numberOfRatings = Convert.ToInt32(reader["NumberOfRatings"]);
                            string title = reader["Title"].ToString();

                            // Extract release year from ReleaseDate
                            DateTime releaseDate = Convert.ToDateTime(reader["ReleaseDate"]);
                            string releaseYear = releaseDate.Year.ToString();

                            // Check if the movie has at least one upcoming showtime for non-admin users
                            if (currentUser == "admin" || HasUpcomingShowtime(conn, movieId))
                            {
                                // Calculate stars (avoid division by zero)
                                double stars = numberOfRatings > 0 ? (double)totalRatings / numberOfRatings : 0.0;

                                if (!string.IsNullOrEmpty(posterPath) && File.Exists(posterPath))
                                {
                                    AddImageToGrid(posterPath, movieId, Math.Round(stars, 1), title, releaseYear);
                                }
                            }
                        }
                    }

                    conn.Close();
                }

                AdjustImageLayout(); // Adjust layout after loading images
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading movies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private bool HasUpcomingShowtime(SQLiteConnection conn, int movieId)
        {
            // Query to check for at least one upcoming showtime
            string query = "SELECT 1 FROM Showtimes WHERE MovieID = @movieId AND StartTime > @currentDateTime LIMIT 1";

            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Parameters.AddWithValue("@movieId", movieId);
                cmd.Parameters.AddWithValue("@currentDateTime", DateTime.Now);

                object result = cmd.ExecuteScalar();
                return result != null; // Returns true if at least one upcoming showtime exists
            }
        }


        private void AddImageToGrid(string imagePath, int movieId, double stars, string title, string releaseYear)
        {
            // Create a container panel to hold all elements
            Panel containerPanel = new Panel
            {
                Size = new Size(200, 400), // Adjusted height to fit all elements
                Margin = new Padding(5),
                Tag = movieId // Store the movie ID in the Tag property
            };

            // Create the PictureBox for the movie poster
            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(200, 300), // Fixed size images
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand,
                Image = Image.FromFile(imagePath),
                Dock = DockStyle.Top,
                Tag = movieId // Store the movie ID in the Tag property
            };

            pictureBox.Click += MoviePoster_Click; // Attach click event handler

            // Create a Label for the title and release year with wrapping enabled
            Label titleLabel = new Label
            {
                Text = $"{title} ({releaseYear})",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Bold),
                AutoSize = true, // Enable AutoSize to automatically adjust to the text
                MaximumSize = new Size(200, 0), // Restrict the width to the container width
                TextAlign = ContentAlignment.TopCenter,
                Dock = DockStyle.Top, // Position it below the PictureBox and above the starsPanel
                Padding = new Padding(5), // Add padding for better readability
            };

            // Create a FlowLayoutPanel for the star rating
            var starsPanel = new FlowLayoutPanel
            {
                AutoSize = true, // Ensure it adjusts to the content
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                Dock = DockStyle.Top, // Align it directly below the title
                Margin = new Padding(0, 5, 0, 0) // Add small margin above the stars
            };

            // Add star rating images based on the stars value
            double avgRating = stars;
            for (int j = 0; j < 5; j++)
            {
                PictureBox starPictureBox = new PictureBox
                {
                    SizeMode = PictureBoxSizeMode.Zoom,
                    Width = 20,
                    Height = 20
                };

                if (avgRating >= 1.0)
                {
                    starPictureBox.Image = Image.FromFile(@"D:\OOP Project\repos\WildCat-Tickets\WildCat-Tickets\bin\assets\full_star.png");
                    avgRating -= 1.0;
                }
                else if (avgRating >= 0.5)
                {
                    if (avgRating == 0.5)
                    {
                        starPictureBox.Image = Image.FromFile(@"D:\OOP Project\repos\WildCat-Tickets\WildCat-Tickets\bin\assets\half_star.png");

                    } else
                    {
                        starPictureBox.Image = Image.FromFile(@"D:\OOP Project\repos\WildCat-Tickets\WildCat-Tickets\bin\assets\more_star.png");
                    }

                    avgRating = 0.0;
                }
                else if (avgRating > 0.0 && avgRating < 0.5)
                {
                    starPictureBox.Image = Image.FromFile(@"D:\OOP Project\repos\WildCat-Tickets\WildCat-Tickets\bin\assets\less_star.png");
                    avgRating = 0.0;
                }
                else
                {
                    starPictureBox.Image = Image.FromFile(@"D:\OOP Project\repos\WildCat-Tickets\WildCat-Tickets\bin\assets\no_star.png");
                }

                starsPanel.Controls.Add(starPictureBox);
            }

            // Add the average rating value next to the stars
            var ratingLabel = new Label
            {
                Text = $"({stars:0.0})",
                ForeColor = Color.White,
                Font = new Font("Arial", 10, FontStyle.Regular),
                TextAlign = ContentAlignment.MiddleLeft,
                Margin = new Padding(5, 5, 0, 0)
            };
            starsPanel.Controls.Add(ratingLabel);

            // Add elements to the container panel in the correct order
            containerPanel.Controls.Add(starsPanel);  // Add the starsPanel first (at the bottom of the container)
            containerPanel.Controls.Add(titleLabel); // Add the titleLabel above the starsPanel
            containerPanel.Controls.Add(pictureBox); // Add the pictureBox at the top of the container

            // Add the container panel to the FlowLayoutPanel
            moviesFlowLayoutPanel.Controls.Add(containerPanel);
        }
        private void moviesFlowLayoutPanel_Resize(object sender, EventArgs e)
        {
            AdjustImageLayout();
        }

        private void AdjustImageLayout()
        {
            int panelWidth = moviesFlowLayoutPanel.ClientSize.Width;
            int imageWidth = 210; // 200px image + 10px margin
            int columns = panelWidth / imageWidth;

            int totalImageWidth = columns * imageWidth;
            int padding = (panelWidth - totalImageWidth) / 2;

            moviesFlowLayoutPanel.SuspendLayout();
            moviesFlowLayoutPanel.Padding = new Padding(padding, 0, padding, 0); // Center the images and ensure padding is balanced
            moviesFlowLayoutPanel.ResumeLayout();
        }

        private void searchBtn_Click(object sender, EventArgs e)
        {
            string searchKeyword = searchTbx.Text.Trim();

            if (string.IsNullOrEmpty(searchKeyword))
            {
                // If no keyword is entered, load all movies
                LoadMoviesFromDatabase();
                return;
            }

            moviesFlowLayoutPanel.Controls.Clear(); // Clear previous images

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // Use a LIKE query for partial matching across multiple fields
                    string query = @"
                SELECT Id, PosterPath, Title, ReleaseDate, TotalRatings, NumberOfRatings 
                FROM Movies 
                WHERE Title LIKE @keyword 
                   OR Description LIKE @keyword 
                   OR Genre LIKE @keyword 
                   OR Rating LIKE @keyword 
                   OR ReleaseDate LIKE @keyword";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@keyword", "%" + searchKeyword + "%");

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                int movieId = Convert.ToInt32(reader["Id"]);
                                string posterPath = reader["PosterPath"].ToString();
                                string title = reader["Title"].ToString();

                                // Extract release year from ReleaseDate
                                DateTime releaseDate = Convert.ToDateTime(reader["ReleaseDate"]);
                                string releaseYear = releaseDate.Year.ToString();

                                int totalRatings = Convert.ToInt32(reader["TotalRatings"]);
                                int numberOfRatings = Convert.ToInt32(reader["NumberOfRatings"]);

                                // Calculate stars (avoid division by zero)
                                double stars = numberOfRatings > 0 ? (double)totalRatings / numberOfRatings : 0.0;

                                if (!string.IsNullOrEmpty(posterPath) && File.Exists(posterPath))
                                {
                                    AddImageToGrid(posterPath, movieId, Math.Round(stars, 1), title, releaseYear);
                                }
                            }
                        }
                    }

                    conn.Close();
                }

                AdjustImageLayout(); // Adjust layout after loading images
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error searching movies: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void MoviesForm_Resize(object sender, EventArgs e)
        {
            int currentFormWidth = this.ClientSize.Width;

            if (addMovieBtn.Visible == false)
            {
                searchTbx.Width = currentFormWidth - searchBtn.Width - 20;
            }
            else
            {
                searchTbx.Width = currentFormWidth - searchBtn.Width - addMovieBtn.Width - 40;
            }
        }

        private void addMovieBtn_Click(object sender, EventArgs e)
        {
            // Disable all open forms except AddMovieForm
            foreach (Form openForm in Application.OpenForms)
            {
                if (openForm != this) // Keep the current form enabled
                {
                    openForm.Enabled = false;
                }
            }

            AddMovieForm addMovieForm = new AddMovieForm();
            addMovieForm.FormClosed += (s, args) =>
            {
                // Re-enable all forms when AddMovieForm is closed
                foreach (Form openForm in Application.OpenForms)
                {
                    openForm.Enabled = true;
                }

                // Reload movies from the database
                LoadMoviesFromDatabase();
            };

            addMovieForm.Show();
        }
        private void MoviePoster_Click(object sender, EventArgs e)
        {
            if (sender is PictureBox pictureBox && pictureBox.Tag is int movieId)
            {
                try
                {
                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                    {
                        conn.Open();

                        string query = @"
                            SELECT Title, Duration, Genre, Description, TotalRatings, NumberOfRatings, Rating, ReleaseDate, PosterPath 
                            FROM Movies 
                            WHERE Id = @movieId";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@movieId", movieId);

                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    // Retrieve movie details
                                    string title = reader["Title"].ToString();
                                    string duration = reader["Duration"].ToString();
                                    string genre = reader["Genre"].ToString();
                                    string description = reader["Description"].ToString();
                                    int totalRatings = Convert.ToInt32(reader["TotalRatings"]);
                                    int numberOfRatings = Convert.ToInt32(reader["NumberOfRatings"]);
                                    string rating = reader["Rating"].ToString();
                                    string releaseDate = reader["ReleaseDate"].ToString();
                                    string posterPath = reader["PosterPath"].ToString();

                                    // Open the MovieDetails form and pass the movie details
                                    MovieDetailsForm movieDetailsForm = new MovieDetailsForm(currentUser)
                                    {
                                        MovieID = movieId,
                                        MovieTitle = title,
                                        MovieDuration = duration,
                                        MovieGenre = genre,
                                        MovieDescription = description,
                                        MovieTotalRatings = totalRatings,
                                        MovieNumberOfRatings = numberOfRatings,
                                        MovieRating = rating,
                                        MovieReleaseDate = releaseDate,
                                        MoviePosterPath = posterPath
                                    };

                                    movieDetailsForm.ShowDialog(); // Open the form as a modal dialog
                                }
                            }
                        }

                        conn.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error fetching movie details: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void searchTbx_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                searchBtn.PerformClick();
            }
        }
    }
}
