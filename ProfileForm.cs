using FontAwesome.Sharp;
using Google.Cloud.Firestore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WildCat_Tickets
{
    public partial class ProfileForm : TabForm
    {
        public static string profilePhotoUrl = "";
        public static string photoPublicID = "";
        private Customer _currentCustomer;
        public ProfileForm(Customer customer)
        {
            InitializeComponent();
            _currentCustomer = customer;
            this.Load += ProfileForm_Load;
            this.Resize += ProfileForm_Resize;
        }

        private void ProfileForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 675);
            profilePictureBox.Size = new Size(150, 150);
            CenterControls();
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

        private async void uploadProfilePhoto_Click(object sender, EventArgs e)
        {
            try
            {
                string filePath = null;
                using (OpenFileDialog openFileDialog = new OpenFileDialog())
                {
                    openFileDialog.InitialDirectory = "c:\\";
                    openFileDialog.Filter = "All files (*.*)|*.*";
                    openFileDialog.FilterIndex = 1;
                    openFileDialog.RestoreDirectory = true;

                    if (openFileDialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = openFileDialog.FileName;
                    }
                }

                if (filePath != null)
                {
                    await Task.Run(() =>
                    {
                        photoPublicID = CloudinaryHelper.UploadFile(filePath);
                        profilePhotoUrl = CloudinaryHelper.GetCloudinaryUrl(photoPublicID, "jpg");

                        this.Invoke((MethodInvoker)delegate
                        {
                            MessageBox.Show("URL: " + profilePhotoUrl);
                            loadProfilePhoto(profilePhotoUrl);
                        });

                        FireBaseHelper.StoreProfileImageUrl(profilePhotoUrl, _currentCustomer.Id).Wait();
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error uploading profile photo: " + ex.Message);
            }
        }

        internal void displayUserInfo()
        {
            fNameTbx.Text = _currentCustomer.FirstName;
            mNameTbx.Text = _currentCustomer.MiddleName;
            lNameTbx.Text = _currentCustomer.LastName;
            programTbx.Text = _currentCustomer.Program;
            yearTbx.Text = _currentCustomer.Year.ToString();
            birthDateTbx.Text = _currentCustomer.BirthDate.ToString("yyyy-MM-dd");
            emailTbx.Text = _currentCustomer.Email;
            phoneTbx.Text = _currentCustomer.Phone;
            loadProfilePhoto(_currentCustomer.ProfileUrl);
        }

        private void loadProfilePhoto(string profilePhotoUrl)
        {
            try
            {
                if (string.IsNullOrEmpty(profilePhotoUrl))
                {
                    return;
                }
                var request = System.Net.WebRequest.Create(profilePhotoUrl);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                {
                    profilePictureBox.Image = Bitmap.FromStream(stream);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading profile photo: " + ex.Message);
            }
        }

        private void editBtn_Click(object sender, EventArgs e)
        {
            saveBtn.Visible = true;
            editBtn.Visible = false;
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

        private async void saveBtn_Click(object sender, EventArgs e)
        {
            try
            {
                // Validate input fields
                if (string.IsNullOrWhiteSpace(fNameTbx.Text) || string.IsNullOrWhiteSpace(lNameTbx.Text) || string.IsNullOrWhiteSpace(phoneTbx.Text))
                {
                    MessageBox.Show("Please fill in all required fields.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                DateTime birthDate;
                if (!DateTime.TryParse(birthDateTbx.Text, out birthDate))
                {
                    MessageBox.Show("Invalid birth date format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                await _currentCustomer.UpdateProfileDetails(
                    fNameTbx.Text,
                    lNameTbx.Text,
                    mNameTbx.Text,
                    birthDate,
                    programTbx.Text,
                    yearTbx.Text,
                    phoneTbx.Text
                );

                saveBtn.Visible = false;
                editBtn.Visible = true;
                cancelBtn.Visible = false;
                fNameTbx.IsReadOnly = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            cancelBtn_Click(sender, e);
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

            saveBtn.Visible = false;
            editBtn.Visible = true;
            cancelBtn.Visible = false;
        }
    }
}
