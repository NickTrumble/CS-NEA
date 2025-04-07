using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization.Configuration;

namespace NEA_Procedural_World_Generator
{
    //class to handle all aspects of the interface generation
    public class InterfaceHandler
    {
        #region variables
        //PUBLIC ENUMERATORS

        //determines what file format the program saves the mesh in
        public enum SaveFileType { OBJ, PLY }
        //determines what noise generation algorithm the program uses next time the 
        //user genernates new terrain
        public enum NoiseState { Perlin, Simplex }
        //determines what to do when the mouse is moving
        public enum MouseState { Nothing, Editing, Moving, Selecting, Saving }
        //determines whether the mouse is being clicked and moved 
        public enum DraggingState { None, Dragging }

        //public controls - UI
        public PictureBox MenuBox, TerrainBox;
        public NumericUpDown WorldSizeNUD, ScaleNUD, OctavesNUD, PersistanceNUD, ColourNumNUD, IslandNUD;
        public Button StartButton, PerlinGen, SimplexGen, EditWorldButton, MoveWorldButton, MouseModeButton, UndoButton,
            RedoButton, SaveButton, MeshButton, ExplanationButton, SettingsButton, ZoomInButton, ZoomOutButton;

        //public variables
        public static MouseState MouseMode = MouseState.Nothing;
        public static NoiseState NoiseMethodd = NoiseState.Perlin;
        public static SaveFileType FileType = SaveFileType.OBJ;

        public static int zoom = 2;//allows for program to account for zoom and scale the terrain
        public int width, height;//width and height of the form

        //private controls
        private Form1 Form;
        private Label RadiusLabel, IntensityLabel, MousePosLabel, TitleLabel, LogoLabel;

        //private variables
        private DraggingState Drag = DraggingState.None;//new instance of dragging state

        
        private (int x, int y) CornerChunk;//determines the first chunk pressed when making a mesh or saving
        private PointF lastPos;//helps the program determine if the mouse is moving when dragging the mouse
        private float intensity = 0.03f;//intensity of the brush to edit terrain
        private int radius = 30;//radius of the brush to edit the terrain
        private string SavePath;//string version of directory to save mesh

        #endregion

        #region Setup
        //Constructer
        public InterfaceHandler(Form1 form)
        {
            Form = form;
            width = 850;
            height = 500;
            InitialiseInterface();
        }

