namespace WildCat_Tickets
{
    partial class TabForm
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
            this.tabPalette = new Krypton.Toolkit.KryptonCustomPaletteBase(this.components);
            this.SuspendLayout();
            // 
            // tabPalette
            // 
            this.tabPalette.BaseFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tabPalette.FormStyles.FormMain.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.tabPalette.FormStyles.FormMain.StateCommon.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.tabPalette.FormStyles.FormMain.StateCommon.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.tabPalette.FormStyles.FormMain.StateCommon.Border.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.tabPalette.HeaderStyles.HeaderForm.StateCommon.Back.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.tabPalette.HeaderStyles.HeaderForm.StateCommon.Back.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.tabPalette.UseThemeFormChromeBorderWidth = Krypton.Toolkit.InheritBool.True;
            // 
            // TabForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.AutoScroll = true;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.ClientSize = new System.Drawing.Size(1010, 605);
            this.ForeColor = System.Drawing.Color.White;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.LocalCustomPalette = this.tabPalette;
            this.Name = "TabForm";
            this.PaletteMode = Krypton.Toolkit.PaletteMode.Custom;
            this.StateCommon.Border.Color1 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.StateCommon.Border.Color2 = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.ResumeLayout(false);

        }

        #endregion

        private Krypton.Toolkit.KryptonCustomPaletteBase tabPalette;
    }
}