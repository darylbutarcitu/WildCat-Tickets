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
    public partial class DashboardForm: KryptonForm
    {
        private bool isSideBarExpanded;
        private int previousHeight;

        public DashboardForm()
        {
            InitializeComponent();
            isSideBarExpanded = true;
            previousHeight = this.Height;
        }

        private async void Dashboard_Load(object sender, EventArgs e)
        {
            // If not logged in, show login form
            // ------
            this.Size = new Size(1280, 720);
            FireBaseHelper.InitializeFirebase();
            await FireBaseHelper.TestFirestoreConnection();
            await FireBaseHelper.LoginUser("daryl.butar@cit.edu", "09261999!Db");
        }

        private void tabsBtn_Click(object sender, EventArgs e)
        {
            menuTransitionTimer.Start();
        }

        private void menuTransitionTimer_Tick(object sender, EventArgs e)
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

        private void adjustSideBarButtonsWidth()
        {
            homeBtn.Width = sidebarFlowPanel.Width;
            moviesBtn.Width = sidebarFlowPanel.Width;
            cinemaBtn.Width = sidebarFlowPanel.Width;
            eventsBtn.Width = sidebarFlowPanel.Width;
            ticketsBtn.Width = sidebarFlowPanel.Width;
            settingsBtn.Width = sidebarFlowPanel.Width;
            logoutContainer.Width = sidebarFlowPanel.Width;
        }

        private void clearSideBarButtonText()
        {
            homeBtn.Text = "";
            moviesBtn.Text = "";
            cinemaBtn.Text = "";
            eventsBtn.Text = "";
            ticketsBtn.Text = "";
            settingsBtn.Text = "";
            logoutBtn.Text = "";
        }

        private void restoreSideBarButtonText()
        {
            homeBtn.Text = "Home";
            moviesBtn.Text = "Movies";
            cinemaBtn.Text = "Cinema";
            eventsBtn.Text = "Events";
            ticketsBtn.Text = "Tickets";
            settingsBtn.Text = "Settings";
            logoutBtn.Text = "Logout";
        }

        private void Dashboard_SizeChanged(object sender, EventArgs e)
        {
            int heightDifference = this.Height - previousHeight;
            logoutContainer.Height += heightDifference;
            previousHeight = this.Height;
        }


        private void userBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            userBtn.IconColor = Color.FromArgb(255, 219, 12);
            userBtn.ForeColor = Color.FromArgb(255, 219, 12);

            ProfileForm profileForm = new ProfileForm();
            profileForm.TopLevel = false;
            profileForm.FormBorderStyle = FormBorderStyle.None;
            profileForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(profileForm);
            profileForm.Show();
        }

        private void homeBtn_MouseClick(object sender, MouseEventArgs e)
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

        private void moviesBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            moviesBtn.IconColor = Color.FromArgb(255, 219, 12);
            moviesBtn.ForeColor = Color.FromArgb(255, 219, 12);
            moviesBtn.Font = new Font(moviesBtn.Font, FontStyle.Bold);

            MoviesForm moviesForm = new MoviesForm();
            moviesForm.TopLevel = false;
            moviesForm.FormBorderStyle = FormBorderStyle.None;
            moviesForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(moviesForm);
            moviesForm.Show();
        }

        private void cinemaBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            cinemaBtn.IconColor = Color.FromArgb(255, 219, 12);
            cinemaBtn.ForeColor = Color.FromArgb(255, 219, 12);
            cinemaBtn.Font = new Font(cinemaBtn.Font, FontStyle.Bold);

            CinemasForm cinemasForm = new CinemasForm();
            cinemasForm.TopLevel = false;
            cinemasForm.FormBorderStyle = FormBorderStyle.None;
            cinemasForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(cinemasForm);
            cinemasForm.Show();
        }

        private void eventsBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            eventsBtn.IconColor = Color.FromArgb(255, 219, 12);
            eventsBtn.ForeColor = Color.FromArgb(255, 219, 12);
            eventsBtn.Font = new Font(eventsBtn.Font, FontStyle.Bold);

            EventsForm eventsForm = new EventsForm();
            eventsForm.TopLevel = false;
            eventsForm.FormBorderStyle = FormBorderStyle.None;
            eventsForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(eventsForm);
            eventsForm.Show();
        }

        private void ticketsBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            ticketsBtn.IconColor = Color.FromArgb(255, 219, 12);
            ticketsBtn.ForeColor = Color.FromArgb(255, 219, 12);
            ticketsBtn.Font = new Font(ticketsBtn.Font, FontStyle.Bold);

            MyTicketsForm ticketsForm = new MyTicketsForm();
            ticketsForm.TopLevel = false;
            ticketsForm.FormBorderStyle = FormBorderStyle.None;
            ticketsForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(ticketsForm);
            ticketsForm.Show();
        }

        private void settingsBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            settingsBtn.IconColor = Color.FromArgb(255, 219, 12);
            settingsBtn.ForeColor = Color.FromArgb(255, 219, 12);
            settingsBtn.Font = new Font(settingsBtn.Font, FontStyle.Bold);

            SettingsForm settingsForm = new SettingsForm();
            settingsForm.TopLevel = false;
            settingsForm.FormBorderStyle = FormBorderStyle.None;
            settingsForm.Dock = DockStyle.Fill;
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(settingsForm);
            settingsForm.Show();
        }

        private void logoutBtn_MouseClick(object sender, MouseEventArgs e)
        {
            resetSideBarColors();
            logoutBtn.IconColor = Color.FromArgb(255, 219, 12);
            logoutBtn.ForeColor = Color.FromArgb(255, 219, 12);
            logoutBtn.Font = new Font(logoutBtn.Font, FontStyle.Bold);
        }

        private void resetSideBarColors()
        {
            userBtn.IconColor = Color.White;
            homeBtn.IconColor = Color.White;
            moviesBtn.IconColor = Color.White;
            cinemaBtn.IconColor = Color.White;
            eventsBtn.IconColor = Color.White;
            ticketsBtn.IconColor = Color.White;
            settingsBtn.IconColor = Color.White;
            logoutBtn.IconColor = Color.White;

            userBtn.ForeColor = Color.White;
            homeBtn.ForeColor = Color.White;
            moviesBtn.ForeColor = Color.White;
            cinemaBtn.ForeColor = Color.White;
            eventsBtn.ForeColor = Color.White;
            ticketsBtn.ForeColor = Color.White;
            settingsBtn.ForeColor = Color.White;
            logoutBtn.ForeColor = Color.White;

            homeBtn.Font = new Font(homeBtn.Font, FontStyle.Regular);
            moviesBtn.Font = new Font(moviesBtn.Font, FontStyle.Regular);
            cinemaBtn.Font = new Font(cinemaBtn.Font, FontStyle.Regular);
            eventsBtn.Font = new Font(eventsBtn.Font, FontStyle.Regular);
            settingsBtn.Font = new Font(settingsBtn.Font, FontStyle.Regular);
            ticketsBtn.Font = new Font(ticketsBtn.Font, FontStyle.Regular);
            logoutBtn.Font = new Font(logoutBtn.Font, FontStyle.Regular);
        }

    }
}
