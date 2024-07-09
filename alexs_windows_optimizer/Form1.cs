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
                this.pcInfo[2] = grabDeviceInfo("Win32_VideoController", "Name");
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
            DialogResult msg = MessageBox.Show("These changes can impact your computer system!\nWould you like to proceed?", "Warning", MessageBoxButtons.YesNo);
            if (msg == DialogResult.Yes)
            {

            }
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

        private void metroButton2_Click(object sender, EventArgs e)
        {
            DialogResult msg = MessageBox.Show("Are you sure you want to create a system restore point?", "Confirm Restore Point", MessageBoxButtons.YesNo);
            if(msg == DialogResult.Yes)
            {
                try
                {
                    ManagementClass mngmt = new ManagementClass("\\\\.\\root\\default:SystemRestore");
                    ManagementBaseObject obj = mngmt.GetMethodParameters("CreateRestorePoint");
                    obj["Description"] = "AWO_RestorePoints_" + Convert.ToString(DateTime.Now);
                    obj["RestorePointType"] = 12; //Settings
                    obj["EventType"] = 100; //Changes to System

                    ManagementBaseObject createRestore = mngmt.InvokeMethod("CreateRestorePoint", obj, null);
                    if (Convert.ToInt16(createRestore.Properties["ReturnValue"].Value) != 0)
                    {
                        MessageBox.Show("Unable to create a system restore point!\nPlease try again or create one manually!", "Error!", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("System restore point created successfully!", "Success!", MessageBoxButtons.OK);
                    }
                }
                catch(ManagementException ex)
                {
                    MessageBox.Show("Exception Occured!\nPlease try again or create a restore point manually!", "Error!", MessageBoxButtons.OK);
                }
            }

        }
    }
}
