using FontAwesome.Sharp;
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

namespace WildCat_Tickets
{
    public partial class ViewProfileForm : TabForm
    {
        private User currentUser;
        public ViewProfileForm(string idNumber)
        {
            InitializeComponent();
            currentUser = new User();
            currentUser.Id = idNumber;
            this.Load += ProfileForm_Load;
            this.Resize += ProfileForm_Resize;
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 675);
            profilePictureBox.Size = new Size(150, 150);
            CenterControls();
            fetchUserInfo();
            displayUserInfo();
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

        private void uploadProfilePhoto_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.Title = "Select Profile Photo";
                    openFileDialog.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
                    openFileDialog.Multiselect = false;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        string selectedFilePath = openFileDialog.FileName;
                        currentUser.ProfileUrl = selectedFilePath;
                        LoadProfilePhoto(selectedFilePath);
                        MessageBox.Show("Profile photo selected successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while selecting the profile photo: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        internal void fetchUserInfo()
        {
            try
            {
                string currentUserId = currentUser.Id;

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    string query = "SELECT FirstName, MiddleName, LastName, LastName, Program, Year, IDNumber, BirthDate, Email, Phone, ProfilePhotoPath FROM Users WHERE IDNumber = @IDNumber";
                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@IDNumber", currentUserId);

                        using (SQLiteDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                currentUser.FirstName = reader["FirstName"]?.ToString() ?? string.Empty;
                                currentUser.MiddleName = reader["MiddleName"]?.ToString() ?? string.Empty;
                                currentUser.LastName = reader["LastName"]?.ToString() ?? string.Empty;
                                currentUser.Program = reader["Program"]?.ToString() ?? string.Empty;
                                currentUser.Year = reader["Year"]?.ToString() ?? string.Empty;
                                currentUser.Id = reader["IDNumber"]?.ToString() ?? string.Empty;

                                if (reader["BirthDate"] != DBNull.Value && DateTime.TryParse(reader["BirthDate"].ToString(), out DateTime birthDate))
                                {
                                    currentUser.BirthDate = birthDate;
                                }

                                currentUser.Email = reader["Email"]?.ToString() ?? string.Empty;
                                currentUser.Phone = reader["Phone"]?.ToString() ?? string.Empty;
                                currentUser.ProfileUrl = reader["ProfilePhotoPath"]?.ToString() ?? string.Empty;
                            }
                            else
                            {
                                MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while fetching user information: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            uploadProfilePhotoBtn.Visible = true;
            saveBtn.Visible = true;
            editBtn.Visible = false;
            deleteBtn.Visible = false;
            cancelBtn.Visible = true;

            fNameTbx.IsReadOnly = false;
            mNameTbx.IsReadOnly = false;
            lNameTbx.IsReadOnly = false;
            programTbx.IsReadOnly = false;
            yearTbx.IsReadOnly = false;
            birthDateTbx.IsReadOnly = false;
            //emailTbx.IsReadOnly = false;
            phoneTbx.IsReadOnly = false;
        }

        private void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input fields
                if (string.IsNullOrWhiteSpace(fNameTbx.Text) || string.IsNullOrWhiteSpace(lNameTbx.Text) || string.IsNullOrWhiteSpace(phoneTbx.Text))
                {
                    MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!DateTime.TryParse(birthDateTbx.Text, out DateTime birthDate))
                {
                    MessageBox.Show("Invalid birth date format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Update currentUser object with the latest values from the form
                currentUser.FirstName = fNameTbx.Text.Trim();
                currentUser.MiddleName = mNameTbx.Text.Trim();
                currentUser.LastName = lNameTbx.Text.Trim();
                currentUser.Program = programTbx.Text.Trim();
                currentUser.Year = yearTbx.Text.Trim();
                currentUser.BirthDate = birthDate;
                currentUser.Email = emailTbx.Text.Trim();
                currentUser.Phone = phoneTbx.Text.Trim();

                using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                {
                    conn.Open();

                    // SQL query to update the user information
                    string query = @"
                        UPDATE Users
                        SET 
                            FirstName = @FirstName,
                            MiddleName = @MiddleName,
                            LastName = @LastName,
                            Program = @Program,
                            Year = @Year,
                            BirthDate = @BirthDate,
                            Email = @Email,
                            Phone = @Phone,
                            ProfilePhotoPath = @ProfilePhotoPath
                        WHERE IDNumber = @IDNumber";

                    using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                    {
                        // Bind parameters
                        cmd.Parameters.AddWithValue("@FirstName", currentUser.FirstName);
                        cmd.Parameters.AddWithValue("@MiddleName", currentUser.MiddleName);
                        cmd.Parameters.AddWithValue("@LastName", currentUser.LastName);
                        cmd.Parameters.AddWithValue("@Program", currentUser.Program);
                        cmd.Parameters.AddWithValue("@Year", currentUser.Year);
                        cmd.Parameters.AddWithValue("@BirthDate", currentUser.BirthDate.ToString("yyyy-MM-dd"));
                        cmd.Parameters.AddWithValue("@Email", currentUser.Email);
                        cmd.Parameters.AddWithValue("@Phone", currentUser.Phone);
                        cmd.Parameters.AddWithValue("@ProfilePhotoPath", currentUser.ProfileUrl);
                        cmd.Parameters.AddWithValue("@IDNumber", currentUser.Id);

                        // Execute the update query
                        int rowsAffected = cmd.ExecuteNonQuery();

                        if (rowsAffected > 0)
                        {
                            MessageBox.Show("Profile updated successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            MessageBox.Show("Failed to update profile. User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }

                    conn.Close();
                }

                // Reset the form to read-only mode
                cancelBtn_Click(sender, e);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while saving the profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void cancelBtn_Click(object sender, EventArgs e)
        {
            fNameTbx.IsReadOnly = true;
            mNameTbx.IsReadOnly = true;
            lNameTbx.IsReadOnly = true;
            programTbx.IsReadOnly = true;
            yearTbx.IsReadOnly = true;
            birthDateTbx.IsReadOnly = true;
            //emailTbx.IsReadOnly = true;
            phoneTbx.IsReadOnly = true;

            uploadProfilePhotoBtn.Visible = false;
            saveBtn.Visible = false;
            editBtn.Visible = true;
            deleteBtn.Visible = true;
            cancelBtn.Visible = false;

            fetchUserInfo();
            displayUserInfo();
        }
        private void LoadProfilePhoto(string profilePhotoPath)
        {
            if (!string.IsNullOrEmpty(profilePhotoPath) && System.IO.File.Exists(profilePhotoPath))
            {
                profilePictureBox.Image = Image.FromFile(profilePhotoPath);
            }
        }
        internal void displayUserInfo()
        {
            try
            {
                fNameTbx.Text = currentUser.FirstName;
                mNameTbx.Text = currentUser.MiddleName;
                lNameTbx.Text = currentUser.LastName;
                programTbx.Text = currentUser.Program;
                yearTbx.Text = currentUser.Year;
                idNumberTbx.Text = currentUser.Id;
                birthDateTbx.Text = currentUser.BirthDate != DateTime.MinValue
                    ? currentUser.BirthDate.ToString("yyyy-MM-dd")
                    : string.Empty;
                emailTbx.Text = currentUser.Email;
                phoneTbx.Text = currentUser.Phone;

                LoadProfilePhoto(currentUser.ProfileUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show("An error occurred while displaying user information: " + ex.Message,
                                "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void deleteBtn_Click(object sender, EventArgs e)
        {
            // Prompt the user to enter their password
            using (Form passwordPrompt = new Form())
            {
                passwordPrompt.Text = "Confirm Deletion";
                passwordPrompt.Size = new Size(300, 150);
                passwordPrompt.StartPosition = FormStartPosition.CenterParent;

                Label promptLabel = new Label
                {
                    Text = "Enter your password to confirm:",
                    AutoSize = true,
                    Location = new Point(10, 10)
                };

                TextBox passwordTbx = new TextBox
                {
                    PasswordChar = '*', // Mask the characters
                    Width = 250,
                    Location = new Point(10, 40)
                };

                Button confirmBtn = new Button
                {
                    Text = "Confirm",
                    DialogResult = DialogResult.OK,
                    Location = new Point(10, 80)
                };

                Button cancelBtn = new Button
                {
                    Text = "Cancel",
                    DialogResult = DialogResult.Cancel,
                    Location = new Point(100, 80)
                };

                passwordPrompt.Controls.Add(promptLabel);
                passwordPrompt.Controls.Add(passwordTbx);
                passwordPrompt.Controls.Add(confirmBtn);
                passwordPrompt.Controls.Add(cancelBtn);

                passwordPrompt.AcceptButton = confirmBtn;
                passwordPrompt.CancelButton = cancelBtn;

                if (passwordPrompt.ShowDialog() == DialogResult.OK)
                {
                    string enteredPassword = passwordTbx.Text;

                    // Validate the entered password
                    try
                    {
                        using (SQLiteConnection conn = new SQLiteConnection("Data Source=wildcattickets.db;Version=3;"))
                        {
                            conn.Open();

                            string query = "SELECT Password FROM Users WHERE IDNumber = @IDNumber";
                            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
                            {
                                cmd.Parameters.AddWithValue("@IDNumber", currentUser.Id);

                                object result = cmd.ExecuteScalar();
                                if (result != null)
                                {
                                    string storedPasswordHash = result.ToString();
                                    string enteredPasswordHash = DatabaseHelper.HashPassword(enteredPassword);

                                    if (storedPasswordHash == enteredPasswordHash)
                                    {
                                        // Password is correct, proceed to delete the user
                                        string deleteQuery = "DELETE FROM Users WHERE IDNumber = @IDNumber";
                                        using (SQLiteCommand deleteCmd = new SQLiteCommand(deleteQuery, conn))
                                        {
                                            deleteCmd.Parameters.AddWithValue("@IDNumber", currentUser.Id);
                                            deleteCmd.ExecuteNonQuery();
                                        }

                                        MessageBox.Show("Account deleted successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                                        // Close the dashboard and redirect to the login page
                                        foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
                                        {
                                            if (openForm is DashboardForm)
                                            {
                                                openForm.Close();
                                            }
                                        }

                                        this.Close(); // Close the current form
                                                      // Close all open forms except the current one
                                        foreach (Form openForm in Application.OpenForms.Cast<Form>().ToList())
                                        {
                                            if (openForm is DashboardForm)
                                            {
                                                openForm.Close();
                                            }
                                        }

                                        // Close the current form
                                        this.Close();
                                    }
                                    else
                                    {
                                        MessageBox.Show("Incorrect password. Please try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    }
                                }
                                else
                                {
                                    MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                }
                            }

                            conn.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("An error occurred while deleting the account: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
    }
}
