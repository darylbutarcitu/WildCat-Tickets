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
using FastReport.DataVisualization.Charting;

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
            LoadStarRatingsChart();
        }

        private void LoadStarRatingsChart()
        {
            // Create a Chart control
            Chart starRatingsChart = new Chart
            {
                Dock = DockStyle.Fill
            };

            // Add a ChartArea
            ChartArea chartArea = new ChartArea("StarRatingsArea");
            starRatingsChart.ChartAreas.Add(chartArea);

            // Add a Series for Average Star Ratings
            Series averageRatingsSeries = new Series("Average Star Ratings")
            {
                ChartType = SeriesChartType.Column, // Use a column chart
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Double,
                Color = Color.Blue // Set color for distinction
            };
            starRatingsChart.Series.Add(averageRatingsSeries);

            // Add a Series for Total Ratings
            Series totalRatingsSeries = new Series("Total Ratings")
            {
                ChartType = SeriesChartType.Line, // Use a line chart for contrast
                XValueType = ChartValueType.String,
                YValueType = ChartValueType.Int32,
                Color = Color.Red, // Set color for distinction
                BorderWidth = 2, // Make the line more visible
                IsValueShownAsLabel = true, // Show literal data as labels
                LabelForeColor = Color.Black // Set label color for visibility
            };
            starRatingsChart.Series.Add(totalRatingsSeries);

            try
            {
                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    string query = "SELECT Title, TotalRatings, NumberOfRatings FROM Movies";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    using (SQLiteDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string title = reader["Title"].ToString();
                            int totalRatings = Convert.ToInt32(reader["TotalRatings"]);
                            int numberOfRatings = Convert.ToInt32(reader["NumberOfRatings"]);

                            // Calculate the average star rating
                            double averageRating = numberOfRatings > 0 ? (double)totalRatings / numberOfRatings : 0.0;

                            // Add data points to the Average Star Ratings series
                            averageRatingsSeries.Points.AddXY(title, averageRating);

                            // Add data points to the Total Ratings series
                            totalRatingsSeries.Points.AddXY(title, totalRatings);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading star ratings: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            // Add the Chart control to the form
            this.Controls.Add(starRatingsChart);
        }
    }
}
