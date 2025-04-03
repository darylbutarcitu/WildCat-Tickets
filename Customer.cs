using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using System.Net.NetworkInformation;
using Newtonsoft.Json;

namespace WildCat_Tickets
{
    public class Customer : Account
    {
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string ProfileUrl { get; set; }
        public string Phone { get; set; }
        public DateTime BirthDate { get; set; }
        public string Program { get; set; }
        public string Year { get; set; }

        // Implement the abstract Login method from Person class
        public override async Task<string> Login(string idNumber, string password)
        {
            try
            {
                if (!FireBaseHelper.IsInternetAvailable())
                {
                    MessageBox.Show("No internet connection. Please check your connection and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }

                CollectionReference usersCollection = FireBaseHelper.db.Collection("users");
                DocumentReference docRef = usersCollection.Document(idNumber);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    var documentData = snapshot.ToDictionary();

                    if (documentData.TryGetValue("contact", out var contactObj) && contactObj is Dictionary<string, object> contact)
                    {
                        if (contact.TryGetValue("email", out var emailObj) && emailObj is string email)
                        {
                            var token = await FireBaseHelper.AuthenticateUserEmail(email, password);
                            if (token != null)
                            {
                                this.Email = email;
                                this.Id = idNumber;
                                await this.FetchProfileDetails();
                                Console.WriteLine($"User {email} logged in successfully.");
                                MessageBox.Show($"User {email} logged in successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                return token;
                            }
                            else
                            {
                                Console.WriteLine("Invalid email or password.");
                                MessageBox.Show("Invalid email or password.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                return null;
                            }
                        }
                        else
                        {
                            Console.WriteLine("Email not found in contact information.");
                            MessageBox.Show("Email not found in contact information.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return null;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Contact information not found.");
                        MessageBox.Show("Contact information not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return null;
                    }
                }
                else
                {
                    Console.WriteLine("User ID not found.");
                    MessageBox.Show("User ID not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return null;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error logging in user: " + ex.Message);
                MessageBox.Show("Error logging in user: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }

        public async Task FetchProfileDetails()
        {
            try
            {
                if (!FireBaseHelper.IsInternetAvailable())
                {
                    MessageBox.Show("No internet connection. Please check your connection and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CollectionReference usersCollection = FireBaseHelper.db.Collection("users");
                DocumentReference docRef = usersCollection.Document(this.Id);
                DocumentSnapshot snapshot = await docRef.GetSnapshotAsync();

                if (snapshot.Exists)
                {
                    var documentData = snapshot.ToDictionary();
                    if (documentData.TryGetValue("profile", out var profileObj) && profileObj is Dictionary<string, object> profile)
                    {
                        // Get the user firstName
                        if (profile.TryGetValue("firstName", out var firstNameObj) && firstNameObj is string firstName)
                        {
                            this.FirstName = firstName;
                            MessageBox.Show("First Name: " + firstName, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            Console.WriteLine("First Name not found.");
                            MessageBox.Show("First Name not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Get the user lastName
                        if (profile.TryGetValue("lastName", out var lastNameObj) && lastNameObj is string lastName)
                        {
                            this.LastName = lastName;
                            MessageBox.Show("Last Name: " + lastName, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            Console.WriteLine("Last Name not found.");
                            MessageBox.Show("Last Name not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Get the user middleName
                        if (profile.TryGetValue("middleName", out var middleNameObj) && middleNameObj is string middleName)
                        {
                            this.MiddleName = middleName;
                            MessageBox.Show("Middle Name: " + middleName, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            Console.WriteLine("Middle Name not found.");
                            MessageBox.Show("Middle Name not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Get the user birthDate (map: day, month, year)
                        if (profile.TryGetValue("birthDate", out var birthDateObj) && birthDateObj is Dictionary<string, object> birthDate)
                        {
                            if (birthDate.TryGetValue("day", out var dayObj) && dayObj is long day &&
                                birthDate.TryGetValue("month", out var monthObj) && monthObj is long month &&
                                birthDate.TryGetValue("year", out var yearObj) && yearObj is long year)
                            {
                                this.BirthDate = new DateTime((int)year, (int)month, (int)day);
                                MessageBox.Show("Birth Date: " + this.BirthDate.ToString("d"), "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                Console.WriteLine("Birth Date not found.");
                                MessageBox.Show("Birth Date not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }


                        // Get the user profileURL
                        if (profile.TryGetValue("profileURL", out var profileUrlObj) && profileUrlObj is string profileUrl)
                        {
                            this.ProfileUrl = profileUrl;
                            MessageBox.Show("Profile URL retrieved successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                        else
                        {
                            Console.WriteLine("Profile URL not found.");
                            MessageBox.Show("Profile URL not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }

                        // Get the user course
                        if (documentData.TryGetValue("course", out var courseObj) && courseObj is Dictionary<string, object> course)
                        {
                            // program
                            if (course.TryGetValue("program", out var programObj) && programObj is string program)
                            {
                                this.Program = program;
                                MessageBox.Show("Program: " + program, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                Console.WriteLine("Program not found.");
                                MessageBox.Show("Program not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }

                            // year level
                            if (course.TryGetValue("year", out var yearObj) && yearObj is string year)
                            {
                                this.Year = year;
                                MessageBox.Show("Year: " + year, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                Console.WriteLine("Year not found.");
                                MessageBox.Show("Year not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                        // Get the user phone1
                        if (documentData.TryGetValue("contact", out var contactObj) && contactObj is Dictionary<string, object> contact)
                        {
                            if (contact.TryGetValue("phone1", out var phoneObj) && phoneObj is string phone)
                            {
                                this.Phone = phone;
                                MessageBox.Show("Phone: " + phone, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            }
                            else
                            {
                                Console.WriteLine("Phone not found.");
                                MessageBox.Show("Phone not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }

                    }
                    else
                    {
                        Console.WriteLine("Profile information not found.");
                        MessageBox.Show("Profile information not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Console.WriteLine("User ID not found.");
                    MessageBox.Show("User ID not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting profile image URL: " + ex.Message);
                MessageBox.Show("Error getting profile image URL: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        public async Task UpdateProfileDetails(string firstName, string lastName, string middleName, DateTime birthDate, string program, string year, string phone)
        {
            try
            {
                if (!FireBaseHelper.IsInternetAvailable())
                {
                    MessageBox.Show("No internet connection. Please check your connection and try again.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                CollectionReference usersCollection = FireBaseHelper.db.Collection("users");
                DocumentReference docRef = usersCollection.Document(this.Id);

                var updateData = new Dictionary<string, object>();

                if (!string.IsNullOrEmpty(firstName))
                {
                    updateData["profile.firstName"] = firstName;
                }

                if (!string.IsNullOrEmpty(lastName))
                {
                    updateData["profile.lastName"] = lastName;
                }

                if (!string.IsNullOrEmpty(middleName))
                {
                    updateData["profile.middleName"] = middleName;
                }

                if (birthDate != DateTime.MinValue)
                {
                    updateData["profile.birthDate"] = new Dictionary<string, object>
                    {
                        { "day", birthDate.Day },
                        { "month", birthDate.Month },
                        { "year", birthDate.Year }
                    };
                }

                if (!string.IsNullOrEmpty(program))
                {
                    updateData["course.program"] = program;
                }

                if (!string.IsNullOrEmpty(year))
                {
                    updateData["course.year"] = year;
                }

                if (!string.IsNullOrEmpty(phone))
                {
                    updateData["contact.phone1"] = phone;
                }

                await docRef.UpdateAsync(updateData);
                MessageBox.Show("Profile updated successfully.", "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating profile: " + ex.Message);
                MessageBox.Show("Error updating profile: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
