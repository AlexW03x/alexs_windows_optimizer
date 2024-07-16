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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

namespace alexs_windows_optimizer
{ 
    public partial class Form1 : MetroForm
    {

        public class Optimizer
        {
            public string[] pcInfo; //Name, CPU, GPU, OS, Paging Size
            public string serviceLocation = @"SYSTEM\CurrentControlSet\Services\";


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

            public void xboxLive(bool toggle)
            {
                setRegisterLM(serviceLocation + "XblAuthManager", "Start", toggle == true ? 4 : 3, RegistryValueKind.DWord);
                setRegisterLM(serviceLocation + "XblGameSave", "Start", toggle == true ? 4 : 3, RegistryValueKind.DWord);
                setRegisterLM(serviceLocation + "xboxgip", "Start", toggle == true ? 4 : 3, RegistryValueKind.DWord);
                setRegisterLM(serviceLocation + "XboxGipSvc", "Start", toggle == true ? 4 : 3, RegistryValueKind.DWord);
                setRegisterLM(serviceLocation + "XboxNetApiSvc", "Start", toggle == true ? 4 : 3, RegistryValueKind.DWord);
            }

            public string executeCommandWithOutput(string command)
            {
                string output = "";
                try
                {
                    string commandPromptPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                         Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess ?
                        @"Sysnative\cmd.exe" : @"System32\cmd.exe");
                    Process cmd = new Process();
                    ProcessStartInfo info = new ProcessStartInfo()
                    {
                        FileName = commandPromptPath,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        Arguments = "/c " + command,
                        Verb = "runas"
                    };

                    cmd.StartInfo = info;
                    cmd.Start();

                    output = "Output: \n" + cmd.StandardOutput.ReadToEnd() + "\n\nErrors: \n" + cmd.StandardError.ReadToEnd();
                    cmd.WaitForExit();
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Command execution failed for the following command:\n{command}", "Error!", MessageBoxButtons.OK);
                }

                return output;
            }

            public void eventTimer(bool toggle)
            {
                try
                {
                    string commandPromptPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Windows),
                        Environment.Is64BitOperatingSystem && !Environment.Is64BitProcess ?
                        @"Sysnative\cmd.exe" : @"System32\cmd.exe");
                    string argument = toggle == true ? "bcdedit /deletevalue useplatformclock" : "bcdedit /set useplatformclock true";
                    Process cmd = new Process();
                    ProcessStartInfo info = new ProcessStartInfo()
                    {
                        FileName = commandPromptPath,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        UseShellExecute = false,
                        Arguments = "/c " + argument,
                        Verb = "runas"
                    };

                    cmd.StartInfo = info;
                    cmd.Start();

                    /*string output = cmd.StandardOutput.ReadToEnd();
                    string errors = cmd.StandardError.ReadToEnd();
                    cmd.WaitForExit();
                    MessageBox.Show($"Output: {output}\n\nError: {errors}", "Output", MessageBoxButtons.OK);   DEBUG COMMANDLINE   */

                    using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Software\AWO_Optimizer")) //so that the toggle can be saved for next launch
                    {
                        if(registryKey != null)
                        {
                            
                        }
                        registryKey.SetValue("eventTimerOn", toggle == true ? 0 : 1, RegistryValueKind.DWord);
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Error when disabling/enabling High Precision Event Timer!", "Error!", MessageBoxButtons.OK);
                }
            }

