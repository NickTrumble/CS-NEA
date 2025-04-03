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
        //public enumerators
        public enum SaveFileType { OBJ, PLY }
        public enum NoiseState { Perlin, Simplex }
        public enum MouseState { Nothing, Editing, Moving, Selecting, Saving }
        public enum DraggingState { None, Dragging }

        //public controls
        public PictureBox MenuBox, TerrainBox;
        public NumericUpDown WorldSizeNUD, ScaleNUD, OctavesNUD, PersistanceNUD, ColourNumNUD, IslandNUD;
        public Button StartButton, PerlinGen, SimplexGen, EditWorldButton, MoveWorldButton, MouseModeButton,
            UndoButton, RedoButton, SaveButton, MeshButton;

        //public variables
        public static MouseState MouseMode = MouseState.Nothing;
        public static NoiseState NoiseMethodd = NoiseState.Perlin;
        public static SaveFileType FileType = SaveFileType.OBJ;

        public static int zoom = 2;
        public int width, height;

        //private controls
        private Form1 Form;
        private Button ExplanationButton, SettingsButton, ZoomInButton, ZoomOutButton;
        private Label RadiusLabel, IntensityLabel, MousePosLabel, TitleLabel, LogoLabel;

        //private variables
        private DraggingState Drag = DraggingState.None;

        private (int x, int y) CornerChunk;
        private PointF lastPos;
        private float intensity = 0.03f;
        private int radius = 30;
        private string SavePath;

        #endregion

        #region Setup
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

        private void UpdateLabels()
        {
            IntensityLabel.Text = $"Brush Intensity:{(int)(intensity * 100f)}";
            RadiusLabel.Text = $"Brush Radius:{radius}";
        }
        #endregion

        #region click handlers

        private void ZoomInButtonClick(object sender, EventArgs e)
        {
            zoom++;
            TerrainBox.Invalidate();
        }        
        
        private void ZoomOutButtonClick(object sender, EventArgs e)
        {
            zoom = Math.Max(zoom - 1, 1);
            TerrainBox.Invalidate();
        }
      
        public void SettingsButtonClick(object sender, EventArgs e)
        {
            SettingsForm Settingform = new SettingsForm(Form);
            Settingform.Show();
        }

        public void HelpButtonClick(object sender, EventArgs e)
        {
            HelpForm Helpform = new HelpForm();
            Helpform.Show();
        }

        public void MeshButtonClick(object sender, EventArgs e)
        {
            if (MouseMode == MouseState.Selecting)
            {
                Form.Text = "Procedural Terrain Generator & Editor";
                MouseModeButtonClick(sender, e);
                MeshButton.Text = "Mesh Form";
            }
            else
            {
                Form.Text = "Click one corner of area to convert to mesh form:";
                MouseMode = MouseState.Selecting;
                MeshButton.Text = "Cancel";
            }
            
        }

        public async void SaveButtonClick(object sender, EventArgs e)
        {
            OBJExport exporter = new OBJExport(Form1.world, 130);
            DialogResult result = MessageBox.Show(
                "Would you like to Save the whole Map?",
                "Save Terrain As...", MessageBoxButtons.YesNoCancel);
            FolderBrowserDialog filelocation = new FolderBrowserDialog
            {
                Description = "Save Terrain at...",
                SelectedPath = "C:\\Users\\iantr\\source\\repos\\NEA Procedural World Generator\\NEA Procedural World Generator"
            };

            if (result != DialogResult.Cancel)
            {
                if (filelocation.ShowDialog() == DialogResult.OK)
                {
                    SavePath = filelocation.SelectedPath;
                    if (result == DialogResult.Yes)
                    {

                        await exporter.SaveAll(SavePath);
                    }
                    else if (result == DialogResult.No)//choose chunks
                    {
                        Form.Text = "Click one corner of area to save as a mesh:";
                        MouseMode = MouseState.Saving;
                        SaveButton.Text = "Cancel";
                    }
                }
            }


        }





        private void UndoButtonClick(object sender, EventArgs e)
        {
            if (Form1.world.UndoStack.Count > 0)//check count
            {
                Form1.world.RedoStack.Push(World.CloneWorld(Form1.world.WorldChunks.Values.ToList()));//redo stack.push -> current world.clone
                foreach (Chunk c in Form1.world.UndoStack.Pop())
                {
                    Form1.world.WorldChunks[(c.X, c.Y)] = c;//current world -> undo stack,pop
                }
                Form1.world.temp = World.CloneWorld(Form1.world.WorldChunks.Values.ToList());

                TerrainBox.Invalidate(); //redraw
            }
        }

        private void RedoButtonClick(object sender, EventArgs e)//
        {
            if (Form1.world.RedoStack.Count > 0)//check count
            {
                Form1.world.UndoStack.Push(World.CloneWorld(Form1.world.WorldChunks.Values.ToList()));//undo stack.push -> current.world.clone
                foreach (Chunk c in Form1.world.RedoStack.Pop())
                {
                    Form1.world.WorldChunks[(c.X, c.Y)] = c;//current world -> redo stack.pop
                }

                TerrainBox.Invalidate();//redraw
            }
        }

        private void MouseModeButtonClick(object sender, EventArgs e)
        {
            if (MouseMode == MouseState.Editing)
            {
                MouseMode = MouseState.Moving;
                MouseModeButton.Text = "✋";
            }
            else
            {
                MouseMode = MouseState.Editing;
                MouseModeButton.Text = "🖌";
            }

        }

        private void PerlinButtonClick(object sender, EventArgs e)
        {
            NoiseMethodd = NoiseState.Perlin;
            Form1.world = new World((int)(WorldSizeNUD.Value), (int)OctavesNUD.Value, (float)PersistanceNUD.Value, (float)(ScaleNUD.Value * 0.001M));
            Form1.world.WorldGeneration();
        }

        private void SimplexButtonClick(object sender, EventArgs e)
        {
            NoiseMethodd = NoiseState.Simplex;
            Form1.world = new World((int)(WorldSizeNUD.Value), (int)OctavesNUD.Value, (float)PersistanceNUD.Value, (float)(ScaleNUD.Value * 0.001M));
            Form1.world.WorldGeneration();
        }

        private void StartButtonClick(object sender, EventArgs e)
        {
            //remove the start button from controls
            TerrainBox.Controls.Remove(StartButton);
            TerrainBox.Controls.Remove(TitleLabel);
            if (Form1.world == null)
            {
                Form1.world = new World((int)WorldSizeNUD.Value, (int)OctavesNUD.Value, (float)PersistanceNUD.Value, (float)(ScaleNUD.Value * 0.001M));
                //start function/timer
                Form1.world.WorldGeneration();
                Form1.Started = true;
            } else
            {
                TerrainBox.Invalidate();
            }
            MouseMode = MouseState.Editing;
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            //exit form and stop debugging
            Application.Exit();
        }

        #endregion

        #region Other Handlers
        private void TerrainBoxClick(object sender, MouseEventArgs e)
        {
            //if edit mode, :
            intensity = (e.Button == MouseButtons.Left) ? intensity : -intensity;
            Form1.world.EditWorld(e.Location.X, e.Location.Y, radius, intensity / 1f);
            TerrainBox.Invalidate();
        }

        private void TerrainBoxMouseDown(object sender, MouseEventArgs e)
        {
            if (TerrainBox.Contains(StartButton))
            {
                return;//check if start button pressed
            }

            //check if in move mode or edit
            if (MouseMode == MouseState.Editing)
            {
                //if edit mode, :
                intensity = (e.Button == MouseButtons.Left) ? intensity : -intensity;
                Form1.world.EditWorld(e.Location.X, e.Location.Y, radius, intensity / 1f);
                TerrainBox.Invalidate();
            }
            if (MouseMode == MouseState.Moving)
            {
                //if move mode, : 
                lastPos = e.Location;
                TerrainBox.Cursor = Cursors.Hand;
            }
            else
            {
                CornerChunk = ((int)(e.X + Form1.xoff * World.chunkSize) / World.chunkSize,
                               (int)(e.Y + (Form1.yoff * World.chunkSize)) / World.chunkSize);
            }
            Drag = DraggingState.Dragging;
        }

        private void TerrainBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (Drag == DraggingState.Dragging)
            {
                if (MouseMode == MouseState.Moving)
                {
                    PointF Pos = e.Location;
                    Form1.xoff = Math.Max(0, Form1.xoff - (Pos.X - lastPos.X) / (World.chunkSize * zoom));
                    Form1.yoff = Math.Max(0, Form1.yoff - (Pos.Y - lastPos.Y) / (World.chunkSize * zoom));

                    //offsets have a max value of the world size in chunks - chunks that fit in the terrain box
                    if (Form1.xoff > Form1.world.Size - TerrainBox.Width / (32f * zoom)) Form1.xoff = Form1.world.Size - TerrainBox.Width / (32f * zoom);
                    if (Form1.yoff > Form1.world.Size - TerrainBox.Height / (32f * zoom)) Form1.yoff = Form1.world.Size - TerrainBox.Height / (32f * zoom);
                    TerrainBox.Invalidate();
                    lastPos = Pos;
                }
                else if (MouseMode == MouseState.Editing)
                {
                    Form1.world.EditWorld(e.Location.X, e.Location.Y, radius, intensity / 10f);
                    TerrainBox.Invalidate();
                }

            }
            if (!TerrainBox.Controls.Contains(StartButton))//add for zoom
            {
                int size = Form1.world.Size;
                //offsets already account for zoom when moving, so only change mouse coordinates
                int mousex = Math.Max(0, Math.Min((int)(e.Location.X / zoom + (Form1.xoff * World.chunkSize)), size * World.chunkSize));
                int mouseu = Math.Max(0, Math.Min((int)(e.Location.Y / zoom + (Form1.yoff * World.chunkSize)), size * World.chunkSize));
                //get block elevation at block hovering over
                float elevation = Form1.world.WorldChunks[(Math.Min(mousex / World.chunkSize, size - 1), Math.Min(size - 1, mouseu / World.chunkSize))]
                    .ChunkBlock[(mousex - (mousex / World.chunkSize) * World.chunkSize, mouseu - (mouseu / World.chunkSize) * World.chunkSize)].Z;
                MousePosLabel.Text = $"[{mousex}, {mouseu}] = {elevation.ToString().Substring(0, Math.Min(4, elevation.ToString().Length))}";
            }


        }

        private async void TerrainBoxMouseUp(object sender, MouseEventArgs e)
        {
            if (MouseMode == MouseState.Editing)
            {
                //Form1.world.UndoStack.Push(World.CloneWorld(Form1.world.temp));//undo stack.push -> temp.clone
                //Form1.world.temp = World.CloneWorld(Form1.world.WorldChunks.Values.ToList()); //temp = current world,clone
                intensity = Math.Abs(intensity);
            }
            else if (MouseMode == MouseState.Selecting)
            {
                (int x, int y) SecondCorner = ((int)(e.X + (Form1.xoff * World.chunkSize)) / World.chunkSize,
                                               (int)(e.Y + (Form1.yoff * World.chunkSize)) / World.chunkSize);
                DrawMesh MeshDrawer = new DrawMesh(Form1.world, 200, CornerChunk.x, SecondCorner.x, CornerChunk.y, SecondCorner.y);
                MeshForm mp = new MeshForm(MeshDrawer, Form);
                mp.Show();
                Form.Hide();
            } else if (MouseMode == MouseState.Saving)
            {

                OBJExport exporter = new OBJExport(Form1.world, 0.015f);

                (int x, int y) SecondCorner = ((int)(e.X + (Form1.xoff * World.chunkSize)) / World.chunkSize,
                                               (int)(e.Y + (Form1.yoff * World.chunkSize)) / World.chunkSize);

                int W = Math.Abs(SecondCorner.x - CornerChunk.x) + 1;
                int H = Math.Abs(SecondCorner.y - CornerChunk.y) + 1;

                int xLow = (SecondCorner.x < CornerChunk.x) ? SecondCorner.x : CornerChunk.x;
                int yLow = (SecondCorner.y < CornerChunk.y) ? SecondCorner.y : CornerChunk.y;

                Form.Text = "Saving Terrain Region...";
                MouseMode = MouseState.Editing;

                await exporter.SaveRegion(xLow, yLow, xLow + W, yLow + H, SavePath);

                Form.Text = "Procedural Terrain Generator and Editor";
                SaveButton.Text = "Save Terrain";

            }

            Drag = DraggingState.None;
            TerrainBox.Cursor = Cursors.Default;


        }

        private void BrushEditor(object sender, MouseEventArgs e)
        {
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
            {
                if (e.Delta < 0)
                {
                    radius += 5;
                }
                else if (radius > 5)
                {
                    radius -= 5;
                }
            }
            UpdateLabels();
        }

        public void PopulateTerrainBox(object sender, PaintEventArgs e)
        {
            int startx = BaseNoise.fastfloor(Form1.xoff);
            int starty = BaseNoise.fastfloor(Form1.yoff);
            int endx = (int)Math.Ceiling(Form1.xoff + (24 / zoom)) + zoom;
            int endy = (int)Math.Ceiling(Form1.yoff + (16 / zoom)) + zoom;

            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

            if (!TerrainBox.Controls.Contains(StartButton))
            {
                for (int i = startx; i < endx; i++)
                {
                    for (int j = starty; j < endy; j++)
                    {
                        int indexi = i % Form1.world.Size;
                        int indexj = j % Form1.world.Size;
                        Bitmap bmp = Form1.world.WorldChunks[(indexi, indexj)].Bmp;
                        lock (bmp)
                        {
                            e.Graphics.DrawImage(bmp, World.chunkSize * (i - Form1.xoff) * zoom, World.chunkSize * (j - Form1.yoff) * zoom,
                                (0.5f + World.chunkSize) * zoom, (0.5f + World.chunkSize) * zoom);
                        }

                    }
                }

            }


        }

        #endregion
    }

}