namespace NEA_Procedural_World_Generator
{
    partial class SettingsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.BackButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.colorDialog1 = new System.Windows.Forms.ColorDialog();
            this.ColourBox1 = new System.Windows.Forms.PictureBox();
            this.ColourBox2 = new System.Windows.Forms.PictureBox();
            this.ColourBox3 = new System.Windows.Forms.PictureBox();
            this.ColourBox4 = new System.Windows.Forms.PictureBox();
            this.ColourBox5 = new System.Windows.Forms.PictureBox();
            this.label7 = new System.Windows.Forms.Label();
            this.PresetButton1 = new System.Windows.Forms.Button();
            this.PresetButton2 = new System.Windows.Forms.Button();
            this.PresetButton3 = new System.Windows.Forms.Button();
            this.PresetButton4 = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox5)).BeginInit();
            this.SuspendLayout();
            // 
            // BackButton
            // 
            this.BackButton.Location = new System.Drawing.Point(0, 0);
            this.BackButton.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.BackButton.Name = "BackButton";
            this.BackButton.Size = new System.Drawing.Size(97, 21);
            this.BackButton.TabIndex = 0;
            this.BackButton.Text = "Save & Exit";
            this.BackButton.UseVisualStyleBackColor = true;
            this.BackButton.Click += new System.EventHandler(this.BackButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Colour Scheme:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 46);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(39, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Water:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 67);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "Grass:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 86);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 4;
            this.label4.Text = "Sand";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 107);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(31, 13);
            this.label5.TabIndex = 5;
            this.label5.Text = "Mud:";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 127);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(37, 13);
            this.label6.TabIndex = 6;
            this.label6.Text = "Snow:";
            // 
            // ColourBox1
            // 
            this.ColourBox1.Location = new System.Drawing.Point(49, 46);
            this.ColourBox1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ColourBox1.Name = "ColourBox1";
            this.ColourBox1.Size = new System.Drawing.Size(17, 16);
            this.ColourBox1.TabIndex = 7;
            this.ColourBox1.TabStop = false;
            this.ColourBox1.Click += new System.EventHandler(this.ColourBoxClick);
            // 
            // ColourBox2
            // 
            this.ColourBox2.Location = new System.Drawing.Point(49, 67);
            this.ColourBox2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ColourBox2.Name = "ColourBox2";
            this.ColourBox2.Size = new System.Drawing.Size(17, 16);
            this.ColourBox2.TabIndex = 8;
            this.ColourBox2.TabStop = false;
            this.ColourBox2.Click += new System.EventHandler(this.ColourBoxClick);
            // 
            // ColourBox3
            // 
            this.ColourBox3.Location = new System.Drawing.Point(49, 86);
            this.ColourBox3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ColourBox3.Name = "ColourBox3";
            this.ColourBox3.Size = new System.Drawing.Size(17, 16);
            this.ColourBox3.TabIndex = 9;
            this.ColourBox3.TabStop = false;
            this.ColourBox3.Click += new System.EventHandler(this.ColourBoxClick);
            // 
            // ColourBox4
            // 
            this.ColourBox4.Location = new System.Drawing.Point(49, 107);
            this.ColourBox4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ColourBox4.Name = "ColourBox4";
            this.ColourBox4.Size = new System.Drawing.Size(17, 16);
            this.ColourBox4.TabIndex = 10;
            this.ColourBox4.TabStop = false;
            this.ColourBox4.Click += new System.EventHandler(this.ColourBoxClick);
            // 
            // ColourBox5
            // 
            this.ColourBox5.Location = new System.Drawing.Point(49, 127);
            this.ColourBox5.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.ColourBox5.Name = "ColourBox5";
            this.ColourBox5.Size = new System.Drawing.Size(17, 16);
            this.ColourBox5.TabIndex = 11;
            this.ColourBox5.TabStop = false;
            this.ColourBox5.Click += new System.EventHandler(this.ColourBoxClick);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(93, 23);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(88, 26);
            this.label7.TabIndex = 12;
            this.label7.Text = "Pre-made Colour \r\nSchemes:";
            // 
            // PresetButton1
            // 
            this.PresetButton1.Location = new System.Drawing.Point(96, 56);
            this.PresetButton1.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PresetButton1.Name = "PresetButton1";
            this.PresetButton1.Size = new System.Drawing.Size(50, 19);
            this.PresetButton1.TabIndex = 13;
            this.PresetButton1.Text = "Lava";
            this.PresetButton1.UseVisualStyleBackColor = true;
            this.PresetButton1.Click += new System.EventHandler(this.GenericPresetPress);
            // 
            // PresetButton2
            // 
            this.PresetButton2.Location = new System.Drawing.Point(96, 79);
            this.PresetButton2.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PresetButton2.Name = "PresetButton2";
            this.PresetButton2.Size = new System.Drawing.Size(50, 19);
            this.PresetButton2.TabIndex = 14;
            this.PresetButton2.Text = "Purple";
            this.PresetButton2.UseVisualStyleBackColor = true;
            this.PresetButton2.Click += new System.EventHandler(this.GenericPresetPress);
            // 
            // PresetButton3
            // 
            this.PresetButton3.Location = new System.Drawing.Point(96, 101);
            this.PresetButton3.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PresetButton3.Name = "PresetButton3";
            this.PresetButton3.Size = new System.Drawing.Size(50, 19);
            this.PresetButton3.TabIndex = 15;
            this.PresetButton3.Text = "Yellow";
            this.PresetButton3.UseVisualStyleBackColor = true;
            this.PresetButton3.Click += new System.EventHandler(this.GenericPresetPress);
            // 
            // PresetButton4
            // 
            this.PresetButton4.Location = new System.Drawing.Point(96, 124);
            this.PresetButton4.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.PresetButton4.Name = "PresetButton4";
            this.PresetButton4.Size = new System.Drawing.Size(50, 19);
            this.PresetButton4.TabIndex = 16;
            this.PresetButton4.Text = "Natural";
            this.PresetButton4.UseVisualStyleBackColor = true;
            this.PresetButton4.Click += new System.EventHandler(this.GenericPresetPress);
            // 
            // SettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlLight;
            this.ClientSize = new System.Drawing.Size(333, 182);
            this.Controls.Add(this.PresetButton4);
            this.Controls.Add(this.PresetButton3);
            this.Controls.Add(this.PresetButton2);
            this.Controls.Add(this.PresetButton1);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.ColourBox5);
            this.Controls.Add(this.ColourBox4);
            this.Controls.Add(this.ColourBox3);
            this.Controls.Add(this.ColourBox2);
            this.Controls.Add(this.ColourBox1);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.BackButton);
            this.ForeColor = System.Drawing.Color.Black;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.Name = "SettingsForm";
            this.Text = "SettingsForm";
            this.Load += new System.EventHandler(this.SettingsForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.ColourBox5)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button BackButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ColorDialog colorDialog1;
        private System.Windows.Forms.PictureBox ColourBox1;
        private System.Windows.Forms.PictureBox ColourBox2;
        private System.Windows.Forms.PictureBox ColourBox3;
        private System.Windows.Forms.PictureBox ColourBox4;
        private System.Windows.Forms.PictureBox ColourBox5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button PresetButton1;
        private System.Windows.Forms.Button PresetButton2;
        private System.Windows.Forms.Button PresetButton3;
        private System.Windows.Forms.Button PresetButton4;
    }
}