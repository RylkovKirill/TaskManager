using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Task_Manager
{
    public partial class PerformanceForm : MetroFramework.Forms.MetroForm
    {
        public PerformanceForm()
        {
            InitializeComponent();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            float cpu = CPU.NextValue();
            float ram = RAM.NextValue();
            metroProgressBar1.Value = (int)cpu;
            metroProgressBar2.Value = (int)ram;
            metroLabel3.Text = string.Format("{0:0.00}%", cpu);
            metroLabel4.Text = string.Format("{0:0.00}%", ram);
            chart1.Series["CPU"].Points.AddY(cpu);
            chart1.Series["RAM"].Points.AddY(ram);
        }

        private void PerformanceForm_Load(object sender, EventArgs e)
        {
            timer.Start();
        }
    }
}
