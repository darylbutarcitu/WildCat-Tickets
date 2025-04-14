using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SQLite;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WildCat_Tickets
{
    public partial class LoginForm : TabForm
    {
        public DashboardForm dashboardForm;

        public LoginForm()
        {
            InitializeComponent();
            this.Load += LoginForm_Load;
            this.Resize += LoginForm_Resize;
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(667, 321);
            CenterControls();
        }
        private void LoginForm_Resize(object sender, EventArgs e)
        {
            CenterControls();
        }

        private void CenterControls()
        {
            loginContainer.Left = (this.ClientSize.Width - loginContainer.Width) / 2;
            loginContainer.Top = (this.ClientSize.Height - loginContainer.Height) / 2;

        }

        private void signUpBtn_Click(object sender, EventArgs e)
        {
            this.Hide();
            SignUpForm signUpForm = new SignUpForm();
            signUpForm.ShowDialog();
            clearTextBoxes();
            this.Show();
        }

        private void LoginForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                loginBtn.PerformClick();
            }
        }
        private void loginBtn_Click(object sender, EventArgs e)
        {
            string idNumber = idNumberTbx.Text.Trim();
            string password = passwordTbx.Text;

            // Check for empty fields
            if (string.IsNullOrEmpty(idNumber) || string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Please enter both ID Number and Password.", "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Hash the entered password
                string hashedPassword = DatabaseHelper.HashPassword(password);

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // Ensure WAL mode is enabled
                    using (SQLiteCommand cmd = new SQLiteCommand("PRAGMA journal_mode=WAL;", conn))
                    {
                        cmd.ExecuteNonQuery();
                    }

                    string query = "SELECT IDNumber, Role FROM Users WHERE IDNumber = @IDNumber AND Password = @Password";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IDNumber", idNumber);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string role = reader["Role"]?.ToString() ?? "Student";

                                if (role.ToLower() == "admin")
                                {
                                    MessageBox.Show("Admin Login successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }
                                else
                                {
                                    MessageBox.Show($"User Login successful! Role: {role}", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                }

                                OpenDashboard(idNumber);
                                clearTextBoxes();
                            }
                            else
                            {
                                MessageBox.Show("Invalid ID number or password.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred during login:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void clearTextBoxes()
        {
            idNumberTbx.Clear();
            passwordTbx.Clear();
        }

        private void OpenDashboard(string userId)
        {
            this.Hide();
            dashboardForm = new DashboardForm(userId);
            dashboardForm.FormClosed += (s, e) => this.Show();
            dashboardForm.Show(); 
        }

        private void idNumberTbx_Click(object sender, EventArgs e)
        {

        }
    }
}
