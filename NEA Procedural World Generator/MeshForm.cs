using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NEA_Procedural_World_Generator
{
    public partial class MeshForm : Form
    {
        public PictureBox p;
        public MeshForm()
        {
            p = new PictureBox
            {
                Size = new Size(Form1.UI.TerrainBox.Width, Form1.UI.TerrainBox.Height),
                Location = new Point(0, 0)
            };
            Controls.Add(p);
            p.BringToFront();
            InitializeComponent();
        }


    }
}
