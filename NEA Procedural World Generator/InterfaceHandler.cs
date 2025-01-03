﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NEA_Procedural_World_Generator
{
    //class to handle all aspects of the interface generation
    public class InterfaceHandler
    {
        //custom cmaps
        //mesh
        //mass optimisation
        //save button/form
        //brush radius


        //public variables
        public enum NoiseState { Perlin, Simplex }
        public enum MouseState { Editing, Moving }
        public static MouseState MouseMode = MouseState.Editing;
        public static NoiseState NoiseMethodd = NoiseState.Perlin;

        public PictureBox MenuBox, TerrainBox;
        public NumericUpDown WorldSizeNUD, ScaleNUD, OctavesNUD, PersistanceNUD, ColourNumNUD;
        public Button PerlinGen, SimplexGen, EditWorldButton, MoveWorldButton, MouseModeButton,
            UndoButton, RedoButton;
        public Label MousePosLabel;

        //private variables
        private Form1 Form;
        private Button StartButton;
        

        private int width;
        private int height;
        private PointF lastPos;
        private float intensity;

        private enum DraggingState { None, Dragging }
        private DraggingState Drag = DraggingState.None;

        public InterfaceHandler(Form1 form)
        {
            Form = form;
            width = 850;
            height = 500;
            InitialiseInterface();
        }


        //Generates and initialises all of the interfaces
        private void InitialiseInterface()
        {
            Form.Size = new Size(width, height);

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
                Size = new Size(Form.ClientSize.Width- 115, Form.ClientSize.Height - 10),
                BorderStyle = BorderStyle.FixedSingle,
                //BackColor = Color.Red
            };

            TerrainBox.MouseDown += TerrainBoxMouseDown;
            TerrainBox.MouseMove += TerrainBoxMouseMove;
            TerrainBox.MouseUp += TerrainBoxMouseUp;
            TerrainBox.Paint += PopulateTerrainBox;

            Form.Controls.Add(TerrainBox);
            //generates first world
            StartButton = new Button
            {
                Location = new Point((TerrainBox.Width / 2) - 60, (TerrainBox.Height / 2) - 40),
                Size = new Size(120, 80),
                Text = "Generate World"
            };
            StartButton.BringToFront();
            StartButton.Click += StartButtonClick;
            TerrainBox.Controls.Add(StartButton);

            //BUTTONS//
            //regenerate perlin
            PerlinGen = ButtonCreator(PerlinButtonClick, new Point(0, MenuBox.Height - 25),
                "Perlin Generation", new Size(100, 25));

            //regenerate simplex
            SimplexGen = ButtonCreator(SimplexButtonClick, new Point(0, MenuBox.Height - 50),
                "Simplex Gen", new Size(100, 25));

            //mouse mode switch
            MouseModeButton = ButtonCreator(MouseModeButtonClick, new Point(0, MenuBox.Height - 75),
                "🖌", new Size(100, 25));
            MouseModeButton.Font = new Font("Arial", 12);

            //undo button
            UndoButton = ButtonCreator(UndoButtonClick, new Point(0, MenuBox.Height - 100),
                "Undo", new Size(50, 25));

            //redo button
            RedoButton = ButtonCreator(RedoButtonClick, new Point(50, MenuBox.Height - 100),
                "Redo", new Size(50, 25));

            //SLIDERS AND LABELS//
            //mouse pos
            MousePosLabel = LabelCreator(new Point(0, 5), "[0, 0] = 0.0");

            //world sizez
            WorldSizeNUD = SliderCreator(new Point(60, 30), 24, 256, 0, 32, 1);
            LabelCreator(new Point(0, 30), "World Size:");

            //Scale
            ScaleNUD = SliderCreator(new Point(60, 55), 1f, 16f, 0, 8f, 1);
            LabelCreator(new Point(0, 55), "Scale:");

            //Octaves
            OctavesNUD = SliderCreator(new Point(60, 80), 1f, 10f, 0, 4, 1);
            LabelCreator(new Point(0, 80), "Octaves:");

            //Persistance
            PersistanceNUD = SliderCreator(new Point(60, 105), 0.1f, 2f, 1, 0.5f, 0.1f);
            LabelCreator(new Point(0, 105), "Persistance:");

            //Colour num 
            ColourNumNUD = SliderCreator(new Point(60, 130), 4, 50, 0, 4, 1);
            LabelCreator(new Point(0, 130), "Num. of Colours:");

        }

        private Button ButtonCreator(EventHandler OnClick, Point loc, string text, Size size)
        {
            Button b = new Button
            {
                Location = loc,
                Text = text,
                Size = size
            };
            b.Click += OnClick;
            MenuBox.Controls.Add(b);
            return b;
        }

        private Label LabelCreator(Point loc, string text)
        {
            Label l = new Label
            {
                Location = loc,
                Text = text,
                Size = new Size(100, 25),
                TextAlign = ContentAlignment.TopLeft,
                Font = new Font("Ariel", 8)
            };
            MenuBox.Controls.Add(l);
            return l;
        }

        private NumericUpDown SliderCreator(Point loc, float min = 0, float max = 10, int dp = 0, float val = 0, float inc = 1)
        {
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
            MenuBox.Controls.Add(nud);
            return nud;
        }

        private void UndoButtonClick(object sender, EventArgs e)
        {
            if (Form1.world.UndoStack.Count > 0)//check count
            {
                Form1.world.RedoStack.Push(World.CloneWorld(Form1.world.WorldChunks.Values.ToList()));//redo stack.push -> current world.clone
                foreach(Chunk c in Form1.world.UndoStack.Pop())
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
                foreach(Chunk c in Form1.world.RedoStack.Pop())
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

            //start function/timer
            Form1.world.WorldGeneration();
        }

        private void ExitButtonClick(object sender, EventArgs e)
        {
            //exit form and stop debugging
            Application.Exit();
        }

        private void TerrainBoxMouseDown(object sender, MouseEventArgs e)
        {
            //check if in move mode or edit
            if (MouseMode == MouseState.Editing)
            {
                //if edit mode, :
                intensity = (e.Button == MouseButtons.Left) ? 0.01f : -0.01f;
                Form1.world.EditWorld(e.Location.X, e.Location.Y, 30, intensity);
                TerrainBox.Invalidate();
            } else
            {
                //if move mode, : 
                    lastPos = e.Location;           
                    TerrainBox.Cursor = Cursors.Hand;
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
                    Form1.xoff -= (Pos.X - lastPos.X) / World.chunkSize;
                    Form1.yoff -= (Pos.Y - lastPos.Y) / World.chunkSize;
                    if (Form1.xoff < 0) Form1.xoff = 0;
                    if (Form1.yoff < 0) Form1.yoff = 0;

                    if (Form1.xoff > Form1.world.Size - TerrainBox.Width / 32f) Form1.xoff = Form1.world.Size - TerrainBox.Width / 32f;
                    if (Form1.yoff > Form1.world.Size - TerrainBox.Height / 32f) Form1.yoff = Form1.world.Size - TerrainBox.Height / 32f;
                    TerrainBox.Invalidate();
                    lastPos = Pos;
                } else
                {
                    Form1.world.EditWorld(e.Location.X, e.Location.Y, 30, intensity);
                    TerrainBox.Invalidate();
                }
                
            }
            if (!TerrainBox.Controls.Contains(StartButton))
            {
                int size = Form1.world.Size;
                int mousex = Math.Max(0, Math.Min(size * World.chunkSize, (int)(e.Location.X + (Form1.xoff * World.chunkSize))));
                int mouseu = Math.Max(0, Math.Min((int)(e.Location.Y + (Form1.yoff * World.chunkSize)), size * World.chunkSize));
                float elevation = Form1.world.WorldChunks[(mousex / World.chunkSize, mouseu / World.chunkSize)].ChunkBlock[(mousex - (mousex / World.chunkSize) * World.chunkSize,
                    mouseu - (mouseu / World.chunkSize) * World.chunkSize)].Z;
                MousePosLabel.Text = $"[{mousex}, {mouseu}] = {elevation.ToString().Substring(0, Math.Min(4, elevation.ToString().Length))}";
            }


        }

        private void TerrainBoxMouseUp(object sender, MouseEventArgs e)
        {
            if (MouseMode == MouseState.Editing)
            {
                Form1.world.UndoStack.Push(World.CloneWorld(Form1.world.temp));//undo stack.push -> temp.clone
                Form1.world.temp = World.CloneWorld(Form1.world.WorldChunks.Values.ToList()); //temp = current world,clone
            }
            
            Drag = DraggingState.None;
            TerrainBox.Cursor = Cursors.Default;

            
        }

        public void PopulateTerrainBox(object sender, PaintEventArgs e)
        {
            int startx = (int)Form1.xoff;
            int starty = (int)Form1.yoff;
            int endx = (int)(Form1.xoff + 24);
            int endy = (int)(Form1.yoff + 16);

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
                            e.Graphics.DrawImage(bmp, new PointF(World.chunkSize * (i - Form1.xoff), World.chunkSize * (j - Form1.yoff)));
                        }

                    }
                }

            }

        }
    }
}
