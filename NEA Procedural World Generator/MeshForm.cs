using System.Drawing;
using System.Windows.Forms;
using System;

namespace NEA_Procedural_World_Generator
{
    public partial class MeshForm : Form
    {
        public PictureBox TerrainBox, MenuBox;
        public NumericUpDown RotationZNUD;
        public Button BackButton, SettingsButton, SaveButton;
        public int width = 800;
        public int height = 500;
        public Form1 form;
        
        DrawMesh MeshDrawer;
        private Point lastClicked;
        bool clicked = false;
        
        public MeshForm(DrawMesh dm, Form1 forn)
        {
            form = forn;
            MeshDrawer = dm;
            InitializeComponent();
            Initialise();
        }

        public void Initialise()
        {
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            
            Size = new Size(width, height);
            //Mesh Holder Picturebox
            TerrainBox = new PictureBox
            {
                Size = new Size(ClientSize.Width - 115, ClientSize.Height - 10),
                Location = new Point(110, 5),
                BorderStyle = BorderStyle.FixedSingle
            };
            TerrainBox.Paint += TerrainBox_Paint;
            TerrainBox.MouseDown += TerrainBoxMouseDown;
            TerrainBox.MouseUp += TerrainBoxMouseUp;
            TerrainBox.MouseMove += TerrainBoxMouseMove;
            Controls.Add(TerrainBox);
            TerrainBox.BringToFront();

            //button and slider holder picturebox
            MenuBox = new PictureBox
            {
                Location = new Point(5, 5),
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(100, ClientSize.Height - 20)
            };
            Controls.Add(MenuBox);

            //Button to main form
            BackButton = InterfaceHandler.ButtonCreator(BackButtonPress, new Point(0, MenuBox.Height - 50), "Home Form",
                new Size(100, 50), null, MenuBox);

            //settings button
            SettingsButton = InterfaceHandler.ButtonCreator(SettingsButtonClick, new Point(0, MenuBox.Height - 75), "⚙️",
                new Size(100, 25), null, MenuBox);

            //rotation label
            InterfaceHandler.LabelCreator(new Point(0, MenuBox.Height - 95), "Rotation:", MenuBox);

            //rotation numeric up and down
            RotationZNUD = InterfaceHandler.SliderCreator(new Point(60, MenuBox.Height - 95), 0f, 360f, 1, 4f, 1f, MenuBox);
            RotationZNUD.ValueChanged += RotationSliderUpdate;

            //Save Button
            SaveButton = InterfaceHandler.ButtonCreator(SaveButtonClick, new Point(0, MenuBox.Height - 120), "Save Terrain",
                new Size(100, 25), null, MenuBox);
        }

        public async void SaveButtonClick(object sender, EventArgs e)
        {
            OBJExport Exporter = new OBJExport(MeshDrawer.inWorld, 0.015f);
            FolderBrowserDialog filelocation = new FolderBrowserDialog
            {
                Description = "Save Terrain at...",
                SelectedPath = "C:\\Users\\iantr\\source\\repos\\NEA Procedural World Generator\\NEA Procedural World Generator"
            };

            if (filelocation.ShowDialog() == DialogResult.OK)
            {
                await Exporter.SaveAll(filelocation.SelectedPath);
            }
            
        }

        public void BackButtonPress(object sender, EventArgs e)
        {
            Form1 form = new Form1(Form1.world);
            form.Show();
            this.Hide();
        }

        public void SettingsButtonClick(object sender, EventArgs e)
        {
            SettingsForm Settingform = new SettingsForm(this);
            Settingform.Show();
        }

        private void TerrainBox_Paint(object sender, PaintEventArgs e)
        {
            MeshDrawer.Draw(e.Graphics, (int)RotationZNUD.Value);
        }

        public void RotationSliderUpdate(object sender, EventArgs e)
        {
            TerrainBox.Invalidate();
        }

        public void TerrainBoxMouseDown(object sender, MouseEventArgs e)
        {
            clicked = true;
            lastClicked = e.Location;
        }

        public void TerrainBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (clicked)
            {
                decimal differencez = (e.Location.X - lastClicked.X);
                RotationZNUD.Value = Math.Max(0, Math.Min((RotationZNUD.Value - differencez), 170));
                lastClicked = e.Location;
            }

            
        }

        public void TerrainBoxMouseUp(object sender, MouseEventArgs e)
        {
            clicked = false;
            int differencez = (e.Location.X - lastClicked.X) / 2;
            RotationZNUD.Value = (RotationZNUD.Value - differencez + 170) % 170;

        }
    }

}