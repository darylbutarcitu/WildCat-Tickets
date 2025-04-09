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

namespace WildCat_Tickets
{
    public partial class AddMovieForm : TabForm
    {
        private string imagePath = string.Empty;
        public AddMovieForm()
        {
            InitializeComponent();
        }

        private void AddMovieForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(772, 470);
            ratingComboBox.DropDownWidth = ratingComboBox.Width;
            genreComboBox.DropDownWidth = genreComboBox.Width;
        }

        private void uploadMoviePosterBtn_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        imagePath = openFileDialog.FileName;
                        moviePosterBox.Image = Image.FromFile(imagePath); // Assuming `moviePictureBox` is the PictureBox control
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error uploading image: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to cancel?", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void addMovieBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string movieTitle = movieTitleTbx.Text.Trim();
                string movieDuration = durationUpDown.Text.Trim();
                string genre = genreComboBox.Text.Trim();
                string rating = ratingComboBox.Text.Trim();
                string releaseDateText = releaseDatePicker.Text.Trim();
                string description = movieDescriptionTbx.Text.Trim();

                // Validate inputs
                if (string.IsNullOrEmpty(movieTitle) || string.IsNullOrEmpty(genre) || string.IsNullOrEmpty(releaseDateText) ||
                    string.IsNullOrEmpty(movieDuration) || string.IsNullOrEmpty(rating) || string.IsNullOrEmpty(description))
                {
                    MessageBox.Show("All fields are required. Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!DateTime.TryParse(releaseDateText, out DateTime releaseDate))
                {
                    MessageBox.Show("Invalid release date format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (string.IsNullOrEmpty(imagePath))
                {
                    MessageBox.Show("Please upload a movie poster.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Determine the status based on the release date
                string status = releaseDate > DateTime.Now ? "Coming Soon" : "Now Showing";

                // Insert movie into the database
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // Ensure the Movies table exists
                    string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Movies (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Title TEXT NOT NULL,
                        Duration TEXT NOT NULL,
                        Genre TEXT NOT NULL,
                        Rating TEXT NOT NULL,
                        ReleaseDate TEXT NOT NULL,
                        Description TEXT NOT NULL,
                        PosterPath TEXT NOT NULL,
                        Status TEXT NOT NULL
                    )";
                    using (SQLiteCommand createTableCmd = new SQLiteCommand(createTableQuery, conn))
                    {
                        createTableCmd.ExecuteNonQuery();
                    }

                    // Insert the movie
                    string insertQuery = @"
                    INSERT INTO Movies (Title, Duration, Genre, Rating, ReleaseDate, Description, PosterPath, Status)
                    VALUES (@Title, @Duration, @Genre, @Rating, @ReleaseDate, @Description, @PosterPath, @Status)";
                    using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@Title", movieTitle);
                        cmd.Parameters.AddWithValue("@Duration", movieDuration);
                        cmd.Parameters.AddWithValue("@Genre", genre);
                        cmd.Parameters.AddWithValue("@Rating", rating);
                        cmd.Parameters.AddWithValue("@ReleaseDate", releaseDate);
                        cmd.Parameters.AddWithValue("@Description", description);
                        cmd.Parameters.AddWithValue("@PosterPath", imagePath);
                        cmd.Parameters.AddWithValue("@Status", status);

                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }

                MessageBox.Show("Movie added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error adding movie: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            this.PerformLayout(); // Force layout recalculation
        }
    }
}
