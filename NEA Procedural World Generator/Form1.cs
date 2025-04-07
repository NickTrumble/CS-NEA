using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System;

namespace NEA_Procedural_World_Generator
{
    public partial class Form1 : Form
    {
        //public variables
        public static InterfaceHandler UI;//user interface class instance
        //offsets for terrain 
        public static float xoff = 0;
        public static float yoff = 0;
        //only world initialised
        public static World world;
        //tells the program if a world has been generated
        public static bool Started = false;

        //constructor
        public Form1(World inWorld = null)
        {
            InitializeComponent();
            //checks for world inputted from other form
            if (inWorld != null)
            {
                world = inWorld;
            }

            //reinitiallises terrain
            UI = new InterfaceHandler(this);

            this.Resize += FormResize;
        }

        //updates UI size on form resize
        public void FormResize(object sender, EventArgs e)
        {
            UI.width = this.Width;
            UI.height = this.Height;
            if (Started)
            {
                Controls.Clear();
                UI.InitialiseInterface();
            }
            
        }
    }
}