using Krypton.Toolkit;
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
    public partial class DashboardForm : KryptonForm
    {
        private bool isLoggingOut = false;
        public bool isSideBarExpanded;
        public int previousHeight;
        public string currentUser;

        public DashboardForm(string idNumber)
        {
            InitializeComponent();
            DatabaseHelper.InitializeDatabase();
            currentUser = idNumber;
            isSideBarExpanded = true;
            previousHeight = this.Height;
        }

        public void Dashboard_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1380, 770);
            currentUserTbx.Text = currentUser;

            if(currentUser == "admin")
            {
                homeBtn.Visible = false;
                userBtn.Visible = false;
                logoutContainer.Height += 35;
                moviesBtn_MouseClick(moviesBtn, new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0));
            }
            else
            {
                homeBtn.Visible = false;// true;
                userBtn.Visible = true;
            }
        }

        public void tabsBtn_Click(object sender, EventArgs e)
        {
            menuTransitionTimer.Start();
        }

        public void menuTransitionTimer_Tick(object sender, EventArgs e)
        {
            if (isSideBarExpanded)
            {
                tabsBtn.IconColor = Color.FromArgb(255, 219, 12);
                sidebarFlowPanel.Width -= 78;

                adjustSideBarButtonsWidth();
                clearSideBarButtonText();
                if (sidebarFlowPanel.Width <= 45)
                {
                    backgroundLogo.Visible = false;
                    sidebarFlowPanel.Width = 45;
                    logoutContainer.Height += backgroundLogo.Height+6;
                    menuTransitionTimer.Stop();
                    isSideBarExpanded = false;
                    tabsBtn.IconColor = Color.White;
                }

            } else
            {
                if (backgroundLogo.Visible == false) { 
                    backgroundLogo.Visible = true;
                    logoutContainer.Height -= backgroundLogo.Height + 6;
                }
                tabsBtn.IconColor = Color.FromArgb(255, 219, 12);
                sidebarFlowPanel.Width += 78;

                adjustSideBarButtonsWidth();
                restoreSideBarButtonText();
                if (sidebarFlowPanel.Width >= 200)
                {
                    sidebarFlowPanel.Width = 200;
                    menuTransitionTimer.Stop();
                    isSideBarExpanded = true;
                    tabsBtn.IconColor = Color.White;
                }
            }
        }

        public void adjustSideBarButtonsWidth()
        {
            homeBtn.Width = sidebarFlowPanel.Width;
            moviesBtn.Width = sidebarFlowPanel.Width;
            ticketsBtn.Width = sidebarFlowPanel.Width;
            logoutContainer.Width = sidebarFlowPanel.Width;
        }

        public void clearSideBarButtonText()
        {
            homeBtn.Text = "";
            moviesBtn.Text = "";
            ticketsBtn.Text = "";
            logoutBtn.Text = "";
        }

        public void restoreSideBarButtonText()
        {
            homeBtn.Text = "Home";
            moviesBtn.Text = "Movies";
            ticketsBtn.Text = "Tickets";
            logoutBtn.Text = "Logout";
        }

        public void Dashboard_SizeChanged(object sender, EventArgs e)
        {
            int heightDifference = this.Height - previousHeight;
            logoutContainer.Height += heightDifference;
            previousHeight = this.Height;
        }

        public void userBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            userBtn.IconColor = Color.FromArgb(255, 219, 12);
            userBtn.ForeColor = Color.FromArgb(255, 219, 12);

            if(currentUser != "admin")
            {
                ViewProfileForm profileForm = new ViewProfileForm(currentUser);
                profileForm.TopLevel = false;
                profileForm.FormBorderStyle = FormBorderStyle.None;
                profileForm.Dock = DockStyle.Fill;
                contentPanel.Controls.Clear();
                contentPanel.Controls.Add(profileForm);
                profileForm.fetchUserInfo();
                profileForm.Show();
            }
        }

        public void homeBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            homeBtn.IconColor = Color.FromArgb(255, 219, 12);
            homeBtn.ForeColor = Color.FromArgb(255, 219, 12);
            homeBtn.Font = new Font(homeBtn.Font, FontStyle.Bold);

            HomeForm homeForm = new HomeForm();
            homeForm.TopLevel = false;
            homeForm.FormBorderStyle = FormBorderStyle.None;
            homeForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(homeForm);
            homeForm.Show();
        }

        public void moviesBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            moviesBtn.IconColor = Color.FromArgb(255, 219, 12);
            moviesBtn.ForeColor = Color.FromArgb(255, 219, 12);
            moviesBtn.Font = new Font(moviesBtn.Font, FontStyle.Bold);


            CatalogForm catalogForm = new CatalogForm(currentUser);
            catalogForm.TopLevel = false;
            catalogForm.FormBorderStyle = FormBorderStyle.None;
            catalogForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(catalogForm);

            if (currentUser != "admin")
            {
                catalogForm.addMovieBtn.Visible = false;
            } 
            else
            {
                catalogForm.addMovieBtn.Visible = true;
            }

            catalogForm.Show();
        }

        public void ticketsBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            ticketsBtn.IconColor = Color.FromArgb(255, 219, 12);
            ticketsBtn.ForeColor = Color.FromArgb(255, 219, 12);
            ticketsBtn.Font = new Font(ticketsBtn.Font, FontStyle.Bold);

            BookingsForm ticketsForm = new BookingsForm(currentUser);
            ticketsForm.TopLevel = false;
            ticketsForm.FormBorderStyle = FormBorderStyle.None;
            ticketsForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(ticketsForm);
            ticketsForm.Show();
        }

        public void logoutBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            logoutBtn.IconColor = Color.FromArgb(255, 219, 12);
            logoutBtn.ForeColor = Color.FromArgb(255, 219, 12);
            logoutBtn.Font = new Font(logoutBtn.Font, FontStyle.Bold);

            // Confirm logout action
            var result = MessageBox.Show("Are you sure you want to log out?", "Logout", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                isLoggingOut = true;
                this.Close();
            }
        }

        public void resetSideBarColors()
        {
            userBtn.IconColor = Color.White;
            homeBtn.IconColor = Color.White;
            moviesBtn.IconColor = Color.White;
            ticketsBtn.IconColor = Color.White;
            logoutBtn.IconColor = Color.White;

            userBtn.ForeColor = Color.White;
            homeBtn.ForeColor = Color.White;
            moviesBtn.ForeColor = Color.White;
            ticketsBtn.ForeColor = Color.White;
            logoutBtn.ForeColor = Color.White;

            homeBtn.Font = new Font(homeBtn.Font, FontStyle.Regular);
            moviesBtn.Font = new Font(moviesBtn.Font, FontStyle.Regular);
            ticketsBtn.Font = new Font(ticketsBtn.Font, FontStyle.Regular);
            logoutBtn.Font = new Font(logoutBtn.Font, FontStyle.Regular);
        }
    }
}
