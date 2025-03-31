namespace WildCat_Tickets
{
    partial class MoviesForm
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
            this.moviesFlowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.SuspendLayout();
            // 
            // moviesFlowLayoutPanel
            // 
            this.moviesFlowLayoutPanel.AutoScroll = true;
            this.moviesFlowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.moviesFlowLayoutPanel.Location = new System.Drawing.Point(0, 0);
            this.moviesFlowLayoutPanel.Name = "moviesFlowLayoutPanel";
            this.moviesFlowLayoutPanel.Size = new System.Drawing.Size(968, 563);
            this.moviesFlowLayoutPanel.TabIndex = 0;
            this.moviesFlowLayoutPanel.Resize += new System.EventHandler(this.moviesFlowLayoutPanel_Resize);
            // 
            // MoviesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(968, 563);
            this.Controls.Add(this.moviesFlowLayoutPanel);
            this.Name = "MoviesForm";
            this.StateCommon.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.StateCommon.Border.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.Load += new System.EventHandler(this.MoviesForm_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.FlowLayoutPanel moviesFlowLayoutPanel;
    }
}