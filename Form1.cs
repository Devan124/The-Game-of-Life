using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Game_of_Life
{
    public partial class Form1 : Form
    {

        // The universe array
        bool[,] universe = new bool[20, 20];
        bool[,] scratchPad = new bool[20, 20];

        // Drawing colors
        Color gridColor = Color.FromArgb(0, 0, 0);
        Color cellColor = Color.FromArgb(253, 217, 181);

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

        private int CountNeighborsFinite(int x, int y)
        {

            int count = 0;
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
                    if (xCheck < 0 || yCheck < 0)
                    {
                        continue;
                    }
                    if (xCheck >= xLen || yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true)
                    {
                        count++;
                    }
                }
            }
            return count;
        }

        private int CountNeighborsToroidal(int x, int y)
        {
            int count = 0;
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
                        count++;
                    }
                }
            }
            return count;
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void x10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe = new bool[10, 10];
            scratchPad = new bool[10, 10];
            timer.Enabled = false;
            generations = 0;
            Refresh();
        }

        private void x20ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe = new bool[20, 20];
            scratchPad = new bool[20, 20];
            timer.Enabled = false;
            generations = 0;
            Refresh();
        }

        private void x50ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe = new bool[50, 50];
            scratchPad = new bool[50, 50];
            timer.Enabled = false;
            generations = 0;
            Refresh();
        }

        private void x100ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            universe = new bool[100, 100];
            scratchPad = new bool[100, 100];
            timer.Enabled = false;
            generations = 0;
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
            cellColor = Color.FromArgb(253, 217, 181);
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

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog savefiledlg = new SaveFileDialog();
            savefiledlg.Filter = "All Files|*.*|Cells|*.cells";
            savefiledlg.FilterIndex = 2; savefiledlg.DefaultExt = "cells";

            if (DialogResult.OK == savefiledlg.ShowDialog())
            {
                StreamWriter writer = new StreamWriter(savefiledlg.FileName);

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
                            currentRow.Append('O');
                        }
                        else if (universe[x, y] == false)
                        {
                            currentRow.Append('.');
                        }
                    }
                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow.ToString());
                }
                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }
    }
}
