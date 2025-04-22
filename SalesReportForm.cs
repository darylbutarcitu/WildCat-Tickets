using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiveCharts;
using LiveCharts.Wpf;


namespace WildCat_Tickets
{
    public partial class SalesReportForm : TabForm
    {
        public SalesReportForm()
        {
            InitializeComponent();
        }

        private void SalesReportForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 675);
        }

        private void salesReportDropown_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = salesReportDropown.SelectedIndex;
            string query = "";
            string reportTitle = "";

            switch (selectedIndex)
            {
                case 0: // Daily
                    reportTitle = "Daily Sales Report";
                    query = @"
                    SELECT DATE(b.BookingTime) AS Label, 
                           COUNT(*) AS TicketsSold, 
                           COALESCE(SUM(s.TicketPrice), 0) AS TotalRevenue,
                           (SELECT Title FROM Movies m
                            JOIN Showtimes sh ON m.Id = sh.MovieID
                            WHERE sh.ShowtimeID = b.ShowtimeID
                            GROUP BY m.Id
                            ORDER BY COUNT(m.Id) DESC
                            LIMIT 1) AS TopMovie,
                           (SELECT Genre FROM Movies m
                            JOIN Showtimes sh ON m.Id = sh.MovieID
                            WHERE sh.ShowtimeID = b.ShowtimeID
                            GROUP BY m.Genre
                            ORDER BY COUNT(m.Genre) DESC
                            LIMIT 1) AS TopGenre,
                           COALESCE(SUM(s.TicketPrice) * 1.0 / COUNT(*), 0) AS AvgRevenuePerTicket
                    FROM Bookings b
                    JOIN Showtimes s ON b.ShowtimeID = s.ShowtimeID
                    GROUP BY Label
                    ORDER BY Label DESC;";
                    break;

                case 1: // Weekly
                    reportTitle = "Weekly Sales Report";
                    query = @"
                    SELECT strftime('%Y', b.BookingTime) || ' - Week ' || strftime('%W', b.BookingTime) AS Label,
                           COUNT(*) AS TicketsSold,
                           COALESCE(SUM(s.TicketPrice), 0) AS TotalRevenue,
                           (SELECT Title FROM Movies m
                            JOIN Showtimes sh ON m.Id = sh.MovieID
                            WHERE sh.ShowtimeID = b.ShowtimeID
                            GROUP BY m.Id
                            ORDER BY COUNT(m.Id) DESC
                            LIMIT 1) AS TopMovie,
                           (SELECT Genre FROM Movies m
                            JOIN Showtimes sh ON m.Id = sh.MovieID
                            WHERE sh.ShowtimeID = b.ShowtimeID
                            GROUP BY m.Genre
                            ORDER BY COUNT(m.Genre) DESC
                            LIMIT 1) AS TopGenre,
                           COALESCE(SUM(s.TicketPrice) * 1.0 / COUNT(*), 0) AS AvgRevenuePerTicket
                    FROM Bookings b
                    JOIN Showtimes s ON b.ShowtimeID = s.ShowtimeID
                    GROUP BY Label
                    ORDER BY Label DESC;";
                    break;

                case 2: // Monthly
                    reportTitle = "Monthly Sales Report";
                    query = @"
                    SELECT strftime('%Y-%m', b.BookingTime) AS Label,
                           COUNT(*) AS TicketsSold,
                           COALESCE(SUM(s.TicketPrice), 0) AS TotalRevenue,
                           (SELECT Title FROM Movies m
                            JOIN Showtimes sh ON m.Id = sh.MovieID
                            WHERE sh.ShowtimeID = b.ShowtimeID
                            GROUP BY m.Id
                            ORDER BY COUNT(m.Id) DESC
                            LIMIT 1) AS TopMovie,
                           (SELECT Genre FROM Movies m
                            JOIN Showtimes sh ON m.Id = sh.MovieID
                            WHERE sh.ShowtimeID = b.ShowtimeID
                            GROUP BY m.Genre
                            ORDER BY COUNT(m.Genre) DESC
                            LIMIT 1) AS TopGenre,
                           COALESCE(SUM(s.TicketPrice) * 1.0 / COUNT(*), 0) AS AvgRevenuePerTicket
                    FROM Bookings b
                    JOIN Showtimes s ON b.ShowtimeID = s.ShowtimeID
                    GROUP BY Label
                    ORDER BY Label DESC;";
                    break;

                case 3: // Yearly
                    reportTitle = "Yearly Sales Report";
                    query = @"
                    SELECT strftime('%Y', b.BookingTime) AS Label,
                           COUNT(*) AS TicketsSold,
                           COALESCE(SUM(s.TicketPrice), 0) AS TotalRevenue,
                           (SELECT Title FROM Movies m
                            JOIN Showtimes sh ON m.Id = sh.MovieID
                            WHERE sh.ShowtimeID = b.ShowtimeID
                            GROUP BY m.Id
                            ORDER BY COUNT(m.Id) DESC
                            LIMIT 1) AS TopMovie,
                           (SELECT Genre FROM Movies m
                            JOIN Showtimes sh ON m.Id = sh.MovieID
                            WHERE sh.ShowtimeID = b.ShowtimeID
                            GROUP BY m.Genre
                            ORDER BY COUNT(m.Genre) DESC
                            LIMIT 1) AS TopGenre,
                           COALESCE(SUM(s.TicketPrice) * 1.0 / COUNT(*), 0) AS AvgRevenuePerTicket
                    FROM Bookings b
                    JOIN Showtimes s ON b.ShowtimeID = s.ShowtimeID
                    GROUP BY Label
                    ORDER BY Label DESC;";
                    break;

                default:
                    MessageBox.Show("Invalid selection. Please choose a valid report type.");
                    return;
            }

            // Update the title in the titlePanel
            titlePanel.Controls.Clear();
            Label titleLabel = new Label
            {
                Text = reportTitle,
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = true,
                Location = new Point(10, 10)
            };
            titlePanel.Controls.Add(titleLabel);

            // Clear previous controls in the resultsPanel
            resultsPanel.Controls.Clear();

            // Create a new DataGridView
            DataGridView dataGridView = new DataGridView
            {
                Dock = DockStyle.Fill, // Fill the entire resultsPanel
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, // Adjust column widths to fill the grid
                ReadOnly = true, // Make the grid read-only
                AllowUserToAddRows = false, // Disable adding rows
                AllowUserToDeleteRows = false, // Disable deleting rows
                BackgroundColor = Color.White, // Set the background color
                AutoGenerateColumns = false, // Disable auto-generating columns to customize headers
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    ForeColor = Color.Black // Set the font color to black
                }
            };

            // Add the DataGridView to the resultsPanel
            resultsPanel.Controls.Add(dataGridView);

            // Add chart rendering logic to the existing code
            if (!string.IsNullOrEmpty(query))
            {
                try
                {
                    string dbPath = "Data Source=wildcattickets.db;Version=3;"; // Adjust if needed
                    using (SQLiteConnection conn = new SQLiteConnection(dbPath))
                    {
                        conn.Open();
                        using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            DataTable dataTable = new DataTable();
                            dataTable.Load(reader);

                            // Dynamically rename the "Report" header based on the selected index
                            string reportHeader;
                            switch (selectedIndex)
                            {
                                case 0:
                                    reportHeader = "Day";
                                    break;
                                case 1:
                                    reportHeader = "Week";
                                    break;
                                case 2:
                                    reportHeader = "Month";
                                    break;
                                case 3:
                                    reportHeader = "Year";
                                    break;
                                default:
                                    reportHeader = "Report";
                                    break;
                            }

                            // Existing DataGridView logic remains unchanged
                            dataGridView.Columns.Clear();
                            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = reportHeader, DataPropertyName = "Label" });
                            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Tickets Sold", DataPropertyName = "TicketsSold" });
                            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Revenue (PHP)", DataPropertyName = "TotalRevenue" });
                            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Top Movie", DataPropertyName = "TopMovie" });
                            dataGridView.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Top Genre", DataPropertyName = "TopGenre" });
                            dataGridView.Columns.Add(new DataGridViewTextBoxColumn
                            {
                                HeaderText = "Avg Revenue (PHP)",
                                DataPropertyName = "AvgRevenuePerTicket",
                                DefaultCellStyle = new DataGridViewCellStyle
                                {
                                    Format = "N2" // Format numbers to 2 decimal places
                                }
                            });
                            dataGridView.DataSource = dataTable;
                            dataGridView.RowHeadersVisible = false;

                            // New: Add chart rendering logic with descriptive titles
                            chartPanel.Controls.Clear(); // Clear previous charts
                            var labels = new List<string>();
                            var values = new ChartValues<double>(); // Use double to support revenue values

                            foreach (DataRow row in dataTable.Rows)
                            {
                                labels.Add(row["Label"].ToString());

                                // Use Tickets Sold or Revenue based on yAxisDropdown selection
                                if (yAxisDropdown.SelectedIndex == 1) // Revenue
                                {
                                    values.Add(Convert.ToDouble(row["TotalRevenue"]));
                                }
                                else // Default to Tickets Sold
                                {
                                    values.Add(Convert.ToInt32(row["TicketsSold"]));
                                }
                            }

                            LiveCharts.WinForms.CartesianChart chart = new LiveCharts.WinForms.CartesianChart
                            {
                                Dock = DockStyle.Fill
                            };

                            // Add descriptive title based on the selected index
                            string chartTitle = "";
                            switch (selectedIndex)
                            {
                                case 0: // Daily
                                    chart.Series.Add(new LineSeries
                                    {
                                        Title = yAxisDropdown.SelectedIndex == 1 ? "Revenue (PHP)" : "Tickets Sold",
                                        Values = values
                                    });
                                    chartTitle = yAxisDropdown.SelectedIndex == 1 ? "Daily Revenue Fluctuation" : "Daily Ticket Sales Fluctuation";
                                    break;

                                case 1: // Weekly
                                    chart.Series.Add(new ColumnSeries
                                    {
                                        Title = yAxisDropdown.SelectedIndex == 1 ? "Revenue (PHP)" : "Tickets Sold",
                                        Values = values
                                    });
                                    chartTitle = yAxisDropdown.SelectedIndex == 1 ? "Weekly Revenue Comparison" : "Weekly Ticket Sales Comparison";
                                    break;

                                case 2: // Monthly
                                    chart.Series.Add(new ColumnSeries
                                    {
                                        Title = yAxisDropdown.SelectedIndex == 1 ? "Revenue (PHP)" : "Tickets Sold",
                                        Values = values
                                    });
                                    chartTitle = yAxisDropdown.SelectedIndex == 1 ? "Monthly Revenue Comparison" : "Monthly Ticket Sales Comparison";
                                    break;

                                case 3: // Yearly
                                    chart.Series.Add(new ColumnSeries
                                    {
                                        Title = yAxisDropdown.SelectedIndex == 1 ? "Revenue (PHP)" : "Tickets Sold",
                                        Values = values
                                    });
                                    chartTitle = yAxisDropdown.SelectedIndex == 1 ? "Yearly Revenue Bar Chart" : "Yearly Ticket Sales Bar Chart";
                                    break;
                            }

                            chart.AxisX.Add(new Axis
                            {
                                Title = reportHeader,
                                Labels = labels,
                                Foreground = System.Windows.Media.Brushes.White
                            });

                            chart.AxisY.Add(new Axis
                            {
                                Title = yAxisDropdown.SelectedIndex == 1 ? "Revenue (PHP)" : "Tickets Sold",
                                LabelFormatter = value => yAxisDropdown.SelectedIndex == 1 ? value.ToString("C") : value.ToString("N0"),
                                Foreground = System.Windows.Media.Brushes.White
                            });

                            // Add a title label above the chart
                            Label chartTitleLabel = new Label
                            {
                                Text = chartTitle,
                                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                                ForeColor = Color.White,
                                AutoSize = true,
                                Dock = DockStyle.Top,
                                TextAlign = ContentAlignment.MiddleCenter
                            };
                            chartPanel.Controls.Add(chartTitleLabel); // Add the title label to the chartPanel
                            chartPanel.Controls.Add(chart); // Add the chart to the chartPanel

                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error generating report: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
