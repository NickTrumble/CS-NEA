using System.Drawing;
using System.Windows.Forms;
using System;

namespace NEA_Procedural_World_Generator
{
    public partial class MeshForm : Form
    {
        public PictureBox TerrainBox, MenuBox;
        public NumericUpDown RotationZNUD;
        public int width = 800;
        public int height = 500;
        DrawMesh MeshDrawer;
        private Point lastClicked;
        bool clicked = false;
        public MeshForm(DrawMesh dm)
        {
            MeshDrawer = dm;
            InitializeComponent();
            Initialise();
        }

        public void Initialise()
        {
            FormBorderStyle = FormBorderStyle.SizableToolWindow;
            
            Size = new Size(width, height);
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

            MenuBox = new PictureBox
            {
                Location = new Point(5, 5),
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(100, ClientSize.Height - 20)
            };
            Controls.Add(MenuBox);


            RotationZNUD = InterfaceHandler.SliderCreator(new Point(5, 20), 0f, 360f, 1, 4f, 1f, MenuBox);
            RotationZNUD.ValueChanged += RotationSliderUpdate;
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

                RotationZNUD.Value = (RotationZNUD.Value - differencez + 360) % 360;

                lastClicked = e.Location;
            }

            
        }

        public void TerrainBoxMouseUp(object sender, MouseEventArgs e)
        {
            clicked = false;
            int differencez = (e.Location.X - lastClicked.X) / 2;
            RotationZNUD.Value = (RotationZNUD.Value - differencez + 360) % 360;

        }
    }

}