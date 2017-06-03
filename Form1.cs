////////////////////////////////////////////////////////////////////////////////
//
// ACO converter for Paint.NET
//
// This software is provided under the MIT License:
//   Copyright (C) 2012-2017 Nicholas Hayes
// 
// See LICENSE.txt for complete licensing and attribution information.
//
/////////////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.IO;
using System.Windows.Forms;
using PaintDotNet;
using SwatchConverter.Properties;

namespace SwatchConverter
{
    internal partial class Form1 : Form
    {
        private ColorSwatchDecoder swatchDecoder;
        private string swatchFileName;
        private string commandLineFileName;

        public Form1(string[] args)
        {
            InitializeComponent();
            this.swatchDecoder = null;
            this.swatchFileName = string.Empty;
            this.commandLineFileName = null;
            this.toolStripStatusLabel1.Text = string.Empty;
            this.statusPaddingLabel.Text = string.Empty;
            this.colorLabel.Text = string.Empty;
            this.nameLabel.Text = string.Empty;

            if (args.Length > 0)
            {
                string path = args[0];
                if (path.EndsWith(".aco", StringComparison.OrdinalIgnoreCase))
                {
                    this.commandLineFileName = path;
                }
            }
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            if (!string.IsNullOrEmpty(this.commandLineFileName))
            {
                OpenSwatchFile(this.commandLineFileName);
            }
        }

        private void swatchControl1_ColorClicked(object sender, IndexEventArgs e)
        {
            ColorBgra color = this.swatchControl1.Colors[e.Index];

            this.nameLabel.Text = this.swatchDecoder.Colors[e.Index].Name ?? string.Empty;

            this.colorLabel.Text = string.Format(CultureInfo.CurrentCulture, Resources.RGBFormat, color.R, color.G, color.B);
        }

        private void OpenSwatchFile(string file)
        {
            try
            {
                this.swatchControl1.Colors = EmptyArray<ColorBgra>.Value;
                this.toolStripStatusLabel1.Text = string.Empty;

                this.swatchDecoder = new ColorSwatchDecoder(file);

                this.swatchControl1.Colors = this.swatchDecoder.Colors.GetSwatchColors();

                this.toolStripStatusLabel1.Text = string.Format(
                    CultureInfo.CurrentCulture,
                    Resources.StatusBarLoadFormat,
                    Path.GetFileName(file),
                    this.swatchControl1.Colors.Length);

                this.swatchFileName = Path.GetFileNameWithoutExtension(file);
                this.saveToolStripMenuItem.Enabled = true;
            }
            catch (DirectoryNotFoundException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (FormatException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (FileNotFoundException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (IOException ex)
            {
                ShowErrorMessage(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                ShowErrorMessage(ex.Message);
            }
        }

        private void ShowErrorMessage(string message)
        {
            MessageBox.Show(this, message, this.Text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, 0);
        }

        private void SavePalette(string path, ColorBgra[] swatches, int index)
        {
            FileStream stream = null;

            try
            {
                stream = new FileStream(path, FileMode.Create, FileAccess.Write);
                using (StreamWriter sw = new StreamWriter(stream, System.Text.Encoding.UTF8))
                {
                    stream = null;

                    sw.WriteLine("; Adobe® Photoshop® Color Swatch file converted to Paint.NET by {0}.", this.Text);
                    int length = swatches.Length - index;

                    if (length > 96)
                    {
                        length = 96;
                    }

                    while (length > 0)
                    {
                        sw.WriteLine(swatches[index].Bgra.ToString("X8", CultureInfo.InvariantCulture));

                        index++;
                        length--;
                    }
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                    stream = null;
                }
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.openFileDialog1.ShowDialog(this) == DialogResult.OK)
            {
                OpenSwatchFile(this.openFileDialog1.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorBgra[] swatches = this.swatchControl1.Colors;

            if (swatches.Length > 0)
            {
                // Default to the file name of the current swatch.
                this.saveFileDialog1.FileName = this.swatchFileName;

                if (this.saveFileDialog1.ShowDialog(this) == DialogResult.OK)
                {
                    string path = this.saveFileDialog1.FileName;

                    SavePalette(path, swatches, 0);

                    // Because Paint.NET uses a 96 color palette swatch files containing more than 96 colors must be split into multiple files.
                    if (swatches.Length > 96)
                    {
                        string dir = Path.GetDirectoryName(path);
                        string baseName = Path.GetFileNameWithoutExtension(path);

                        int fileNumber = 2;
                        int colorsRemaining = swatches.Length - 96;
                        int index = 96;

                        while (colorsRemaining > 0)
                        {
                            string file = Path.Combine(dir, string.Format(CultureInfo.InvariantCulture, "{0}#{1}.txt", baseName, fileNumber));

                            SavePalette(file, swatches, index);

                            fileNumber++;
                            index += 96;
                            colorsRemaining -= 96;
                        }
                    }

                    this.swatchControl1.Colors = EmptyArray<ColorBgra>.Value;
                    this.toolStripStatusLabel1.Text = string.Empty;
                    this.colorLabel.Text = string.Empty;
                    this.saveToolStripMenuItem.Enabled = false;
                } 
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void statusDonateLink_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://forums.getpaint.net/index.php?showtopic=25743");
        }
    }
}
