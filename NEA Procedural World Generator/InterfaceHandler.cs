﻿using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace NEA_Procedural_World_Generator
{
    //class to handle all aspects of the interface generation
    public class InterfaceHandler
    {
        //public variables
        public PictureBox MenuBox;
        public PictureBox TerrainBox;
        public NumericUpDown WorldSizeNUD;
        public Button PerlinGen;

        //private variables
        private Form1 Form;
        private Button StartButton;

        private int width;
        private int height;
        private PointF lastPos;


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
            //regenerate
            PerlinGen = ButtonCreator(PerlinWorldGen, new Point(0, MenuBox.Height - 30),
                "Perlin Generation", new Size(100, 30));

            //SLIDERS//
            //world sizez
            WorldSizeNUD = SliderCreator(new Point(60, 5), 24, 256, 0, 32, 1);
            LabelCreator(new Point(0, 5), "World Size:");


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

        private void PerlinWorldGen(object sender, EventArgs e)
        {
            Form1.world = new World((int)(WorldSizeNUD.Value));
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
            if (e.Button == MouseButtons.Right)
            {
                lastPos = e.Location;
                Drag = DraggingState.Dragging;
                TerrainBox.Cursor = Cursors.Hand;

            }
        }

        private void TerrainBoxMouseMove(object sender, MouseEventArgs e)
        {
            if (Drag == DraggingState.Dragging)
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
            }

        }

        private void TerrainBoxMouseUp(object sender, MouseEventArgs e)
        {
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
                        e.Graphics.DrawImage(Form1.world.WorldChunks[(indexi, indexj)].Bmp,
                            new PointF(World.chunkSize * (i - Form1.xoff), World.chunkSize * (j - Form1.yoff)));
                    }
                }

            }

        }
    }
}
