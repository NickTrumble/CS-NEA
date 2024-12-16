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
            world = new World(32);
            UI = new InterfaceHandler(this);
        }
    }
}
