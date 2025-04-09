using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebaseAdmin.Auth;
using Google.Cloud.Firestore;

namespace WildCat_Tickets
{
    public partial class SignUpForm : TabForm
    {
        private static string profilePhotoUrl;

        public SignUpForm()
        {
            InitializeComponent();
        }

        private void SignUp_Load(object sender, EventArgs e)
        {
            this.Size = new Size(605, 560);
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
                    profilePhotoUrl = filePath;
                    profilePictureBox.Image = Image.FromFile(filePath);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile photo: " + ex.Message);
            }
        }

        private async void signUpBtn_Click(object sender, EventArgs e)
        {
            try
            {
                string email = emailTbx.Text;
                string password = passwordTbx.Text;
                string confirmPassword = confirmPasswordTbx.Text;
                string firstName = fNameTbx.Text;
                string lastName = lNameTbx.Text;
                string middleName = mNameTbx.Text;
                DateTime birthDate;
                if (!DateTime.TryParse(birthDateTbx.Text, out birthDate))
                {
                    MessageBox.Show("Invalid birth date format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string program = programTbx.Text;
                string year = yearTbx.Text;
                string phone = phoneTbx.Text;
                string idNumber = idNumberTbx.Text;

                if (password != confirmPassword)
                {
                    MessageBox.Show("Passwords do not match.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Use Firebase Admin SDK to create the user with email and password
                var userRecordArgs = new UserRecordArgs()
                {
                    Email = email,
                    Password = password,
                };

                UserRecord userRecord = await FirebaseAuth.DefaultInstance.CreateUserAsync(userRecordArgs);

                if (userRecord != null)
                {
                    // Store user details in Firestore
                    var userDoc = new Dictionary<string, object>
                    {
                        { "firstName", firstName },
                        { "lastName", lastName },
                        { "middleName", middleName },
                        { "birthDate", birthDate },
                        { "program", program },
                        { "year", year },
                        { "contact", new Dictionary<string, string> { { "phone", phone }, { "email", email } } }
                    };

                    DocumentReference docRef = FireBaseHelper.db.Collection("users").Document(idNumber);
                    await docRef.SetAsync(userDoc);

                    if (!string.IsNullOrEmpty(profilePhotoUrl))
                    {
                        string photoPublicID = CloudinaryHelper.UploadFile(profilePhotoUrl);
                        string profilePhotoUrlFromCloudinary = CloudinaryHelper.GetCloudinaryUrl(photoPublicID, "jpg");
                        await FireBaseHelper.StoreProfileImageUrl(profilePhotoUrlFromCloudinary, idNumber);
                    }

                    MessageBox.Show("Sign up successful!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error signing up: " + ex.Message);
                MessageBox.Show("Error signing up: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
