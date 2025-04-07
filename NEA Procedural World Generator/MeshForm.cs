using System.Drawing;
using System.Windows.Forms;
using System;
using System.Numerics;
using System.Linq;

namespace NEA_Procedural_World_Generator
{
    public partial class MeshForm : Form
    {
        #region setup
        public PictureBox TerrainBox, MenuBox;
        public NumericUpDown RotationZNUD;
        public Button BackButton, SettingsButton, SaveButton;
        public TrackBar RotationSlider;
        public CheckBox GridViewBox;
        public int width = 800;
        public int height = 500;
        public Form1 form;
        
        private DrawMesh MeshDrawer;
        private Point lastClicked;
        private bool clicked = false;
        private Label LogoLabel;
        
        public MeshForm(DrawMesh dm, Form1 forn)
        {
            form = forn;
            MeshDrawer = dm;
            InitializeComponent();
            Initialise();
        }

        public void Initialise()
        {
            this.Text = "Procedural Terrain Generator - Mesh View";
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
            BackButton = InterfaceHandler.ButtonCreator(BackButtonPress, new Point(0, MenuBox.Height - 100), "Back To Home Form",
                new Size(100, 100), null, MenuBox);

            //Save Button
            SaveButton = InterfaceHandler.ButtonCreator(SaveButtonClick, new Point(0, MenuBox.Height - 150), "Save Terrain",
                new Size(100, 50), null, MenuBox);

            //settings button
            SettingsButton = InterfaceHandler.ButtonCreator(SettingsButtonClick, new Point(0, MenuBox.Height - 200), "⚙️",
                new Size(100, 50), new Font("Arial", 20), MenuBox);

            //rotation slider
            RotationSlider = new TrackBar
            {
                Maximum = 175,
                Minimum = 0,
                Value = 45,
                Location = new Point(0, MenuBox.Height - 235)
            };
            MenuBox.Controls.Add(RotationSlider);
            RotationSlider.ValueChanged += RotationSliderUpdate;

            //rotation label
            InterfaceHandler.LabelCreator(new Point(0, MenuBox.Height - 255), "Rotation:", MenuBox);

            //rotation numeric up and down
            RotationZNUD = InterfaceHandler.SliderCreator(new Point(60, MenuBox.Height - 260), 0f, 175f, 1, 4f, 1f, MenuBox);
            RotationZNUD.ValueChanged += RotationNUDUpdate;

            //grid view check boxs
            GridViewBox = new CheckBox
            {
                Location = new Point(5, MenuBox.Height - 285),
                Text = "Grid View"
            };
            MenuBox.Controls.Add(GridViewBox);
            GridViewBox.Click += (sender, e) => { TerrainBox.Invalidate(); };

            //min/max labels
            Label maxLabel = new Label
            {
                Location = new Point(0, MenuBox.Height - 300),
                Text = $"Max. Height: {MeshDrawer.heightmap.Cast<float>().Max().ToString().Substring(0, 4)}",
                Width = 200
            };
            MenuBox.Controls.Add(maxLabel);
            Label minLabel = new Label
            {
                Location = new Point(0, MenuBox.Height - 315),
                Text = $"Max. Height: {MeshDrawer.heightmap.Cast<float>().Min().ToString().Substring(0, 4)}",
                Width = 200
            };
            MenuBox.Controls.Add(minLabel);

            //Top Left title
            LogoLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(MenuBox.Width, 100),
                Text = "Procedural Terrain Generator: Mesh Form",
                Font = new Font("Arial", 13),
                TextAlign = ContentAlignment.TopCenter,
            };
            MenuBox.Controls.Add(LogoLabel);
        }

        #endregion
        #region button clicks