        //Generates and initialises all of the interfaces
        public void InitialiseInterface()
        {
            Form.Size = new Size(width, height);
            Form.Text = "Procedural Terrain Generator & Editor";
            // Contains all actions, e.g. save,edit,undo...
            MenuBox = new PictureBox
            {
                Location = new Point(5, 5),
                Size = new Size(100, Form.ClientSize.Height - 10),
                BorderStyle = BorderStyle.FixedSingle,
                //BackColor = Color.Blue
            };
            Form.Controls.Add(MenuBox);

            // Contains the generated terrain
            TerrainBox = new PictureBox
            {
                Location = new Point(110, 5),
                Size = new Size(Form.ClientSize.Width - 115, Form.ClientSize.Height - 10),
                BorderStyle = BorderStyle.FixedSingle,
                //BackColor = Color.Red
            };
            TerrainBox.MouseDown += TerrainBoxMouseDown;
            TerrainBox.MouseMove += TerrainBoxMouseMove;
            TerrainBox.MouseUp += TerrainBoxMouseUp;
            TerrainBox.Paint += PopulateTerrainBox;
            TerrainBox.MouseWheel += BrushEditor;
            Form.Controls.Add(TerrainBox);

            //generates first world
            if (!Form1.Started)
            {
                StartButton = new Button
                {
                    Location = new Point((TerrainBox.Width / 2) - 60, (TerrainBox.Height / 2) - 40),
                    Size = new Size(120, 80),
                    Text = "Load World"
                };
                StartButton.BringToFront();
                StartButton.Click += StartButtonClick;
                TerrainBox.Controls.Add(StartButton);

                //Title on start
                TitleLabel = new Label
                {
                    Location = new Point(TerrainBox.Width / 5, TerrainBox.Height / 4),
                    Text = "Procedural Terrain Generator",
                    Font = new Font("Arial", 26, FontStyle.Bold),
                    Size = new Size(TerrainBox.Width, 50),
                };
                TerrainBox.Controls.Add(TitleLabel);
            }
            

            //BUTTONS//
            //regenerate perlin
            PerlinGen = ButtonCreator(PerlinButtonClick, new Point(0, MenuBox.Height - 25),
                "Perlin Generation", new Size(100, 25), null, MenuBox);

            //regenerate simplex
            SimplexGen = ButtonCreator(SimplexButtonClick, new Point(0, MenuBox.Height - 50),
                "Simplex Gen", new Size(100, 25), null, MenuBox);

            //undo button
            UndoButton = ButtonCreator(UndoButtonClick, new Point(0, MenuBox.Height - 75),
                "Undo", new Size(50, 25), null, MenuBox);

            //redo button
            RedoButton = ButtonCreator(RedoButtonClick, new Point(50, MenuBox.Height - 75),
                "Redo", new Size(50, 25), null, MenuBox);

            //mouse mode switch
            MouseModeButton = ButtonCreator(MouseModeButtonClick, new Point(0, MenuBox.Height - 100),
                "🖌", new Size(100, 25), new Font("Arial", 12), MenuBox);

            IntensityLabel = LabelCreator(new Point(0, MenuBox.Height - 120), $"Brush Intensity:{intensity * 100f}", MenuBox);
            RadiusLabel = LabelCreator(new Point(0, MenuBox.Height - 140), $"Brush Radius:{radius}", MenuBox);

            //save button
            SaveButton = ButtonCreator(SaveButtonClick, new Point(0, MenuBox.Height - 165),
                "Save Terrain", new Size(100, 25), null, MenuBox);

            //instructions button
            ExplanationButton = ButtonCreator(HelpButtonClick, new Point(0, MenuBox.Height - 190),
                "?", new Size(50, 25), new Font("Arial", 12), MenuBox);
            //settings button
            SettingsButton = ButtonCreator(SettingsButtonClick, new Point(50, MenuBox.Height - 190),
                "⚙️", new Size(50, 25), new Font("Arial", 12), MenuBox);

            //Mesh button
            MeshButton = ButtonCreator(MeshButtonClick, new Point(0, MenuBox.Height - 215),
                "Create Mesh", new Size(100, 25), null, MenuBox);

            //Zoom In buttton
            ZoomInButton = ButtonCreator(ZoomInButtonClick, new Point(0, MenuBox.Height - 240),
                "➕", new Size(50, 25), null, MenuBox);
            
            //Zoom Out buttton
            ZoomOutButton = ButtonCreator(ZoomOutButtonClick, new Point(50, MenuBox.Height - 240),
                "➖", new Size(50, 25), null, MenuBox);


            //SLIDERS AND LABELS//

            //Top Left title
            LogoLabel = new Label
            {
                Location = new Point(0, 0),
                Size = new Size(MenuBox.Width, 60),
                Text = "Procedural Terrain Generator",
                Font = new Font("Arial", 12),
                TextAlign = ContentAlignment.TopCenter,
            };
            MenuBox.Controls.Add(LogoLabel);

            //mouse pos
            MousePosLabel = LabelCreator(new Point(0, MenuBox.Height - 390), "[0, 0] = 0.0", MenuBox);

            //world sizez
            WorldSizeNUD = SliderCreator(new Point(60, MenuBox.Height - 365), 24, 256, 0, 32, 1, MenuBox);
            LabelCreator(new Point(0, MenuBox.Height - 365), "World Size:", MenuBox);

            //Scale
            ScaleNUD = SliderCreator(new Point(60, MenuBox.Height - 340), 1f, 16f, 0, 6f, 1, MenuBox);
            LabelCreator(new Point(0, MenuBox.Height - 340), "Scale:", MenuBox);

            //Octaves
            OctavesNUD = SliderCreator(new Point(60, MenuBox.Height - 315), 1f, 10f, 0, 8, 1, MenuBox);
            LabelCreator(new Point(0, MenuBox.Height - 315), "Octaves:", MenuBox);

            //Persistance
            PersistanceNUD = SliderCreator(new Point(60, MenuBox.Height - 290), 0.1f, 2f, 1, 0.5f, 0.1f, MenuBox);
            LabelCreator(new Point(0, MenuBox.Height - 290), "Persistance:", MenuBox);

            //Island scaler
            IslandNUD = SliderCreator(new Point(60, MenuBox.Height - 265), T: MenuBox);
            LabelCreator(new Point(0, MenuBox.Height - 265), "Ocean Val:", MenuBox);

        }
        #endregion

