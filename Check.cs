using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TODOWorkers
{
    public partial class Check : Form
    {
        public Check()
        {
            InitializeComponent();

            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Form1 main = this.Owner as Form1;
            if (main != null)
            {
                string s = textBox1.Text;
                if(s == "pass")
                {
                    main.pass = true;


                }
                else
                {
                    main.pass = false;

                }
                this.Close();
            }


        }
    }
}
