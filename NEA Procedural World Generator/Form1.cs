using System.ComponentModel;
using System.Data;
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

        public Form1()
        {
            InitializeComponent();
            world = new World(32, 4, 0.5f, 0.008f);
            UI = new InterfaceHandler(this);
        }
    }
}
//GOALS
//mesh - started:
//make mesh form
//make colour lerp
//mass optimisation
//save options e.g. other formats plus select region
//manual and settings:
//  instructions
//  cmaps and colour blind
//  font sizes

//loading bar/symbol / make saving async
//terracingh
//zooming
//smaller worlds
//fix ui design
//selecting visuals