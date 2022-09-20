using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {
        int mHeight = 0;
        int mWidth = 0;

        // The universe array
        bool[,] universe = new bool[20, 20];
        bool[,] scratchPad = new bool[20, 20];


        // Drawing colors
        Color gridColor = Color.FromArgb(0, 0, 0);
        Color cellColor = Color.FromArgb(222, 184, 135);

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();
            // Setup the timer

            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }


        public void Randomize(bool[,] universe, int max)
        {
            Random randomNum = new Random();

            for (int x = 0; x < universe.GetLength(0); x++)
            {
                for (int y = 0; y < universe.GetLength(1); y++)
                {

                }
            }
        }


        // Calculate the next generation of cells
        private void NextGeneration()
        {
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int sum = CountNeighborsToroidal(x, y);

                    if (universe[x, y])
                    {
                        if (sum == 2 || sum == 3)
                            if (sum == 2 || sum == 3)
                            {
                                scratchPad[x, y] = true;
                            }

                        if (sum < 2 || sum > 3)
                        {
                            scratchPad[x, y] = false;
                        }
                    }

                    else
                    {
                        if (sum == 3)
                        {
                            scratchPad[x, y] = true;
                        }
                    }
                }
            }

            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;
            generations++;
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
            graphicsPanel1.Invalidate();
            Array.Clear(temp, 0, temp.Length);
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            NextGeneration();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    Rectangle cellRect = Rectangle.Empty;
                    cellRect.X = x * cellWidth;
                    cellRect.Y = y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int neighborcount = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }

                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }

                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }

                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }

                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
                    }

                    if (universe[xCheck, yCheck] == true)
                    {
                        neighborcount++;
                    }
                }
            }

            return neighborcount;
        }

        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                int cellWidth = graphicsPanel1.ClientSize.Width / universe.GetLength(0);
                int cellHeight = graphicsPanel1.ClientSize.Height / universe.GetLength(1);

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                int x = e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                int y = e.Y / cellHeight;

                // Toggle the cell's state
                universe[x, y] = !universe[x, y];

                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private bool[,] Restart()
        {
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = 0";
            timer.Enabled = false;

            var dim0 = universe.GetLength(0);
            var dim1 = universe.GetLength(1);

            var dim2 = scratchPad.GetLength(0);
            var dim3 = scratchPad.GetLength(1);

            universe = new bool[dim0, dim1];
            scratchPad = new bool[dim2, dim3];

            Refresh();
            return scratchPad;
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void toolStripButton2_Click_1(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void toolStripButton3_Click_1(object sender, EventArgs e)
        {
            NextGeneration();
            timer.Enabled = true;
            timer.Enabled = false;
            graphicsPanel1.Invalidate();

        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Restart();
        }

        private void x10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mWidth = 10;
            mHeight = 10;
            universe = new bool[mWidth, mHeight];
            scratchPad = new bool[mWidth, mHeight];
            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = 0";
            Refresh();
        }

        private void x20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mWidth = 20;
            mHeight = 20;
            universe = new bool[mWidth, mHeight];
            scratchPad = new bool[mWidth, mHeight];
            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = 0";
            Refresh();
        }

        private void x50ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mWidth = 50;
            mHeight = 50;
            universe = new bool[mWidth, mHeight];
            scratchPad = new bool[mWidth, mHeight];
            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = 0";
            Refresh();
        }

        private void x100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mWidth = 100;
            mHeight = 100;
            universe = new bool[mWidth, mHeight];
            scratchPad = new bool[mWidth, mHeight];
            timer.Enabled = false;
            generations = 0;
            toolStripStatusLabelGenerations.Text = "Generations = 0";
            Refresh();
        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ComboBox comboBox = new ComboBox();

            comboBox.Show();

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void darkModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.FromArgb(38, 28, 44);
            gridColor = Color.FromArgb(92, 82, 127);
            cellColor = Color.FromArgb(110, 133, 178);
            graphicsPanel1.Invalidate();
        }

        private void lightModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.FromArgb(232, 246, 239);
            gridColor = Color.FromArgb(39, 123, 192);
            cellColor = Color.FromArgb(184, 223, 216);
            graphicsPanel1.Invalidate();
        }

        private void matrixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.FromArgb(13, 2, 8);
            gridColor = Color.FromArgb(0, 59, 0);
            cellColor = Color.FromArgb(0, 255, 65);
            graphicsPanel1.Invalidate();
        }

        private void defaultToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.FromArgb(255, 255, 255);
            gridColor = Color.FromArgb(0, 0, 0);
            cellColor = Color.FromArgb(222, 184, 135);
            graphicsPanel1.Invalidate();
        }

        private void halloweenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.FromArgb(33, 23, 23);
            gridColor = Color.FromArgb(163, 74, 40);
            cellColor = Color.FromArgb(245, 139, 84);
            graphicsPanel1.Invalidate();
        }

        private void christmasToolStripMenuItem_Click(object sender, EventArgs e)
        {
            graphicsPanel1.BackColor = Color.FromArgb(165, 56, 66);
            gridColor = Color.FromArgb(66, 133, 91);
            cellColor = Color.FromArgb(92, 148, 55);
            graphicsPanel1.Invalidate();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string fileName = AppDomain.CurrentDomain.BaseDirectory + "GolPattern.txt";
            try
            {
                using (var writer = new StreamWriter(fileName))
                {
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        for (int z = 0; z < universe.GetLength(1); z++)
                        {

                            if (universe[x, z] == true)
                            {
                                writer.Write("O");
                            }
                            else
                            {
                                writer.Write(".");
                            }
                        }

                        writer.Write("\n");
                    }

                    writer.Flush();
                    writer.Close();
                    MessageBox.Show("File saved!");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("There was a problem saving your file.\n" + ex.Message);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "All Files|*.*|Text|*.txt";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;

                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row.StartsWith("!") == true)
                    {
                        continue;
                    }

                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    if (row.StartsWith("!") == false)
                    {
                        maxHeight++;
                    }

                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    maxWidth = row.Length;
                }

                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                universe = new bool[maxHeight, maxWidth];
                scratchPad = new bool[maxHeight, maxWidth];
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                maxHeight = 0;

                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!") == true)
                    {
                        continue;
                    }

                    // If the row is not a comment then 
                    // it is a row of cells and needs to be iterated through.
                    if (row.StartsWith("!") == false)
                    {
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if (row[xPos] == 'O')
                            {
                                universe[xPos, maxHeight] = true;
                            }

                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                            if (row[xPos] == '.')
                            {
                                universe[xPos, maxHeight] = false;
                            }

                        }
                    }

                    maxHeight++;
                }

                // Close the file.
                reader.Close();
            }

            graphicsPanel1.Invalidate();
        }

        // Save
        private void savePattern_Click(object sender, EventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;
            dlg.DefaultExt = "cells";


            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine("!This is my comment.");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(0); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(1); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y] == true)
                        {
                            currentRow += 'O';
                        }

                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else if (universe[x, y] == false)
                        {
                            currentRow += '.';
                        }
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }

                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string input = "...";
            ShowInputDialogBox(ref input, "Set grid size", "Grid size", 300, 200);
        }
        private static DialogResult ShowInputDialogBox(ref string input, string prompt, string title = "Title", int width = 300, int height = 200)
        {
            //This function creates the custom input dialog box by individually creating the different window elements and adding them to the dialog box

            //Specify the size of the window using the parameters passed
            Size size = new Size(width, height);
            //Create a new form using a System.Windows Form
            Form inputBox = new Form();

            inputBox.FormBorderStyle = FormBorderStyle.FixedDialog;
            inputBox.ClientSize = size;
            //Set the window title using the parameter passed
            inputBox.Text = title;

            //Create a new label to hold the prompt
            Label label = new Label();
            label.Text = prompt;
            label.Location = new Point(5, 5);
            label.Width = size.Width - 10;
            inputBox.Controls.Add(label);

            //Create a textbox to accept the user's input
            TextBox textBox = new TextBox();
            textBox.Size = new Size(size.Width / 3, 23);
            textBox.Location = new Point(5, label.Location.Y + 20);
            textBox.Text = input;
            inputBox.Controls.Add(textBox);

            TextBox textBox2 = new TextBox();
            textBox2.Size = new Size(size.Width / 3, 23);
            textBox2.Location = new Point(5, label.Location.Y + 40);
            textBox2.Text = input;
            inputBox.Controls.Add(textBox2);

            //Create an OK Button 
            Button okButton = new Button();
            okButton.DialogResult = DialogResult.OK;
            okButton.Name = "okButton";
            okButton.Size = new Size(75, 23);
            okButton.Text = "&OK";
            okButton.Location = new Point(size.Width - 80 - 80, size.Height - 30);
            inputBox.Controls.Add(okButton);

            //Create a Cancel Button
            Button cancelButton = new Button();
            cancelButton.DialogResult = DialogResult.Cancel;
            cancelButton.Name = "cancelButton";
            cancelButton.Size = new Size(75, 23);
            cancelButton.Text = "&Cancel";
            cancelButton.Location = new Point(size.Width - 80, size.Height - 30);
            inputBox.Controls.Add(cancelButton);

            //Set the input box's buttons to the created OK and Cancel Buttons respectively so the window appropriately behaves with the button clicks
            inputBox.AcceptButton = okButton;
            inputBox.CancelButton = cancelButton;

            //Show the window dialog box 
            DialogResult result = inputBox.ShowDialog();
            input = textBox.Text;

            //After input has been submitted, return the input value
            return result;
        }
    }
}
