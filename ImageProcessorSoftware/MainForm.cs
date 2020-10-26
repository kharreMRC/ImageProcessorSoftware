using System;
using System.Drawing;
using System.Windows.Forms;

namespace ImageProcessorSoftware
{
    public partial class MainForm : Form
    {
        private Bitmap image;

        public MainForm()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "BMP|*.bmp|GIF|*.gif|JPEG|*.jpeg|PNG|*.png";
                openFileDialog.InitialDirectory = "C:\\";
                openFileDialog.Multiselect = false;

                if (openFileDialog.ShowDialog().Equals(DialogResult.OK))
                {
                    try
                    {
                        image = new Bitmap(openFileDialog.FileName);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"{ex}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    mainPictureBox.Image = image;
                    saveToolStripMenuItem.Enabled = true;
                    exportToASCIIToolStripMenuItem.Enabled = true;
                    editToolStripMenuItem.Enabled = true;
                    filterToolStripMenuItem.Enabled = true;
                }
            }
        }

        private void exportToASCIIToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "TXT|*.txt";
                saveFileDialog.InitialDirectory = "C:\\";

                if (saveFileDialog.ShowDialog().Equals(DialogResult.OK))
                {
                    ExportToASCII(saveFileDialog.FileName);
                    MessageBox.Show($"The file \"{saveFileDialog.FileName}\" has been exported to ASCII with success.", "Export to ASCII", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.Filter = "BMP|*.bmp|GIF|*.gif|JPEG|*.jpeg|PNG|*.png";
                saveFileDialog.InitialDirectory = "C:\\";

                if (saveFileDialog.ShowDialog().Equals(DialogResult.OK))
                {
                    mainPictureBox.Image.Save(saveFileDialog.FileName);
                    MessageBox.Show($"The file \"{saveFileDialog.FileName}\" has been saved with success.", "Save", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainPictureBox.Image = image;
        }

        private void inverseColorsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainPictureBox.Image = InverseColors((Bitmap)mainPictureBox.Image, 0, 0, mainPictureBox.Image.Width, mainPictureBox.Image.Height);
        }

        private void oldStyledToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainPictureBox.Image = GetOldStyledImage((Bitmap)mainPictureBox.Image, 0, 0, mainPictureBox.Image.Width, mainPictureBox.Image.Height);
        }

        private void shadesOfGrayToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainPictureBox.Image = GetGrayShades((Bitmap)mainPictureBox.Image, 0, 0, mainPictureBox.Image.Width, mainPictureBox.Image.Height);
        }

        private void sepiaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mainPictureBox.Image = GetSepiaImage((Bitmap)mainPictureBox.Image, 0, 0, mainPictureBox.Image.Width, mainPictureBox.Image.Height);
        }

        private void ExportToASCII(string fileName)
        {
            byte scaleX = 8;
            byte scaleY = scaleX;

            char[,] ascii = new char[mainPictureBox.Image.Width, mainPictureBox.Image.Height];
            Bitmap lowRes = new Bitmap(mainPictureBox.Image.Width / scaleX, mainPictureBox.Image.Height / scaleY);

            for (var x = 0; x < mainPictureBox.Image.Width - scaleX; x += scaleX)
            {
                for (var y = 0; y < mainPictureBox.Image.Height - scaleY; y += scaleY)
                {
                    Color currentPixelColor = ((Bitmap)mainPictureBox.Image).GetPixel(x, y);
                    lowRes.SetPixel(x / scaleX, y / scaleY, currentPixelColor);
                }
            }

            for (var x = 0; x < lowRes.Width; x++)
            {
                for (var y = 0; y < lowRes.Height; y++)
                {
                    Color currentPixelColor = lowRes.GetPixel(x, y);
                    var shade = (byte)((currentPixelColor.R + currentPixelColor.G + currentPixelColor.B) / 3);
                    if ((shade <= 255) && (shade > 192))
                    {
                        ascii[x, y] = '.';
                    }
                    else if ((shade <= 192) && (shade > 128))
                    {
                        ascii[x, y] = 'o';
                    }
                    else if ((shade <= 128) && (shade > 64))
                    {
                        ascii[x, y] = '0';
                    }
                    else if ((shade <= 64) && (shade >= 0))
                    {
                        ascii[x, y] = '@';
                    }
                }
            }

            string[] lines = new string[image.Height];

            for (int x = 0; x < lowRes.Height; x++)
            {
                string line = "";
                for (int y = 0; y < lowRes.Width; y++)
                {
                    line += ascii[y, x] + "  ";
                }
                lines[x] = line;
            }

            System.IO.File.WriteAllLines($"{fileName}", lines);
        }

        private Bitmap GetGrayShades(Bitmap source, int sourceX, int sourceY, int sourceWidth, int sourceHeight)
        {
            Bitmap destination = new Bitmap(sourceWidth, sourceHeight);
            for (var i = 0; i < destination.Width; i++)
            {
                for (var j = 0; j < destination.Height; j++)
                {
                    Color currentPixelColor = source.GetPixel(i + sourceX, j + sourceY);
                    byte shade = (byte)((currentPixelColor.R + currentPixelColor.G + currentPixelColor.B) / 3);
                    destination.SetPixel(i, j, Color.FromArgb(shade, shade, shade));
                }
            }
            return destination;
        }

        private Bitmap GetOldStyledImage(Bitmap source, int sourceX, int sourceY, int sourceWidth, int sourceHeight)
        {
            Bitmap destination = new Bitmap(sourceWidth, sourceHeight);
            for (var i = 0; i < source.Width; i += 2)
            {
                for (var j = 0; j < source.Height; j += 2)
                {
                    Color currentPixelColor = source.GetPixel(i + sourceX, j + sourceY);
                    destination.SetPixel(i, j, currentPixelColor);
                }
            }
            return destination;
        }

        public static Bitmap GetSepiaImage(Bitmap source, int sourceX, int sourceY, int sourceWidth, int sourceHeight)
        {
            Bitmap destination = new Bitmap(sourceWidth, sourceHeight);
            for (var i = 0; i < destination.Width; i++)
            {
                for (var j = 0; j < destination.Height; j++)
                {
                    Color currentPixelColor = source.GetPixel(i + sourceX, j + sourceY);
                    byte shade = (byte)((currentPixelColor.R + currentPixelColor.G + currentPixelColor.B) / 3);
                    if (!(shade > 255 - 48))
                    {
                        destination.SetPixel(i, j, Color.FromArgb(shade + 48, shade + 32, shade));
                    }
                    else
                    {
                        destination.SetPixel(i, j, Color.FromArgb(shade + (255 - shade), shade + (255 - shade), shade));
                    }
                }
            }
            return destination;
        }

        private Bitmap InverseColors(Bitmap source, int sourceX, int sourceY, int sourceWidth, int sourceHeight)
        {
            Bitmap destination = new Bitmap(source.Width, source.Height);
            for (var i = 0; i < source.Width; i++)
            {
                for (var j = 0; j < source.Height; j++)
                {
                    Color currentPixelColor = source.GetPixel(i + sourceX, j + sourceY);
                    Color newPixelColor = Color.FromArgb(255 - currentPixelColor.R, 255 - currentPixelColor.G, 255 - currentPixelColor.B);
                    destination.SetPixel(i, j, newPixelColor);
                }
            }
            return destination;
        }
    }
}
