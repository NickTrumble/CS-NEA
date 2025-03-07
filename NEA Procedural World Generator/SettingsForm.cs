using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace NEA_Procedural_World_Generator
{
    public partial class SettingsForm : Form
    {
        Form1 form;
        public SettingsForm(Form1 forn)
        {
            form = forn;
            InitializeComponent();
            UpdateColourBox();
        }

        private void UpdateColourBox()
        {
            foreach (Control c in Controls)
            {
                if (c is PictureBox)
                {
                    c.Click += ColourBoxClick;
                }
            }
        }

        private void BackButton_Click(object sender, EventArgs e)
        {
            Button StartButton = Form1.UI.StartButton;
            if (!Form1.UI.TerrainBox.Controls.Contains(StartButton))//work
            {
                RedrawTerrain();
                Form1.UI.TerrainBox.Invalidate();
            }
            this.Close();
            form.BringToFront();
        }

        private void RedrawTerrain()
        {
            foreach (Chunk c in Form1.world.WorldChunks.Values)
            {
                for (int i = 0; i < World.chunkSize; i++)
                {
                    for (int j = 0; j < World.chunkSize; j++)
                    {
                        c.Bmp.SetPixel(i, j, World.BlockColourTransformer(c.ChunkBlock[(i, j)]));
                    }
                }
            }
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            ColourBox1.BackColor = TerrainCmap.cmapC[0];
            ColourBox2.BackColor = TerrainCmap.cmapC[1];
            ColourBox3.BackColor = TerrainCmap.cmapC[2];
            ColourBox4.BackColor = TerrainCmap.cmapC[3];
            ColourBox5.BackColor = TerrainCmap.cmapC[4];
        }

        private void ColourBoxClick(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            Color c = colorDialog1.Color;
            string name = ((PictureBox)sender).Name;
            int index = int.Parse(name.Substring(name.Length - 1)) - 1;
            ChangeColour(index, c);            
        }

        private void ChangeColour(int index, Color C)
        {
            PictureBox ColourBox = (PictureBox)Controls.Find($"ColourBox{index + 1}", true).FirstOrDefault();
            ColourBox.BackColor = C;
            TerrainCmap.cmapC[index] = C;
        }

        private void LavaButton_Click(object sender, EventArgs e)
        {
            Color[] ColourScheme = new Color[5]
            {
                Color.FromArgb(240, 220, 0),
                Color.FromArgb(255, 128, 64),
                Color.FromArgb(255, 0, 0),
                Color.FromArgb(100, 50, 50),
                Color.FromArgb(0, 0, 0)
            };
                
                
            for (int i = 0; i < 5; i++)
            {
                ChangeColour(i, ColourScheme[i]);
            }
        }
    }
}
