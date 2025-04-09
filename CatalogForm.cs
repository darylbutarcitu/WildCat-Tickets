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

                    string query = "SELECT Id, PosterPath FROM Movies";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            int movieId = Convert.ToInt32(reader["Id"]);
                            string posterPath = reader["PosterPath"].ToString();
                            if (!string.IsNullOrEmpty(posterPath) && File.Exists(posterPath))
                            {
                                AddImageToGrid(posterPath, movieId);
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

        private void AddImageToGrid(string imagePath, int movieId)
        {
            // Create a container panel to hold the PictureBox and the delete icon
            Panel containerPanel = new Panel
            {
                Size = new Size(200, 300), // Same size as the PictureBox
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

            // Add the PictureBox to the container panel
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
                                    AddImageToGrid(posterPath, movieId); // Pass both posterPath and movieId
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

                        string query = "SELECT Title, Description, ReleaseDate, Genre, Rating FROM Movies WHERE Id = @movieId";
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@movieId", movieId);

                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    string title = reader["Title"].ToString();
                                    string description = reader["Description"].ToString();
                                    string releaseDate = reader["ReleaseDate"].ToString();
                                    string genre = reader["Genre"].ToString();
                                    string rating = reader["Rating"].ToString();

                                    // Display the movie information (e.g., in a MessageBox or a new form)
                                    MessageBox.Show(
                                        $"Title: {title}\nDescription: {description}\nRelease Date: {releaseDate}\nGenre: {genre}\nRating: {rating}",
                                        "Movie Details",
                                        MessageBoxButtons.OK,
                                        MessageBoxIcon.Information
                                    );
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