        #region control editors
        //used to make buttons quicker and easier
        public static Button ButtonCreator(EventHandler OnClick, Point loc, string text, Size size, Font font = null, Control T = null)
        {
            if (font == null)
            {
                font = new Font("Arial", 8.5f);
            }
            Button b = new Button
            {
                Location = loc,
                Text = text,
                Size = size,
                Font = font
            };
            b.Click += OnClick;
            T.Controls.Add(b);
            return b;
        }

        //used to make labels quicker and easier
        public static Label LabelCreator(Point loc, string text, Control T = null)
        {
            if (T == null)
            {
                T = Form1.UI.MenuBox;
            }
            Label l = new Label
            {
                Location = loc,
                Text = text,
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Ariel", 8)
            };
            T.Controls.Add(l);
            return l;
        }

        //used to make numeric updowns quicker and easier
        public static NumericUpDown SliderCreator(Point loc, float min = 0, float max = 10, int dp = 0, float val = 0, float inc = 1, Control T = null)
        {
            if (T == null)
            {
                T = Form1.UI.MenuBox;
            }
            NumericUpDown nud = new NumericUpDown
            {
                Minimum = (decimal)min,
                Maximum = (decimal)max,
                Value = (decimal)val,
                DecimalPlaces = dp,
                Location = loc,
                Increment = (decimal)inc,
                Size = new Size(35, 25)
            };
            nud.Controls[0].Visible = false;
            T.Controls.Add(nud);
            nud.BringToFront();
            return nud;
        } 

        //updates the labels containing the brush data when called
        private void UpdateLabels()
        {
            IntensityLabel.Text = $"Brush Intensity:{(int)(intensity * 100f)}";
            RadiusLabel.Text = $"Brush Radius:{radius}";
        }
        #endregion

        #region click handlers

        //increments zoom value and calls terrain paint event
        private void ZoomInButtonClick(object sender, EventArgs e)
        {
            zoom++;
            TerrainBox.Invalidate();
        }

        //decrements zoom value and calls terrain paint event
        private void ZoomOutButtonClick(object sender, EventArgs e)
        {
            zoom = Math.Max(zoom - 1, 1);
            TerrainBox.Invalidate();
        }
      
        //opens setting forms
        public void SettingsButtonClick(object sender, EventArgs e)
        {
            SettingsForm Settingform = new SettingsForm(Form);
            Settingform.Show();
        }

        //opens help form with information
        public void HelpButtonClick(object sender, EventArgs e)
        {
            HelpForm Helpform = new HelpForm();
            Helpform.Show();
        }

        //allows the user to convert terrain into mesh
        public void MeshButtonClick(object sender, EventArgs e)
        {
            if (MouseMode == MouseState.Selecting)
            {
                //go back to default and cancel actions if pressed second time
                Form.Text = "Procedural Terrain Generator & Editor";
                MouseModeButtonClick(sender, e);
                MeshButton.Text = "Mesh Form";
            }
            else
            {
                //lets the user select the first corner to convert to mesh
                Form.Text = "Click one corner of area to convert to mesh form:";
                MouseMode = MouseState.Selecting;//tells the program to collect mouse data
                MeshButton.Text = "Cancel";
            }
            
        }

        //Saving Mesh button
        public async void SaveButtonClick(object sender, EventArgs e)
        {
            OBJExport exporter = new OBJExport(Form1.world, 130);//initialise new Exporter
            DialogResult result = MessageBox.Show(
                "Would you like to Save the whole Map?",
                "Save Terrain As...", MessageBoxButtons.YesNoCancel);//asks if the user wants to save the entire terrain
            FolderBrowserDialog filelocation = new FolderBrowserDialog
            {
                Description = "Save Terrain at...",
                SelectedPath = "C:\\Users\\iantr\\source\\repos\\NEA Procedural World Generator\\NEA Procedural World Generator"
            };//collects the directory to save to

            if (result != DialogResult.Cancel)//if the user chooses to save either all of the terrain or part of it...
            {
                if (filelocation.ShowDialog() == DialogResult.OK)//once the user chooses a directory
                {
                    SavePath = filelocation.SelectedPath;//save directory path
                    if (result == DialogResult.Yes)//if the user saves the whole terrain
                    {
                        //await to not block the program from being responsive
                        await exporter.SaveAll(SavePath);
                    }
                    else if (result == DialogResult.No)//lets the user choose chunks if they selected only part
                    {
                        Form.Text = "Click one corner of area to save as a mesh:";
                        MouseMode = MouseState.Saving;//tells the program to keep the mouse data for saving
                        SaveButton.Text = "Cancel";
                    }
                }
            }


        }

