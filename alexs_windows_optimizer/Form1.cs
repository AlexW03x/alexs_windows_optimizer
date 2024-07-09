using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MetroFramework.Components;
using MetroFramework.Forms;
using MetroFramework;
using System.Management;
using System.Security.Cryptography;

namespace alexs_windows_optimizer
{ 
    public partial class Form1 : MetroForm
    {

        public class Optimizer
        {
            public string[] pcInfo; //Name, CPU, GPU, OS, Paging Size

            public Optimizer() //Grabs system information first
            {
                this.pcInfo = new string[5];
                this.pcInfo[0] = Environment.UserDomainName;
                this.pcInfo[1] = grabDeviceInfo("Win32_Processor", "Name");
                this.pcInfo[2] = grabDeviceInfo("Win32_VideoController", "Description");
                this.pcInfo[3] = Environment.OSVersion + (Environment.Is64BitOperatingSystem == true ? " (64-Bit OS)" : " (32-Bit OS)");
                this.pcInfo[4] = Convert.ToString(Environment.SystemPageSize);
            }

            public string grabDeviceInfo(string searchModule, string searchRef)
            {
                string endValue = "?";
                try
                {
                    ManagementObjectSearcher MOS = new ManagementObjectSearcher("select * from " + searchModule);
                    foreach (ManagementObject o in MOS.Get())
                    {
                        if (o[searchRef] != null)
                        {
                            endValue = o[searchRef].ToString();
                            break;
                        }
                    }
                }
                catch (Exception ex)
                {

                }

                return endValue;
            }
        }

        public Form1()
        {
            InitializeComponent();
            Optimizer o = new Optimizer();
            updatePCLabels(o.pcInfo[0], o.pcInfo[1], o.pcInfo[2], o.pcInfo[3], o.pcInfo[4]);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {

        }

        private void metroLabel19_Click(object sender, EventArgs e)
        {

        }

        public void updatePCLabels(string r1, string r2, string r3, string r4, string r5)
        {
            metroLabel19.Text = r1;
            metroLabel20.Text = r2;
            metroLabel21.Text = r3;
            metroLabel22.Text = r4;
            metroLabel23.Text = r5;
        }
    }
}
