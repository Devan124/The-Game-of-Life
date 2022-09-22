using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Game_of_Life
{
    public partial class ModalDialog : Form
    {
        public int mySeed;
        public ModalDialog()
        {
            InitializeComponent();
        }

        private void ModalDialog_Load(object sender, EventArgs e)
        {

        }

        private void button3_Click(object sender, EventArgs e)
        {
            SeedNumeric.Minimum = -9999999;
            SeedNumeric.Maximum =  9999999;
            Random rng = new Random();
            SeedNumeric.Value = rng.Next(-10000, 10000);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int Picked = (int)SeedNumeric.Value;
            mySeed = Picked;
        }
        public void setMyNumber(int num)
        {
            mySeed = num;
        }
    }
}