        //undoes latest edits when clicked
        private void UndoButtonClick(object sender, EventArgs e)
        {
            if (Form1.world.UndoStack.Count > 0)//check count, if there have been edits -> pushes to the stack
            {
                Form1.world.RedoStack.Push(World.CloneWorld(Form1.world.WorldChunks.Values.ToList()));//redo stack.push -> current world.clone
                foreach (Chunk c in Form1.world.UndoStack.Pop())//iterate over every chunk in the top world of the undo stack
                {
                    Form1.world.WorldChunks[(c.X, c.Y)] = c;//current world -> undo stack,pop
                }
                Form1.world.temp = World.CloneWorld(Form1.world.WorldChunks.Values.ToList());//add the current world to the temp

                TerrainBox.Invalidate(); //redraw
            }
        }

        //undoes latest undo button clicked on press
        private void RedoButtonClick(object sender, EventArgs e)
        {
            if (Form1.world.RedoStack.Count > 0)//check count, if undo button pressed  -> pushes to redo stack
            {
                Form1.world.UndoStack.Push(World.CloneWorld(Form1.world.WorldChunks.Values.ToList()));//undo stack.push -> current.world.clone
                foreach (Chunk c in Form1.world.RedoStack.Pop())//iterate over every chunk in the top world of the redo stack
                {
                    Form1.world.WorldChunks[(c.X, c.Y)] = c;//current world -> redo stack.pop
                }

                TerrainBox.Invalidate();//redraw
            }
        }

        //swaps mouse mode between editing the terrain and moving around it
        private void MouseModeButtonClick(object sender, EventArgs e)
        {
            if (MouseMode == MouseState.Editing)
            {
                //if mouse mode is editing, make it moving
                MouseMode = MouseState.Moving;
                MouseModeButton.Text = "✋";
            }
            else
            {
                //if mouse mode is moving, make it editing
                MouseMode = MouseState.Editing;
                MouseModeButton.Text = "🖌";
            }

        }

        //generates a new world using Perlin Noise Generation
        private void PerlinButtonClick(object sender, EventArgs e)
        {
            NoiseMethodd = NoiseState.Perlin;//sets generation method to perlin, terlls the program which inherited function to call
            //creates new world template with the settings
            Form1.world = new World((int)(WorldSizeNUD.Value), (int)OctavesNUD.Value, (float)PersistanceNUD.Value, (float)(ScaleNUD.Value * 0.001M));
            //generates the world
            Form1.world.WorldGeneration();
        }

        //generates a new world using Simplex Noise Generation
        private void SimplexButtonClick(object sender, EventArgs e)
        {
            NoiseMethodd = NoiseState.Simplex;//sets generation method to Simplex, terlls the program which inherited function to call
            //creates new world template with the settings
            Form1.world = new World((int)(WorldSizeNUD.Value), (int)OctavesNUD.Value, (float)PersistanceNUD.Value, (float)(ScaleNUD.Value * 0.001M));
            //generates the world
            Form1.world.WorldGeneration();
        }

        //begins generating a world if there is one, or just reloads the current one
        public void StartButtonClick(object sender, EventArgs e)
        {
            //remove the start button from controls
            TerrainBox.Controls.Remove(StartButton);
            TerrainBox.Controls.Remove(TitleLabel);
            if (Form1.world == null)//if the world hasnt been generated, make a new one
            {
                Form1.world = new World((int)WorldSizeNUD.Value, (int)OctavesNUD.Value, (float)PersistanceNUD.Value, (float)(ScaleNUD.Value * 0.001M));
                //start function/timer
                Form1.world.WorldGeneration();
                Form1.Started = true;//tells the program to not generate a new world when coming back to home form
            } else
            {
                TerrainBox.Invalidate();//refresh terrain box, aka reload world
            }
            MouseModeButtonClick(sender, e);//resets the mouse mode
        }

        //closes form
        private void ExitButtonClick(object sender, EventArgs e)
        {
            //exit form and stop debugging
            Application.Exit();
        }

        #endregion

