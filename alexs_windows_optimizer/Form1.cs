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
using Microsoft.Win32;
using System.Security.AccessControl;

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

            public void windowsGameMode(bool toggle)
            {
                string path = @"Software\Microsoft\GameBar";
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
                {
                    if(key == null)
                    {
                        MessageBox.Show("Game Mode registry key doesn't exist or not found!", "Error!", MessageBoxButtons.OK);
                    }
                    key.SetValue("AllowAutoGameMode", toggle == true ? 1 : 0, RegistryValueKind.DWord);
                    key.SetValue("AutoGameModeEnabled", toggle == true ? 1 : 0, RegistryValueKind.DWord);
                }
            }

            public void gameBar(bool toggle)
            {
                setRegisterCU(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AllowGameDVR", toggle == true ? 0 : 1, RegistryValueKind.DWord);
                setRegisterCU(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCaptureEnabled", toggle == true ? 0 : 1, RegistryValueKind.DWord);
                setRegisterCU(@"System\GameConfigStore", "GameDVR_Enabled", toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void setRegisterCU(string path, string name, int value, RegistryValueKind type) //setting current user registry
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
                {
                    if(key == null)
                    {
                        MessageBox.Show("Registry key or path doesn't exist or error when finding!", "Error!", MessageBoxButtons.OK);
                    }
                    key.SetValue(name, value, type);
                }
            }

            public void setRegisterLM(string path, string name, int value, RegistryValueKind type) //setting local machine user registry
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path, true))
                {
                    if(key == null)
                    {
                        MessageBox.Show("Registry key or path doesn't exist or error when finding!", "Error!", MessageBoxButtons.OK);
                    }
                    key.SetValue(name, value, type);
                }
            }

            public int returnCurrentUserKeyValue(string path, string name)
            {
                int result = 0;
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
                    {
                        if(key != null)
                        {
                            if(key.GetValue(name) != null)
                            {
                                result = Convert.ToInt16(key.GetValue(name).ToString()); //turn object to string to get value then convert to int
                            }
                        }
                    }
                }
                catch(Exception ex)
                {

                }
                //MessageBox.Show(result.ToString()); debug reg values
                return result;
            }

            public int returnLocalKeyValue(string path, string name)
            {
                int result = 0;
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
                    {
                        if(key != null)
                        {
                            if(key.GetValue(name) != null)
                            {
                                result = Convert.ToInt16(key.GetValue(name).ToString());
                            }
                        }
                    }
                }
                catch(Exception ex)
                {

                }
                //MessageBox.Show(result.ToString());
                return result;
            }
        }

        public Optimizer o = new Optimizer();
        public Form1()
        {
            InitializeComponent();
            updatePCLabels(o.pcInfo[0], o.pcInfo[1], o.pcInfo[2], o.pcInfo[3], o.pcInfo[4]);
            metroToggle1.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\GameBar", "AllowAutoGameMode") == 1 && 
                o.returnCurrentUserKeyValue(@"Software\Microsoft\GameBar", "AutoGameModeEnabled") == 1 ? true : false;
            metroToggle2.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AllowGameDVR") == 0 &&
                o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCatureEnabled") == 0 &&
                o.returnCurrentUserKeyValue(@"System\GameConfigStore", "GameDVR_Enabled") == 0 ? true : false;
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void metroButton1_Click(object sender, EventArgs e)
        {
            DialogResult msg = MessageBox.Show("These changes can impact your computer system!\nWould you like to proceed?", "Warning", MessageBoxButtons.YesNo);
            if (msg == DialogResult.Yes)
            {
                o.windowsGameMode(metroToggle1.Checked);
                o.gameBar(metroToggle2.Checked);
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

        private void metroToggle1_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
