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
        Form form;
        public SettingsForm(Form forn)
        {
            form = forn;
            InitializeComponent();
        }



        private void BackButton_Click(object sender, EventArgs e)
        {
            if (form is Form1)
            {
                Button StartButton = Form1.UI.StartButton;
                if (!Form1.UI.TerrainBox.Controls.Contains(StartButton))//work
                {
                    RedrawTerrain();
                    Form1.UI.TerrainBox.Invalidate();
                }
            } else if (form is MeshForm)
            {
                for (int i = 0; i < 50; i++)
                {
                    DrawMesh.Colours[i] = TerrainCmap.Interpolate_value(i / 50f);
                }
                form.Invalidate();
            }
            switch (FileTypeListBox.SelectedItem)
            {
                case ".OBJ":
                    InterfaceHandler.FileType = InterfaceHandler.SaveFileType.OBJ;
                    break;
                case ".PLY":
                    InterfaceHandler.FileType = InterfaceHandler.SaveFileType.PLY;
                    break;
            }
            
            this.Close();
            form.BringToFront();
        }

        public static void RedrawTerrain()
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
            CurrentFileLabel.Text = InterfaceHandler.FileType.ToString();
            FileNameTextBox.Text = OBJExport.FileName; 
        }

        private void ColourBoxClick(object sender, EventArgs e)
        {
            colorDialog1.ShowDialog();
            Color c = colorDialog1.Color;
            string name = ((PictureBox)sender).Name;
            int index = int.Parse(name.Substring(name.Length - 1));
            ChangeColour(index, c);            
        }

        private void ChangeColour(int index, Color C)
        {
            PictureBox ColourBox = (PictureBox)Controls.Find($"ColourBox{index}", true).FirstOrDefault();
            ColourBox.BackColor = C;
            TerrainCmap.cmapC[index - 1] = C;
        }

        private void GenericPresetPress(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            int index = int.Parse(btn.Name.Substring(btn.Name.Length - 1)) - 1;//index of cmap
            for (int i = 0; i < 5; i++)
            {
                //input(index of button, colour(index of cmap, colour index)
                ChangeColour(i + 1, TerrainCmap.ColourSchemes[index, i]);
            }
        }

        private void FileTypeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentFileLabel.Text = FileTypeListBox.SelectedItem.ToString().Substring(1);
        }
    }
}
