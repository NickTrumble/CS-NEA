using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NEA_Procedural_World_Generator
{
    public partial class Form1 : Form
    {
        //public variables
        public static InterfaceHandler UI;
        public static float xoff = 0;
        public static float yoff = 0;
        public static World world;


        //private variables

        public Form1(World inWorld = null)
        {
            InitializeComponent();
            if (inWorld != null)
            {
                world = inWorld;
            }
            UI = new InterfaceHandler(this);
        }
    }
}
//GOALS
//make mesh form
//mass optimisation
//save options e.g. other formats plus select region
//manual and settings:
//  instructions
//  cmaps and colour blind
//  font sizes

//loading bar/symbol / make saving async
//zooming
//smaller worlds
//fix ui design
//selecting visuals