        //Allows the user to save the terrain from the mesh form
        public async void SaveButtonClick(object sender, EventArgs e)
        {
            //new instance of OBJExporter
            OBJExport Exporter = new OBJExport(MeshDrawer.inWorld, 130f);
            //gets the directory path
            FolderBrowserDialog filelocation = new FolderBrowserDialog
            {
                Description = "Save Terrain at...",
                SelectedPath = "C:\\Users\\iantr\\source\\repos\\NEA Procedural World Generator\\NEA Procedural World Generator"
            };

            if (filelocation.ShowDialog() == DialogResult.OK)//once the user selects a path...
            {
                //saves entire terrain instance was initialised with, which is the mesh terrain
                await Exporter.SaveAll(filelocation.SelectedPath);
            }
        }

        //takes user back to home form
        public void BackButtonPress(object sender, EventArgs e)
        {
            //loads the home form with the world given to mesh form to restore world
            Form1 form = new Form1(Form1.world);
            form.Show();//show main form
            Form1.UI.StartButtonClick(sender, e);//skip the load world screen
            this.Hide();//hide this form
        }

        //show settings form
        public void SettingsButtonClick(object sender, EventArgs e)
        {
            SettingsForm Settingform = new SettingsForm(this);
            Settingform.Show();
        }

        #endregion
        #region rotation
        //redraws terrain after rotation numeric updown angle is changed
        public void RotationNUDUpdate(object sender, EventArgs e)
        {
            RotationSlider.Value = (int)RotationZNUD.Value;
            TerrainBox.Invalidate();
        }
        //redraws terrain after rotation slider angle is changed
        public void RotationSliderUpdate(object sender, EventArgs e)
        {
            RotationZNUD.Value = RotationSlider.Value;
            TerrainBox.Invalidate();
        }

        #endregion
        #region terrain box

        //main picturebox paint event
        private void TerrainBox_Paint(object sender, PaintEventArgs e)
        {
            //if grid view box checked, draw grid view underneath
            if (GridViewBox.Checked)
            {
                //create rotation matrix
                Matrix3x2 rMatrix = Matrix3x2.CreateRotation((float)((int)RotationZNUD.Value * Math.PI / 180));

                //calculates step between each square to show 20 squares
                float stepx = MeshDrawer.heightmap.GetLength(0) / 20f;
                float stepy = MeshDrawer.heightmap.GetLength(1) / 20f;
                for (int i = 0; i <= 20; i++)
                {
                    //calculates points of either end of the grid
                    PointF p1 = MeshDrawer.PointCalc(new Vector3(i * stepx, 0, 0), rMatrix);
                    PointF p2 = MeshDrawer.PointCalc(new Vector3(i * stepx, 0, 20 * stepy), rMatrix);
                    PointF p3 = MeshDrawer.PointCalc(new Vector3(0, 0, i * stepy), rMatrix);
                    PointF p4 = MeshDrawer.PointCalc(new Vector3(20 * stepx, 0, i * stepy), rMatrix);
                    //draws grid
                    e.Graphics.DrawLine(Pens.Black, p1, p2);
                    e.Graphics.DrawLine(Pens.Black, p3, p4);
                }
            }
            //redraws the terrain
            MeshDrawer.Draw(e.Graphics, (int)RotationZNUD.Value);
        }

        //tells the program to move the mesh when mouse is moved
        public void TerrainBoxMouseDown(object sender, MouseEventArgs e)
        {
            clicked = true;
            lastClicked = e.Location;
        }

        public void TerrainBoxMouseMove(object sender, MouseEventArgs e)
        {
            //if mouse is being clicked
            if (clicked)
            {
                //finds the difference in pixels between last mouse pos and now
                decimal differencez = (e.Location.X - lastClicked.X);
                //update the up and down values
                RotationZNUD.Value = Math.Max(0, Math.Min((RotationZNUD.Value - differencez), 175));
                //update the last clicked location 
                lastClicked = e.Location;
            }           
        }

        //stops the program from moving the mesh
        public void TerrainBoxMouseUp(object sender, MouseEventArgs e)
        {
            clicked = false;
            //moves the mesh one last time
            int differencez = (e.Location.X - lastClicked.X) / 2;
            //% to keep the value in range
            RotationZNUD.Value = (RotationZNUD.Value - differencez + 175) % 175;

        }
        #endregion
    }

}