            public void coreIsolation(bool toggle)
            {
                try
                {
                    using(RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity"))
                    {
                        if(key != null)
                        {
                            setRegisterLM(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled",
                                toggle == true ? 0 : 1, RegistryValueKind.DWord);
                        }
                        else
                        {
                            MessageBox.Show("Core Isolation not found on your device\n\nDisabled by default!", "Not Found!", MessageBoxButtons.OK);
                        }
                    }
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Core Isolation not found on your device\nYour device may not support it!", "Error", MessageBoxButtons.OK);
                }
            }

            public string getPlanID(string name)
            {
                string list = executeCommandWithOutput("powercfg /list");
                string[] listToArray = list.Split();
                for(int i = 0; i < listToArray.Length; i++)
                {
                    //MessageBox.Show($"Value {i}: \n{listToArray[i]}");    Debug List Print Box
                    if (listToArray[i] == name || listToArray[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return listToArray[i-2]; //space between the ID and the Name so -2 instead of -1
                    }
                }

                return null;
            }

            public void ultimatePower(bool toggle)
            {
                string doPower = "";
                if(executeCommandWithOutput("powercfg /list").ToString().Contains("Ultimate Performance"))
                {
                    if (toggle == true)
                    {
                        string id = getPlanID("(Ultimate");
                        doPower = executeCommandWithOutput("powercfg /setactive " + id);
                    }
                    else
                    {
                        doPower = executeCommandWithOutput("powercfg /setactive 381b4222-f694-41f0-9685-ff5bb260df2e");
                    }
                }
                else
                {
                    if (toggle == true)
                    {
                        doPower = executeCommandWithOutput("powercfg -duplicatescheme e9a42b02-d5df-448d-aa00-03f14749eb61");
                        string id = getPlanID("(Ultimate");
                        doPower = executeCommandWithOutput("powercfg /setactive " + id);
                    }
                    else
                    {
                        doPower = executeCommandWithOutput("powercfg /setactive 381b4222-f694-41f0-9685-ff5bb260df2e");
                    }
                }
            }

            public void performanceVisuals(bool toggle)
            {
                //0 = Let Windows Choose, 1 = Best Appearance, 2 = Best Performance, 3 = Custom
                setRegisterCU(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting", toggle == true ? 3 : 1, RegistryValueKind.DWord);

                //items to change - utilizing my visual effect settings as slightly improved windows performance
                setRegisterCU(@"Control Panel\Desktop\WindowMetrics", "MinAnimate", toggle == true ? "0" : "1", RegistryValueKind.String);
                setRegisterCU(@"Control Panel\Desktop", "DragFullWindows", toggle == true ? "1" : "1", RegistryValueKind.String);
                setRegisterCU(@"Control Panel\Desktop", "FontSmoothing", toggle == true ? "2" : "2", RegistryValueKind.String);
                string[] advanced =
                {
                    "ListviewAlphaSelect",
                    "TaskbarAnimations",
                    "ListviewShadow",
                    "IconsOnly"
                };
                foreach(string name in advanced)
                {
                    setRegisterCU(@"Software\Microsoft\Windows\CurrentVersion\Explorer\Advanced", name, toggle == true ? 0 :
                        toggle == false && name != "IconsOnly" ? 1 : 0, RegistryValueKind.DWord);
                }
                setRegisterCU(@"Software\Microsoft\Windows\DWM", "EnableAeroPeek", toggle == true ? 0 : 1, RegistryValueKind.DWord);
                setRegisterCU(@"Software\Microsoft\Windows\DWM", "AlwaysHibernateThumbnails", toggle == true ? 0 : 1, RegistryValueKind.DWord);
                setRegisterCU(@"Software\Microsoft\Windows\DWM", "UseDropShadow", toggle == true ? 0 : 1, RegistryValueKind.DWord);
                setRegisterCU(@"Control Panel\Mouse", "MouseHoverWidth", toggle == true ? "0" : "4", RegistryValueKind.String);
                setRegisterCU(@"Control Panel\Desktop", "UserPreferencesMask", toggle == true ?
                    new byte[] { 0x90, 0x12, 0x03, 0x80, 0x10, 0x00, 0x00, 0x00 } : 
                    new byte[] { 0x9e, 0x3e, 0x07, 0x80, 0x12, 0x00, 0x00, 0x00 }, RegistryValueKind.Binary);
            }

            public void gpuScheduling(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "HwSchMode", toggle == true ? 2 : 1, RegistryValueKind.DWord);
            }

            public void notifications(bool toggle)
            {
                setRegisterCU(@"Software\Microsoft\Windows\CurrentVersion\PushNotifications", "ToastEnabled", toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void cpuParking(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583",
                    "Attributes", toggle == true ? 0 : 1, RegistryValueKind.DWord);
                setRegisterLM(@"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583",
                    "ValueMax", toggle == true ? 0 : 64, RegistryValueKind.DWord);
            }

            public void cpuThrottling(bool toggle)
            {
                using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling", true))
                {
                    if(key != null)
                    {

                    }
                    key.SetValue("PowerThrottlingOff", toggle == true ? 1 : 0, RegistryValueKind.DWord);
                }
            }

            public void driverSearch(bool toggle)
            {
                setRegisterLM(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", "SearchOrderConfig", toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void setRegisterCU(string path, string name, object value, RegistryValueKind type) //setting current user registry
            {
                using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
                {
                    if(key == null)
                    {
                        MessageBox.Show($"{path}+/{name} doesn't exist or could not be found!\n\nYour system may not support this feature!", "Error!", MessageBoxButtons.OK);
                    }
                    key.SetValue(name, value, type);
                }
            }


            public void setRegisterLM(string path, string name, object value, RegistryValueKind type) //setting local machine user registry
            {
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path, true))
                {
                    if(key == null)
                    {
                        MessageBox.Show($"{path}+/{name} doesn't exist or could not be found!\n\nYour system may not support this feature!", "Error!", MessageBoxButtons.OK);
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

            //Showcase toggle settings from analysing current PC settings
            metroToggle1.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\GameBar", "AllowAutoGameMode") == 1 && 
                o.returnCurrentUserKeyValue(@"Software\Microsoft\GameBar", "AutoGameModeEnabled") == 1 ? true : false;

            metroToggle2.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AllowGameDVR") == 0 &&
                o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCatureEnabled") == 0 &&
                o.returnCurrentUserKeyValue(@"System\GameConfigStore", "GameDVR_Enabled") == 0 ? true : false;

            metroToggle3.Checked = o.returnLocalKeyValue(o.serviceLocation + "XblAuthManager", "Start") == 4 &&
                o.returnLocalKeyValue(o.serviceLocation + "XblGameSave", "Start") == 4 &&
                o.returnLocalKeyValue(o.serviceLocation + "xboxgip", "Start") == 4 &&
                o.returnLocalKeyValue(o.serviceLocation + "XboxGipSvc", "Start") == 4 &&
                o.returnLocalKeyValue(o.serviceLocation + "XboxNetApiSvc", "Start") == 4 ? true : false;

            metroToggle4.Checked = o.returnCurrentUserKeyValue(@"Software\AWO_Optimizer", "eventTimerOn") == 0 ? true : false;
            metroToggle5.Checked = o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity", "Enabled") == 0 ? true : false;
            metroToggle6.Checked = (o.executeCommandWithOutput(" powercfg /getactivescheme").ToString().Contains("Ultimate Performance")) ? true : false;

            metroToggle7.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting") == 3 ? true : false;
            metroToggle8.Checked = o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "HwSchMode") == 2 ? true : false;
            metroToggle9.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\PushNotifications", "ToastEnabled") == 0 ? true : false;

            metroToggle10.Checked = o.returnLocalKeyValue(
                @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583",
                 "Attributes") == 0 && o.returnLocalKeyValue(
                @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583", 
                "ValueMax") == 0 ? true : false;

            metroToggle11.Checked = o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling", "PowerThrottlingOff") == 1 ? true : false;
            metroToggle12.Checked = o.returnLocalKeyValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", "SearchOrderConfig") == 0 ? true : false;
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
                o.xboxLive(metroToggle3.Checked);
                o.eventTimer(metroToggle4.Checked);
                o.coreIsolation(metroToggle5.Checked);

                o.ultimatePower(metroToggle6.Checked);
                o.performanceVisuals(metroToggle7.Checked);
                o.gpuScheduling(metroToggle8.Checked);
                o.notifications(metroToggle9.Checked);
                o.cpuParking(metroToggle10.Checked);

                o.cpuThrottling(metroToggle11.Checked);
                o.driverSearch(metroToggle12.Checked);
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

        private void metroToggle4_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle5_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle6_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle7_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle8_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle9_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle10_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle11_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle12_CheckedChanged(object sender, EventArgs e)
        {

        }
    }
}
