namespace WildCat_Tickets
{
    partial class SalesReportForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            SiticoneNetFrameworkUI.SiticoneFlowPanel.LayoutState layoutState1 = new SiticoneNetFrameworkUI.SiticoneFlowPanel.LayoutState();
            SiticoneNetFrameworkUI.SiticoneFlowPanel.LayoutState layoutState2 = new SiticoneNetFrameworkUI.SiticoneFlowPanel.LayoutState();
            SiticoneNetFrameworkUI.SiticoneFlowPanel.LayoutState layoutState3 = new SiticoneNetFrameworkUI.SiticoneFlowPanel.LayoutState();
            SiticoneNetFrameworkUI.SiticoneFlowPanel.LayoutState layoutState4 = new SiticoneNetFrameworkUI.SiticoneFlowPanel.LayoutState();
            this.filterPanel = new SiticoneNetFrameworkUI.SiticoneFlatPanel();
            this.yAxisDataLabel = new SiticoneNetFrameworkUI.SiticoneLabel();
            this.yAxisDropdown = new SiticoneNetFrameworkUI.SiticoneDropdown();
            this.siticoneFlatPanel1 = new SiticoneNetFrameworkUI.SiticoneFlatPanel();
            this.salesViewType = new SiticoneNetFrameworkUI.SiticoneLabel();
            this.salesReportDropown = new SiticoneNetFrameworkUI.SiticoneDropdown();
            this.siticoneHumanizerDateTime1 = new SiticoneNetFrameworkUI.SiticoneHumanizerDateTime(this.components);
            this.contentPanel = new SiticoneNetFrameworkUI.SiticoneFlatPanel();
            this.resultsPanel = new SiticoneNetFrameworkUI.SiticoneFlatPanel();
            this.titlePanel = new SiticoneNetFrameworkUI.SiticoneFlatPanel();
            this.chartPanel = new SiticoneNetFrameworkUI.SiticoneFlatPanel();
            this.filterPanel.SuspendLayout();
            this.contentPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // filterPanel
            // 
            this.filterPanel.BackColor = System.Drawing.Color.Transparent;
            this.filterPanel.Controls.Add(this.yAxisDataLabel);
            this.filterPanel.Controls.Add(this.yAxisDropdown);
            this.filterPanel.Controls.Add(this.siticoneFlatPanel1);
            this.filterPanel.Controls.Add(this.salesViewType);
            this.filterPanel.Controls.Add(this.salesReportDropown);
            this.filterPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.filterPanel.Location = new System.Drawing.Point(0, 0);
            this.filterPanel.MinimumSize = new System.Drawing.Size(20, 20);
            this.filterPanel.Name = "filterPanel";
            this.filterPanel.Size = new System.Drawing.Size(1064, 105);
            this.filterPanel.TabIndex = 2;
            // 
            // yAxisDataLabel
            // 
            this.yAxisDataLabel.BackColor = System.Drawing.Color.Transparent;
            this.yAxisDataLabel.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.yAxisDataLabel.Location = new System.Drawing.Point(232, 3);
            this.yAxisDataLabel.Name = "yAxisDataLabel";
            this.yAxisDataLabel.Size = new System.Drawing.Size(137, 26);
            this.yAxisDataLabel.TabIndex = 5;
            layoutState1.Location = new System.Drawing.Point(3, 0);
            layoutState1.Size = new System.Drawing.Size(168, 31);
            layoutState1.Visible = true;
            this.yAxisDataLabel.Tag = layoutState1;
            this.yAxisDataLabel.Text = "Y-Axis Data";
            // 
            // yAxisDropdown
            // 
            this.yAxisDropdown.AllowMultipleSelection = false;
            this.yAxisDropdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(12)))));
            this.yAxisDropdown.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.yAxisDropdown.BorderSize = 2;
            this.yAxisDropdown.CanBeep = false;
            this.yAxisDropdown.CanShake = true;
            this.yAxisDropdown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.yAxisDropdown.DataSource = null;
            this.yAxisDropdown.DisplayMember = null;
            this.yAxisDropdown.DropdownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(12)))));
            this.yAxisDropdown.DropdownWidth = 0;
            this.yAxisDropdown.DropShadowEnabled = false;
            this.yAxisDropdown.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.yAxisDropdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.yAxisDropdown.HoveredItemBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.yAxisDropdown.HoveredItemTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(12)))));
            this.yAxisDropdown.IsReadonly = false;
            this.yAxisDropdown.ItemHeight = 30;
            this.yAxisDropdown.Items.AddRange(new string[] {
            "Tickets Sold",
            "Revenue"});
            this.yAxisDropdown.Location = new System.Drawing.Point(232, 29);
            this.yAxisDropdown.MaxDropDownItems = 8;
            this.yAxisDropdown.Name = "yAxisDropdown";
            this.yAxisDropdown.PlaceholderColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.yAxisDropdown.PlaceholderDisappearsOnFocus = false;
            this.yAxisDropdown.PlaceholderText = "- Select Data -";
            this.yAxisDropdown.SelectedIndex = 0;
            this.yAxisDropdown.SelectedItem = "Tickets Sold";
            this.yAxisDropdown.SelectedItemBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(12)))));
            this.yAxisDropdown.SelectedItemTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.yAxisDropdown.SelectedValue = null;
            this.yAxisDropdown.Size = new System.Drawing.Size(200, 40);
            this.yAxisDropdown.TabIndex = 6;
            layoutState2.Location = new System.Drawing.Point(177, 3);
            layoutState2.Size = new System.Drawing.Size(220, 40);
            layoutState2.Visible = true;
            this.yAxisDropdown.Tag = layoutState2;
            this.yAxisDropdown.Text = "Select Data";
            this.yAxisDropdown.UnselectedItemTextColor = System.Drawing.Color.Black;
            this.yAxisDropdown.ValueMember = null;
            this.yAxisDropdown.SelectedIndexChanged += new System.EventHandler(this.salesReportDropown_SelectedIndexChanged);
            // 
            // siticoneFlatPanel1
            // 
            this.siticoneFlatPanel1.BackColor = System.Drawing.Color.Transparent;
            this.siticoneFlatPanel1.Location = new System.Drawing.Point(206, 3);
            this.siticoneFlatPanel1.MaximumSize = new System.Drawing.Size(20, 66);
            this.siticoneFlatPanel1.MinimumSize = new System.Drawing.Size(20, 66);
            this.siticoneFlatPanel1.Name = "siticoneFlatPanel1";
            this.siticoneFlatPanel1.Size = new System.Drawing.Size(20, 66);
            this.siticoneFlatPanel1.TabIndex = 4;
            // 
            // salesViewType
            // 
            this.salesViewType.BackColor = System.Drawing.Color.Transparent;
            this.salesViewType.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.salesViewType.Location = new System.Drawing.Point(3, 0);
            this.salesViewType.Name = "salesViewType";
            this.salesViewType.Size = new System.Drawing.Size(137, 26);
            this.salesViewType.TabIndex = 1;
            layoutState3.Location = new System.Drawing.Point(3, 0);
            layoutState3.Size = new System.Drawing.Size(168, 31);
            layoutState3.Visible = true;
            this.salesViewType.Tag = layoutState3;
            this.salesViewType.Text = "Sales Report";
            // 
            // salesReportDropown
            // 
            this.salesReportDropown.AllowMultipleSelection = false;
            this.salesReportDropown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(12)))));
            this.salesReportDropown.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.salesReportDropown.BorderSize = 2;
            this.salesReportDropown.CanBeep = false;
            this.salesReportDropown.CanShake = true;
            this.salesReportDropown.Cursor = System.Windows.Forms.Cursors.Hand;
            this.salesReportDropown.DataSource = null;
            this.salesReportDropown.DisplayMember = null;
            this.salesReportDropown.DropdownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(12)))));
            this.salesReportDropown.DropdownWidth = 0;
            this.salesReportDropown.DropShadowEnabled = false;
            this.salesReportDropown.Font = new System.Drawing.Font("Segoe UI", 12F);
            this.salesReportDropown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.salesReportDropown.HoveredItemBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.salesReportDropown.HoveredItemTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(12)))));
            this.salesReportDropown.IsReadonly = false;
            this.salesReportDropown.ItemHeight = 30;
            this.salesReportDropown.Items.AddRange(new string[] {
            "Daily",
            "Weekly",
            "Monthly",
            "Yearly"});
            this.salesReportDropown.Location = new System.Drawing.Point(0, 29);
            this.salesReportDropown.MaxDropDownItems = 8;
            this.salesReportDropown.Name = "salesReportDropown";
            this.salesReportDropown.PlaceholderColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.salesReportDropown.PlaceholderDisappearsOnFocus = false;
            this.salesReportDropown.PlaceholderText = "- Select View -";
            this.salesReportDropown.SelectedIndex = -1;
            this.salesReportDropown.SelectedItem = null;
            this.salesReportDropown.SelectedItemBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(219)))), ((int)(((byte)(12)))));
            this.salesReportDropown.SelectedItemTextColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.salesReportDropown.SelectedValue = null;
            this.salesReportDropown.Size = new System.Drawing.Size(200, 40);
            this.salesReportDropown.TabIndex = 2;
            layoutState4.Location = new System.Drawing.Point(177, 3);
            layoutState4.Size = new System.Drawing.Size(220, 40);
            layoutState4.Visible = true;
            this.salesReportDropown.Tag = layoutState4;
            this.salesReportDropown.Text = "Select View";
            this.salesReportDropown.UnselectedItemTextColor = System.Drawing.Color.Black;
            this.salesReportDropown.ValueMember = null;
            this.salesReportDropown.SelectedIndexChanged += new System.EventHandler(this.salesReportDropown_SelectedIndexChanged);
            // 
            // siticoneHumanizerDateTime1
            // 
            this.siticoneHumanizerDateTime1.AdaptivePrecision = true;
            this.siticoneHumanizerDateTime1.CalculationMode = SiticoneNetFrameworkUI.SiticoneHumanizerDateTime.TimeCalculationMode.Calendar;
            this.siticoneHumanizerDateTime1.Culture = new System.Globalization.CultureInfo("en-PH");
            this.siticoneHumanizerDateTime1.CustomFormat = "";
            this.siticoneHumanizerDateTime1.Date = new System.DateTime(2025, 4, 22, 4, 12, 45, 206);
            this.siticoneHumanizerDateTime1.IncludeMilliseconds = false;
            this.siticoneHumanizerDateTime1.MaxPrecision = 4;
            this.siticoneHumanizerDateTime1.PreferredKind = System.DateTimeKind.Local;
            this.siticoneHumanizerDateTime1.TimeFormat = SiticoneNetFrameworkUI.SiticoneHumanizerDateTime.TimeSpanFormat.Standard;
            this.siticoneHumanizerDateTime1.UseAbbreviations = false;
            this.siticoneHumanizerDateTime1.UseRelativeDays = true;
            this.siticoneHumanizerDateTime1.UseSeasonalContext = false;
            // 
            // contentPanel
            // 
            this.contentPanel.BackColor = System.Drawing.Color.Transparent;
            this.contentPanel.Controls.Add(this.resultsPanel);
            this.contentPanel.Controls.Add(this.titlePanel);
            this.contentPanel.Controls.Add(this.chartPanel);
            this.contentPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentPanel.Location = new System.Drawing.Point(0, 105);
            this.contentPanel.MinimumSize = new System.Drawing.Size(20, 20);
            this.contentPanel.Name = "contentPanel";
            this.contentPanel.Size = new System.Drawing.Size(1064, 554);
            this.contentPanel.TabIndex = 3;
            // 
            // resultsPanel
            // 
            this.resultsPanel.BackColor = System.Drawing.Color.Transparent;
            this.resultsPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.resultsPanel.Location = new System.Drawing.Point(0, 56);
            this.resultsPanel.MinimumSize = new System.Drawing.Size(20, 20);
            this.resultsPanel.Name = "resultsPanel";
            this.resultsPanel.Size = new System.Drawing.Size(308, 498);
            this.resultsPanel.TabIndex = 2;
            // 
            // titlePanel
            // 
            this.titlePanel.BackColor = System.Drawing.Color.Transparent;
            this.titlePanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlePanel.Location = new System.Drawing.Point(0, 0);
            this.titlePanel.MinimumSize = new System.Drawing.Size(20, 20);
            this.titlePanel.Name = "titlePanel";
            this.titlePanel.Size = new System.Drawing.Size(308, 56);
            this.titlePanel.TabIndex = 1;
            // 
            // chartPanel
            // 
            this.chartPanel.BackColor = System.Drawing.Color.Transparent;
            this.chartPanel.Dock = System.Windows.Forms.DockStyle.Right;
            this.chartPanel.Location = new System.Drawing.Point(308, 0);
            this.chartPanel.MinimumSize = new System.Drawing.Size(20, 20);
            this.chartPanel.Name = "chartPanel";
            this.chartPanel.Size = new System.Drawing.Size(756, 554);
            this.chartPanel.TabIndex = 0;
            // 
            // SalesReportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1064, 659);
            this.Controls.Add(this.contentPanel);
            this.Controls.Add(this.filterPanel);
            this.MinimumSize = new System.Drawing.Size(1080, 675);
            this.Name = "SalesReportForm";
            this.StateCommon.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.StateCommon.Border.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.Load += new System.EventHandler(this.SalesReportForm_Load);
            this.filterPanel.ResumeLayout(false);
            this.contentPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SiticoneNetFrameworkUI.SiticoneFlatPanel filterPanel;
        private SiticoneNetFrameworkUI.SiticoneFlatPanel siticoneFlatPanel1;
        private SiticoneNetFrameworkUI.SiticoneLabel salesViewType;
        private SiticoneNetFrameworkUI.SiticoneDropdown salesReportDropown;
        private SiticoneNetFrameworkUI.SiticoneHumanizerDateTime siticoneHumanizerDateTime1;
        private SiticoneNetFrameworkUI.SiticoneFlatPanel contentPanel;
        private SiticoneNetFrameworkUI.SiticoneFlatPanel titlePanel;
        private SiticoneNetFrameworkUI.SiticoneFlatPanel chartPanel;
        private SiticoneNetFrameworkUI.SiticoneFlatPanel resultsPanel;
        private SiticoneNetFrameworkUI.SiticoneLabel yAxisDataLabel;
        private SiticoneNetFrameworkUI.SiticoneDropdown yAxisDropdown;
    }
}