using System.Drawing;
using System.Windows.Forms;

namespace NEA_Procedural_World_Generator
{
    public partial class MeshForm : Form
    {
        public PictureBox p, MenuBox;
        public NumericUpDown RotationNUD;
        public int width = 800;
        public int height = 500;
        public MeshForm()
        {
            Initialise();
            InitializeComponent();
        }

        public void Initialise()
        {
            Size = new Size(width, height);
            p = new PictureBox
            {
                Size = new Size(width - 120, height - 10),
                Location = new Point(110, 5),
                BorderStyle = BorderStyle.FixedSingle
            };
            Controls.Add(p);
            p.BringToFront();

            MenuBox = new PictureBox
            {
                Location = new Point(5, 5),
                BorderStyle = BorderStyle.FixedSingle,
                Size = new Size(100, height - 10)
            };
            Controls.Add(MenuBox);


            RotationNUD = InterfaceHandler.SliderCreator(new Point(0, 20), 0f, 360f, 0, 4f, 1f, MenuBox);
        }


    }

}