        #region Other Handlers
        private void TerrainBoxMouseDown(object sender, MouseEventArgs e)
        {
            if (TerrainBox.Contains(StartButton))
            {
                return;//check if start button pressed, else return
            }

            //check if in move mode or edit
            if (MouseMode == MouseState.Editing)
            {
                //intensity positive if left click, else negative to decrease
                intensity = (e.Button == MouseButtons.Left) ? intensity : -intensity;
                //call edit world function
                Form1.world.EditWorld(e.Location.X, e.Location.Y, radius, intensity / 1f);
                TerrainBox.Invalidate();//refresh terrain box
            }
            if (MouseMode == MouseState.Moving)//if mouse is in moving mode
            {
                lastPos = e.Location;//set new lastpos location to determine if mouse is moving
                TerrainBox.Cursor = Cursors.Hand;
            }
            else
            {//other modes, e.g. saving and selecting for mesh both use corner, so save the first corner
                CornerChunk = ((int)Math.Floor(e.X / zoom + Form1.xoff * World.chunkSize) / World.chunkSize,
                               (int)Math.Floor(e.Y / zoom + (Form1.yoff * World.chunkSize)) / World.chunkSize);
            }
            Drag = DraggingState.Dragging;//update dragging to true
        }

        //checks if mouse is moving
        private void TerrainBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (Drag == DraggingState.Dragging)//if mouse is being held down
            {
                if (MouseMode == MouseState.Moving)//checks if the terrain is moving
                {
                    PointF Pos = e.Location;//sets the location of the new pos
                    //determines new offsets to draw the terrain, accoutns for zoom
                    Form1.xoff = Math.Max(0, Form1.xoff - (Pos.X - lastPos.X) / (World.chunkSize * zoom));
                    Form1.yoff = Math.Max(0, Form1.yoff - (Pos.Y - lastPos.Y) / (World.chunkSize * zoom));

                    //offsets have a max value of the world size in chunks - chunks that fit in the terrain box
                    if (Form1.xoff > Form1.world.Size - TerrainBox.Width / (32f * zoom)) Form1.xoff = Form1.world.Size - TerrainBox.Width / (32f * zoom);
                    if (Form1.yoff > Form1.world.Size - TerrainBox.Height / (32f * zoom)) Form1.yoff = Form1.world.Size - TerrainBox.Height / (32f * zoom);
                    TerrainBox.Invalidate();//redraw the terrain box
                    lastPos = Pos;//update last pos for movement checks
                }
                else if (MouseMode == MouseState.Editing)//is mousemode = editing
                {
                    //edit terrain with current mouse pos
                    Form1.world.EditWorld(e.Location.X, e.Location.Y, radius, intensity / 10f);
                    TerrainBox.Invalidate();//redraw terrain
                }

            }
            if (!TerrainBox.Controls.Contains(StartButton))
            {
                //displays elevation of point currently hovering over
                int size = Form1.world.Size;
                //offsets already account for zoom when moving, so only change mouse coordinates
                int mousex = Math.Max(0, Math.Min((int)(e.Location.X / zoom + (Form1.xoff * World.chunkSize)), size * World.chunkSize));
                int mouseu = Math.Max(0, Math.Min((int)(e.Location.Y / zoom + (Form1.yoff * World.chunkSize)), size * World.chunkSize));
                //get block elevation at block hovering over
                float elevation = Form1.world.WorldChunks[(Math.Min(mousex / World.chunkSize, size - 1), Math.Min(size - 1, mouseu / World.chunkSize))]
                    .ChunkBlock[(mousex - (mousex / World.chunkSize) * World.chunkSize, mouseu - (mouseu / World.chunkSize) * World.chunkSize)].Z;
                //displays new elevation
                MousePosLabel.Text = $"[{mousex}, {mouseu}] = {elevation.ToString().Substring(0, Math.Min(4, elevation.ToString().Length))}";
            }
        }
        
