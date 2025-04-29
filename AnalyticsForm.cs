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
using LiveCharts.WinForms;
using LiveCharts.Wpf;
using LiveCharts.Definitions.Charts;

namespace WildCat_Tickets
{
    public partial class AnalyticsForm : TabForm
    {
        public AnalyticsForm()
        {
            InitializeComponent();
        }

        private void AnalyticsForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 675);
            LoadYearDropdownItems();
            LoadAnalyticsData();
        }

        private void LoadAnalyticsData()
        {
            // Clear existing chart controls in the resultsPanel
            resultsPanel.Controls.Clear();
            rankingPanel.Controls.Clear();
            titlePanel.Controls.Clear();

            // Prepare filters
            string selectedGenre = genreDropdown.SelectedItem?.ToString() ?? "Any";
            string selectedReleaseYear = releaseYearDropdown.SelectedItem?.ToString() ?? "Any";
            int maxResults;
            switch (countDropdown.SelectedIndex)
            {
                case 0:
                    maxResults = 3;
                    break;
                case 1:
                    maxResults = 5;
                    break;
                case 2:
                    maxResults = 10;
                    break;
                default:
                    maxResults = 10;
                    break;
            }

            // Column Chart for Movie Titles & Avg. Ratings
            if (analyticsDropdown.SelectedIndex == 0) 
            {
                try
                {
                    var movieTitles = new List<string>();
                    var avgRatings = new List<double>();

                    // Create a new CartesianChart
                    var columnChart = new LiveCharts.WinForms.CartesianChart
                    {
                        Dock = DockStyle.Fill,
                        BackColor = Color.FromArgb(30, 30, 30) // Set a dark background for better aesthetics
                    };

                    // Build the SQL query with filters
                    string query = @"
                    SELECT Title, TotalRatings, NumberOfRatings 
                    FROM Movies 
                    WHERE (@Genre = 'Any' OR Genre = @Genre)
                      AND (@ReleaseYear = 'Any' OR strftime('%Y', ReleaseDate) = @ReleaseYear)
                      AND NumberOfRatings > 0 -- Exclude movies with 0 ratings
                    ORDER BY TotalRatings DESC
                    LIMIT @MaxResults";

                    using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                    {
                        conn.Open();

                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Genre", selectedGenre);
                            cmd.Parameters.AddWithValue("@ReleaseYear", selectedReleaseYear);
                            cmd.Parameters.AddWithValue("@MaxResults", maxResults);

                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                movieTitles = new List<string>();
                                avgRatings = new List<double>();

                                while (reader.Read())
                                {
                                    string title = reader["Title"].ToString();
                                    int totalRatings = Convert.ToInt32(reader["TotalRatings"]);
                                    int numberOfRatings = Convert.ToInt32(reader["NumberOfRatings"]);

                                    // Calculate average rating
                                    double avgRating = numberOfRatings > 0 ? (double)totalRatings / numberOfRatings : 0.0;

                                    movieTitles.Add(title);
                                    avgRatings.Add(avgRating);
                                }

                                if (movieTitles.Count == 0)
                                {
                                    // Display "Not enough data" if no results
                                    var noDataLabel = new Label
                                    {
                                        Text = "Not enough data",
                                        ForeColor = Color.White,
                                        Font = new Font("Arial", 16, FontStyle.Bold),
                                        Dock = DockStyle.Fill,
                                        TextAlign = ContentAlignment.MiddleCenter
                                    };
                                    resultsPanel.Controls.Add(noDataLabel);
                                    return;
                                }

                                // Populate the chart
                                columnChart.AxisX.Add(new LiveCharts.Wpf.Axis
                                {
                                    Title = "Movies",
                                    Labels = movieTitles,
                                    Foreground = System.Windows.Media.Brushes.White, // Keep labels' color as white
                                    FontSize = 14, // Increase font size for better readability
                                    Separator = new LiveCharts.Wpf.Separator
                                    {
                                        Step = 1,
                                        IsEnabled = false
                                    }
                                });

                                columnChart.AxisY.Add(new LiveCharts.Wpf.Axis
                                {
                                    Title = "Average Star Rating",
                                    LabelFormatter = value => value.ToString("N1"),
                                    Foreground = System.Windows.Media.Brushes.White, // Keep labels' color as white
                                    FontSize = 14,
                                    MaxValue = 5,
                                    MinValue = 0,
                                    Separator = new LiveCharts.Wpf.Separator
                                    {
                                        Step = 1,
                                        IsEnabled = true
                                    },
                                    ShowLabels = true
                                });

                                columnChart.AxisX[0].Title = "Movies";
                                columnChart.AxisX[0].FontSize = 15;
                                columnChart.AxisX[0].Margin = new System.Windows.Thickness(0, 0, 0, 20);
                                columnChart.AxisX[0].Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 219, 12));

                                columnChart.AxisY[0].Title = "Average Star Rating";
                                columnChart.AxisY[0].FontSize = 15;
                                columnChart.AxisY[0].Margin = new System.Windows.Thickness(0, 0, 0, 20);
                                columnChart.AxisY[0].Foreground = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Color.FromRgb(255, 219, 12));

                                columnChart.Series.Add(new LiveCharts.Wpf.ColumnSeries
                                {
                                    Title = "Avg. Ratings",
                                    Values = new LiveCharts.ChartValues<double>(avgRatings),
                                    Fill = System.Windows.Media.Brushes.SkyBlue
                                });

                                columnChart.Margin = new Padding(10, 20, 10, 10);
                            }
                        }
                    }

                    var chartTitleLabel = new Label
                    {
                        Text = "Column Chart of Top Movies by Average Star Ratings",
                        ForeColor = Color.White,
                        Font = new Font("Arial", 18, FontStyle.Italic),
                        Dock = DockStyle.Top,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Height = 30 
                    };

                    titlePanel.Controls.Add(chartTitleLabel);
                    resultsPanel.Controls.Add(columnChart);
                    DisplayMoviesInRankingPanel(movieTitles, avgRatings, true);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading analytics data: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else if (analyticsDropdown.SelectedIndex == 1) // Pie chart for Movie Titles & Total Bookings
            {
                try
                {
                    // Get total bookings by movie
                    var totalBookingsByMovie = DatabaseHelper.GetTotalBookingsByMovie(selectedGenre, selectedReleaseYear, maxResults);

                    totalBookingsByMovie = totalBookingsByMovie.Where(movie => movie.Value > 0).ToDictionary(movie => movie.Key, movie => movie.Value);

                    if (totalBookingsByMovie.Count == 0)
                    {
                        resultsPanel.Controls.Add(new Label
                        {
                            Text = "Not enough data",
                            ForeColor = Color.White,
                            Font = new Font("Arial", 16, FontStyle.Bold),
                            Dock = DockStyle.Fill,
                            TextAlign = ContentAlignment.MiddleCenter
                        });
                        return;
                    }

                    var chartTitleLabel = new Label
                    {
                        Text = "Pie Chart of Top Selling Movies",
                        ForeColor = Color.White,
                        Font = new Font("Arial", 18, FontStyle.Italic),
                        Dock = DockStyle.Top,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Height = 30
                    };
                    titlePanel.Controls.Add(chartTitleLabel);

                    // Function to generate a color from an HSL hue value
                    System.Windows.Media.Color HSLColor(double hue)
                    {
                        var h = hue % 360;
                        var s = 0.6;
                        var l = 0.5;

                        var c = (1 - Math.Abs(2 * l - 1)) * s;
                        var x = c * (1 - Math.Abs(h / 60 % 2 - 1));
                        var m = l - c / 2;

                        double r = 0, g = 0, b = 0;

                        if (h < 60) { r = c; g = x; }
                        else if (h < 120) { r = x; g = c; }
                        else if (h < 180) { g = c; b = x; }
                        else if (h < 240) { g = x; b = c; }
                        else if (h < 300) { r = x; b = c; }
                        else { r = c; b = x; }

                        return System.Windows.Media.Color.FromRgb(
                            (byte)((r + m) * 255),
                            (byte)((g + m) * 255),
                            (byte)((b + m) * 255)
                        );
                    }

                    // Create a new PieChart
                    var pieChart = new LiveCharts.WinForms.PieChart
                    {
                        Dock = DockStyle.Fill,
                        Margin = new Padding(10),
                    };

                    // Generate dynamic colors for each movie
                    int index = 0;
                    int count = totalBookingsByMovie.Count;
                    foreach (var movie in totalBookingsByMovie)
                    {
                        var color = HSLColor(index * 360.0 / count);
                        pieChart.Series.Add(new LiveCharts.Wpf.PieSeries
                        {
                            Title = movie.Key,
                            Values = new LiveCharts.ChartValues<double> { movie.Value },
                            DataLabels = true,
                            LabelPoint = chartPoint => $"{chartPoint.Y} ({chartPoint.Participation:P})",
                            Fill = new System.Windows.Media.SolidColorBrush(color)
                        });
                        index++;
                    }

                    resultsPanel.Controls.Add(pieChart);

                    DisplayMoviesInRankingPanel(totalBookingsByMovie.Keys.ToList(), totalBookingsByMovie.Values.Select(v => (double)v).ToList(), false);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading analytics data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DisplayMoviesInRankingPanel(List<string> movieTitles, List<double> values, bool showStars)
        {
            try
            {
                // Clear existing controls in the Siticone Flat Panel
                rankingPanel.Controls.Clear();

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    int rank = 1; // Initialize ranking counter

                    for (int i = 0; i < movieTitles.Count; i++)
                    {
                        string title = movieTitles[i];
                        double value = values[i]; // Use the value (either avgRating or number of bookings)

                        string query = "SELECT PosterPath FROM Movies WHERE Title = @Title LIMIT 1";
                        string posterPath = null;

                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        {
                            cmd.Parameters.AddWithValue("@Title", title);

                            using (SQLiteDataReader reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                {
                                    posterPath = reader["PosterPath"]?.ToString();
                                }
                            }
                        }

                        // Use TableLayoutPanel for proper layout management
                        var moviePanel = new TableLayoutPanel
                        {
                            ColumnCount = 2, // One column for the poster, one for the details
                            RowCount = 1, // A single row
                            Height = 120,
                            Dock = DockStyle.Top,
                            Padding = new Padding(10),
                            Margin = new Padding(0, 0, 0, 10),
                            AutoSize = true
                        };
                        moviePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 90)); // Fixed width for the poster
                        moviePanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); // Remaining space for details

                        // Add movie poster
                        if (!string.IsNullOrEmpty(posterPath) && System.IO.File.Exists(posterPath))
                        {
                            var posterPictureBox = new PictureBox
                            {
                                Image = Image.FromFile(posterPath),
                                SizeMode = PictureBoxSizeMode.Zoom,
                                Width = 80, // Fixed width
                                Height = 100, // Fixed height
                                Dock = DockStyle.Fill,
                                Margin = new Padding(0, 0, 10, 0)
                            };
                            moviePanel.Controls.Add(posterPictureBox, 0, 0); // Add to the first column
                        }

                        // Create a container for title and details
                        var detailsPanel = new FlowLayoutPanel
                        {
                            Dock = DockStyle.Fill,
                            FlowDirection = FlowDirection.TopDown,
                            Padding = new Padding(10, 0, 0, 0), // Add padding to prevent overlap with the poster
                            WrapContents = false,
                            AutoSize = true
                        };

                        // Add movie title
                        var titleLabel = new Label
                        {
                            Text = $"{rank}. {title}",
                            ForeColor = Color.White,
                            Font = new Font("Arial", 10, FontStyle.Bold),
                            AutoSize = true, // Ensure proper layout
                            Margin = new Padding(0, 0, 0, 5), // Add margin below the title
                            TextAlign = ContentAlignment.MiddleLeft
                        };
                        detailsPanel.Controls.Add(titleLabel);

                        if (showStars)
                        {
                            // Add star rating with value below the title
                            var starsPanel = new FlowLayoutPanel
                            {
                                AutoSize = true, // Ensure it adjusts to the content
                                FlowDirection = FlowDirection.LeftToRight,
                                WrapContents = false,
                                Margin = new Padding(0, 0, 0, 5), // Add margin below the stars
                            };

                            // Determine star images based on avgRating
                            double avgRating = value;
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

                                    }
                                    else
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
                                Text = $"({value:0.0})",
                                ForeColor = Color.White,
                                Font = new Font("Arial", 10, FontStyle.Regular),
                                TextAlign = ContentAlignment.MiddleLeft,
                                Margin = new Padding(5, 5, 0, 0)
                            };
                            starsPanel.Controls.Add(ratingLabel);

                            detailsPanel.Controls.Add(starsPanel);
                        }
                        else
                        {
                            // Add number of bookings
                            var bookingsLabel = new Label
                            {
                                Text = $"Tickets Sold: {value}",
                                ForeColor = Color.White,
                                Font = new Font("Arial", 10, FontStyle.Regular),
                                AutoSize = true,
                                Margin = new Padding(0, 0, 0, 5),
                                TextAlign = ContentAlignment.MiddleLeft
                            };
                            detailsPanel.Controls.Add(bookingsLabel);
                        }

                        // Add detailsPanel to the second column of the TableLayoutPanel
                        moviePanel.Controls.Add(detailsPanel, 1, 0);

                        // Add the movie panel to the rankingPanel
                        rankingPanel.Controls.Add(moviePanel);
                        rankingPanel.Controls.SetChildIndex(moviePanel, 0); // Ensure the newest panel is displayed at the top

                        rank++; // Increment ranking counter
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error displaying movies in ranking panel: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadYearDropdownItems()
        {
            try
            {
                // Clear existing items and add the default "Any" option
                releaseYearDropdown.Items.Clear();
                releaseYearDropdown.Items.Add("Any");

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // Query to extract unique release years from the ReleaseDate column, sorted from oldest to newest
                    string query = "SELECT DISTINCT strftime('%Y', ReleaseDate) AS ReleaseYear FROM Movies WHERE ReleaseDate IS NOT NULL ORDER BY ReleaseYear ASC";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Add each release year to the dropdown
                            releaseYearDropdown.Items.Add(reader["ReleaseYear"].ToString());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading release years: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
