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
using System.Security.Cryptography;
using System.Windows.Controls;

namespace WildCat_Tickets
{
    public partial class SignUpForm : TabForm
    {
        private static string profilePhotoPath;

        public SignUpForm()
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
        }

        private void SignUp_Load(object sender, EventArgs e)
        {
            this.Size = new Size(605, 580);
            profilePictureBox.Size = new Size(150, 150);
        }

        private void ProfileForm_Resize(object sender, EventArgs e)
        {
            CenterControls();
        }

        private void CenterControls()
        {
            profileContainer.Left = (this.ClientSize.Width - profileContainer.Width) / 2;
            profileContainer.Top = (this.ClientSize.Height - profileContainer.Height) / 2;

        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to cancel?", "Confirm Cancel", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                profilePhotoPath = "";
                profilePictureBox.Image = null;
                this.Close();
            }
        }

        private void uploadProfilePhotoBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = null;
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "Image files (*.jpg, *.jpeg, *.png) | *.jpg; *.jpeg; *.png";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = openFileDialog.FileName;
                    }
                }

                if (filePath != null)
                {
                    profilePhotoPath = filePath;
                    profilePictureBox.Image = System.Drawing.Image.FromFile(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile photo: " + ex.Message);
            }
        }

        private void signUpBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string email = emailTbx.Text.Trim();
                string password = passwordTbx.Text;
                string confirmPassword = confirmPasswordTbx.Text;
                string firstName = fNameTbx.Text.Trim();
                string lastName = lNameTbx.Text.Trim();
                string middleName = mNameTbx.Text.Trim();
                string program = programTbx.Text.Trim();
                string year = yearTbx.Text.Trim();
                string phone = phoneTbx.Text.Trim();
                string idNumber = idNumberTbx.Text.Trim();
                string birthDateText = birthDateTbx.Text.Trim();

                // Check if any field is empty
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword) ||
                    string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(program) ||
                    string.IsNullOrEmpty(year) || string.IsNullOrEmpty(phone) || string.IsNullOrEmpty(idNumber) ||
                    string.IsNullOrEmpty(birthDateText))
                {
                    MessageBox.Show("All fields are required. Please fill in all fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!DateTime.TryParse(birthDateText, out DateTime birthDate))
                {
                    MessageBox.Show("Invalid birth date format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (password != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Hash the password
                string hashedPassword = DatabaseHelper.HashPassword(password);

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // Ensure the Users table exists
                    string createTableQuery = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        IDNumber TEXT PRIMARY KEY,
                        FirstName TEXT,
                        MiddleName TEXT,
                        LastName TEXT,
                        BirthDate TEXT,
                        Program TEXT,
                        Year TEXT,
                        Phone TEXT,
                        Email TEXT UNIQUE,
                        Password TEXT,
                        ProfilePhotoPath TEXT
                        Role TEXT
                    );";

                    using (SQLiteCommand createCmd = new SQLiteCommand(createTableQuery, conn))
                    {
                        createCmd.ExecuteNonQuery();
                    }

                    // Optional: Check if email or ID number already exists
                    string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email OR IDNumber = @IDNumber";
                    using (SQLiteCommand checkCmd = new SQLiteCommand(checkQuery, conn))
                    {
                        checkCmd.Parameters.AddWithValue("@Email", email);
                        checkCmd.Parameters.AddWithValue("@IDNumber", idNumber);
                        long count = (long)checkCmd.ExecuteScalar();
                        if (count > 0)
                        {
                            MessageBox.Show("Email or ID number already exists.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }
                    }

                    // Insert user
                    // Insert user
                    string insertQuery = @"
                        INSERT INTO Users (IDNumber, FirstName, MiddleName, LastName, BirthDate, Program, Year, Phone, Email, Password, ProfilePhotoPath, Role)
                        VALUES (@IDNumber, @FirstName, @MiddleName, @LastName, @BirthDate, @Program, @Year, @Phone, @Email, @Password, @ProfilePhotoPath, @Role)";

                    using (SQLiteCommand cmd = new SQLiteCommand(insertQuery, conn))
                    {
                        cmd.Parameters.AddWithValue("@IDNumber", idNumber);
                        cmd.Parameters.AddWithValue("@FirstName", firstName);
                        cmd.Parameters.AddWithValue("@MiddleName", middleName);
                        cmd.Parameters.AddWithValue("@LastName", lastName);
                        cmd.Parameters.AddWithValue("@BirthDate", birthDate);
                        cmd.Parameters.AddWithValue("@Program", program);
                        cmd.Parameters.AddWithValue("@Year", year);
                        cmd.Parameters.AddWithValue("@Phone", phone);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", hashedPassword); // Store hashed password
                        cmd.Parameters.AddWithValue("@ProfilePhotoPath", profilePhotoPath ?? "");
                        cmd.Parameters.AddWithValue("@Role", "Student"); // Default role
                        cmd.ExecuteNonQuery();
                    }

                    conn.Close();
                }

                MessageBox.Show("Sign up successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                this.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error signing up: " + ex.Message);
                MessageBox.Show("Error signing up: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void SignUpForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                signUpBtn.PerformClick();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                cancelBtn.PerformClick();
            }
        }

    }
}
