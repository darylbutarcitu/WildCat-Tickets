using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WildCat_Tickets
{
    public partial class MoviesForm : TabForm
    {
        private string imageFolderPath = @"movies"; // Path to your images folder

        public MoviesForm()
        {
            InitializeComponent();
        }

        private void MoviesForm_Load(object sender, EventArgs e)
        {
            this.Size = new Size(1080, 675);
            LoadImagesFromFolder();
        }

        private void LoadImagesFromFolder()
        {
            moviesFlowLayoutPanel.Controls.Clear(); // Clear previous images

            if (!Directory.Exists(imageFolderPath))
            {
                MessageBox.Show("Image folder not found!");
                return;
            }

            string[] imageFiles = Directory.GetFiles(imageFolderPath, "*.*")
                .Where(f => f.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".png", StringComparison.OrdinalIgnoreCase) ||
                            f.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase))
                .ToArray();

            foreach (string imagePath in imageFiles)
            {
                AddImageToGrid(imagePath);
            }

            AdjustImageLayout(); // Adjust layout after loading images
        }

        private void AddImageToGrid(string imagePath)
        {
            PictureBox pictureBox = new PictureBox
            {
                Size = new Size(200, 300), // Fixed size images
                SizeMode = PictureBoxSizeMode.Zoom,
                Cursor = Cursors.Hand,
                Margin = new Padding(5),
                Image = Image.FromFile(imagePath),
                //BorderStyle = BorderStyle.FixedSingle // Add a thin black border for debugging
            };

            moviesFlowLayoutPanel.Controls.Add(pictureBox);
        }

        private void moviesFlowLayoutPanel_Resize(object sender, EventArgs e)
        {
            AdjustImageLayout();
        }

        private void AdjustImageLayout()
        {
            int panelWidth = moviesFlowLayoutPanel.ClientSize.Width;
            int imageWidth = 210; // 200px image + 10px margin
            int columns = panelWidth / imageWidth;

            int totalImageWidth = columns * imageWidth;
            int padding = (panelWidth - totalImageWidth) / 2;

            moviesFlowLayoutPanel.SuspendLayout();
            moviesFlowLayoutPanel.Padding = new Padding(padding, 0, padding, 0); // Center the images and ensure padding is balanced
            moviesFlowLayoutPanel.ResumeLayout();
        }
    }
}
