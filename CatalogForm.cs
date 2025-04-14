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

                    string query = "SELECT Id, PosterPath, TotalRatings, NumberOfRatings FROM Movies";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int movieId = Convert.ToInt32(reader["Id"]);
                            string posterPath = reader["PosterPath"].ToString();
                            int totalRatings = Convert.ToInt32(reader["TotalRatings"]);
                            int numberOfRatings = Convert.ToInt32(reader["NumberOfRatings"]);

                            // Calculate stars (avoid division by zero)
                            double stars = numberOfRatings > 0 ? (double)totalRatings / numberOfRatings : 0.0;

                            if (!string.IsNullOrEmpty(posterPath) && File.Exists(posterPath))
                            {
                                AddImageToGrid(posterPath, movieId, Math.Round(stars, 1)); // Pass stars to AddImageToGrid
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

        private void AddImageToGrid(string imagePath, int movieId, double stars)
        {
            // Create a container panel to hold the PictureBox and the star rating label
            Panel containerPanel = new Panel
            {
                Size = new Size(200, 340), // Adjusted height to accommodate the star rating label
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
                Tag = movieId // Store the movie ID in the Tag property
            };

            pictureBox.Click += MoviePoster_Click; // Attach click event handler

            // Create a Label for the star rating
            Label starsLabel = new Label
            {
                Text = $"⭐ {stars:F1} Stars", // Display stars with 1 decimal place
                TextAlign = ContentAlignment.MiddleCenter,
                Dock = DockStyle.Bottom,
                Font = new Font("Segoe UI", 15, FontStyle.Bold),
                ForeColor = Color.White,
                Height = 30
            };

            // Add the PictureBox and the Label to the container panel
            containerPanel.Controls.Add(starsLabel);
            containerPanel.Controls.Add(pictureBox);

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
                        SELECT Id, PosterPath 
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
                                if (!string.IsNullOrEmpty(posterPath) && File.Exists(posterPath))
                                {
                                    AddImageToGrid(posterPath, movieId, 0.0);
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