        //async for saving
        private async void TerrainBoxMouseUp(object sender, MouseEventArgs e)
        {
            if (MouseMode == MouseState.Editing)
            {
                //push new world onto undo stack
                Form1.world.UndoStack.Push(World.CloneWorld(Form1.world.temp));
                //keep track of current world to push onto stack if pressed
                Form1.world.temp = World.CloneWorld(Form1.world.WorldChunks.Values.ToList());
                //makes intensity positive again
                intensity = Math.Abs(intensity);
            }
            else if (MouseMode == MouseState.Selecting)
            {
                //finds the second corner for converting into a mesh
                (int x, int y) SecondCorner = ((int)(e.X / zoom + (Form1.xoff * World.chunkSize)) / World.chunkSize,
                                               (int)(e.Y / zoom + (Form1.yoff * World.chunkSize)) / World.chunkSize);
                //creates a new instance of draw mesh, using the new bounds of corner chunk and second corner
                DrawMesh MeshDrawer = new DrawMesh(Form1.world, 200, CornerChunk.x, SecondCorner.x, CornerChunk.y, SecondCorner.y);
                //new instance of Mesh form to show
                MeshForm mp = new MeshForm(MeshDrawer, Form);
                mp.Show();
                Form.Hide();
            }
            else if (MouseMode == MouseState.Saving)
            {
                //new instance of OBJExport to export terrain
                OBJExport exporter = new OBJExport(Form1.world, 130);
                //finds the second corner for saving the mesh
                (int x, int y) SecondCorner = ((int)Math.Floor(e.X / zoom + (Form1.xoff * World.chunkSize)) / World.chunkSize,
                                               (int)Math.Floor(e.Y / zoom + (Form1.yoff * World.chunkSize)) / World.chunkSize);
                //width and height of second corner to corner chunk to find upper bounds
                int W = Math.Abs(SecondCorner.x - CornerChunk.x) + 1;
                int H = Math.Abs(SecondCorner.y - CornerChunk.y) + 1;

                //sets lower bounds to smaller of the two chunks
                int xLow = (SecondCorner.x < CornerChunk.x) ? SecondCorner.x : CornerChunk.x;
                int yLow = (SecondCorner.y < CornerChunk.y) ? SecondCorner.y : CornerChunk.y;

                //updates the form text
                Form.Text = "Saving Terrain Region...";
                MouseModeButtonClick(sender, e);

                //await export the selected region, await to keep responsive
                await exporter.SaveRegion(xLow, yLow, xLow + W, yLow + H, SavePath);

                Form.Text = "Procedural Terrain Generator and Editor";
                SaveButton.Text = "Save Terrain";

            }
            Drag = DraggingState.None;//restores dragging to false as mouse lifted up
            TerrainBox.Cursor = Cursors.Default;
        }

        //edits brush withb scroll wheel
        private void BrushEditor(object sender, MouseEventArgs e)
        {
            //if shift is pressed while scrolling, update intensity
            if ((Control.ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (e.Delta < 0)
                {
                    intensity += 0.01f;
                }
                else if (intensity > 0.01f)
                {
                    intensity -= 0.01f;
                }
            }
            else
            {//if no shift key is pressed, radius of the brush is pressed
                if (e.Delta < 0)
                {
                    radius += 5;
                }
                else if (radius > 5)
                {
                    radius -= 5;
                }
            }
            UpdateLabels();//update the labels on the form with the new values
        }

        //terrain box paint event
        public void PopulateTerrainBox(object sender, PaintEventArgs e)
        {
            //finds lower and upper bounds to draw, accounting for zoom, to conserve resources 
            //anmd not draw unseen chunks off the screen
            int startx = BaseNoise.fastfloor(Form1.xoff);
            int starty = BaseNoise.fastfloor(Form1.yoff);
            int endx = (int)Math.Ceiling(Form1.xoff + (24 / zoom)) + zoom;
            int endy = (int)Math.Ceiling(Form1.yoff + (16 / zoom)) + zoom;

            //stops gaps appearing between chunks
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            //if the user has generated the world already...
            if (!TerrainBox.Controls.Contains(StartButton))
            {
                //iterate through chunks
                for (int i = startx; i < endx; i++)
                {
                    for (int j = starty; j < endy; j++)
                    {
                        //wraps i and j to prevent out of index exception
                        int indexi = i % Form1.world.Size;
                        int indexj = j % Form1.world.Size;
                        //retrieves the bitmap from the chunk class
                        Bitmap bmp = Form1.world.WorldChunks[(indexi, indexj)].Bmp;
                        lock (bmp)//locks to become thread safe
                        {
                            //draw the current chunk at the specific coordinates based on offsets and zoom
                            e.Graphics.DrawImage(bmp, World.chunkSize * (i - Form1.xoff) * zoom, World.chunkSize * (j - Form1.yoff) * zoom,
                                (0.5f + World.chunkSize) * zoom, (0.5f + World.chunkSize) * zoom);
                            e.Graphics.DrawRectangle(Pens.Black, World.chunkSize * (i - Form1.xoff) * zoom, World.chunkSize * (j - Form1.yoff) * zoom,
                                (0.5f + World.chunkSize) * zoom, (0.5f + World.chunkSize) * zoom);
                        }

                    }
                }

            }


        }

        #endregion
    }

}