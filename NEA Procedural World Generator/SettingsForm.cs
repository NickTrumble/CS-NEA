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
        //private variables
        private Form form;//reference to main form

        //constructor
        public SettingsForm(Form forn)
        {
            form = forn;
            InitializeComponent();
        }

        //takes user back to main form and updates the terrain for new settings
        private void BackButton_Click(object sender, EventArgs e)
        {
            if (form is Form1)
            {
                Button StartButton = Form1.UI.StartButton;
                //if no initial world generated, then dont redraw it all
                if (!Form1.UI.TerrainBox.Controls.Contains(StartButton))
                {
                    //redraw all bitmaps if world already drawn
                    RedrawTerrain();
                    Form1.UI.TerrainBox.Invalidate();
                }
            } else if (form is MeshForm)
            {
                //preload 50 colours of the new colour scheme to draw
                for (int i = 0; i < 50; i++)
                {
                    //updates the colours of drawmesh if on mesh
                    DrawMesh.Colours[i] = TerrainCmap.Interpolate_value(i / 50f);
                }
                form.Invalidate();//redraw terrain
            }
            switch (FileTypeListBox.SelectedItem)
            {
                //updates file save type if changed
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

        //redraw bitmaps
        public static void RedrawTerrain()
        {
            //iterate over all chunks in the world
            foreach (Chunk c in Form1.world.WorldChunks.Values)
            {
                //redraw chunk bitmap using new colour scheme
                for (int i = 0; i < World.chunkSize; i++)
                {
                    for (int j = 0; j < World.chunkSize; j++)
                    {
                        //sets each blocks new colour
                        c.Bmp.SetPixel(i, j, World.BlockColourTransformer(c.ChunkBlock[(i, j)]));
                    }
                }
            }
        }

        //loads default settings into form
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

        //lets user update colours of all pictureboxes
        private void ColourBoxClick(object sender, EventArgs e)
        {
            //shows colour picker dialog
            colorDialog1.ShowDialog();
            Color c = colorDialog1.Color;
            //finds reference to the clicked picturebox
            string name = ((PictureBox)sender).Name;
            //find picturebox index out of Colour scheme indexes
            int index = int.Parse(name.Substring(name.Length - 1));
            //chanmge colour of picfturebox clicked
            ChangeColour(index, c);            
        }

        //changes the colour of the selected picturebox
        private void ChangeColour(int index, Color C)
        {
            //gets the pictureboxc using the index of the picturebox
            PictureBox ColourBox = (PictureBox)Controls.Find($"ColourBox{index}", true).FirstOrDefault();
            //changes the bakc colour
            ColourBox.BackColor = C;
            //changes the colour map
            TerrainCmap.cmapC[index - 1] = C;
        }

        //changes colour scheme pictureboxes when presets clicked
        private void GenericPresetPress(object sender, EventArgs e)
        {
            //gets button pressed
            Button btn = (Button)sender;
            //index of colour map
            int index = int.Parse(btn.Name.Substring(btn.Name.Length - 1)) - 1;
            //changes the colour of each of the pictureboxes for the new coloru scheme
            for (int i = 0; i < 5; i++)
            {
                //input(index of button, colour(index of cmap, colour index)
                ChangeColour(i + 1, TerrainCmap.ColourSchemes[index, i]);
            }
        }

        //updates label of save file extension when changed in drop down
        private void FileTypeListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentFileLabel.Text = FileTypeListBox.SelectedItem.ToString().Substring(1);
        }
    }
}
