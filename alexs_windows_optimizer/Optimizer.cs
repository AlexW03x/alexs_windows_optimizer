using System;
using System.Linq;
using System.Windows.Forms;
using System.Management;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Net.NetworkInformation;

namespace alexs_windows_optimizer
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
                    if (key == null)
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

            public string executePowershellWithOutput(string command)
            {
                string output = "";
                try
                {
                    string path = Path.Combine(Environment.GetFolderPath(
                        Environment.SpecialFolder.System), @"WindowsPowerShell\v1.0\powershell.exe");
                    Process ps = new Process();
                    ProcessStartInfo info = new ProcessStartInfo()
                    {
                        FileName = path,
                        RedirectStandardError = true,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true,
                        Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                        Verb = "runas"
                    };

                    ps.StartInfo = info;
                    ps.Start();

                    output = "Output:\n" + ps.StandardOutput.ReadToEnd() + "\n\nErrors: " + ps.StandardError.ReadToEnd();
                    ps.WaitForExit();
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
                        if (registryKey != null)
                        {

                        }
                        registryKey.SetValue("eventTimerOn", toggle == true ? 0 : 1, RegistryValueKind.DWord);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error when disabling/enabling High Precision Event Timer!", "Error!", MessageBoxButtons.OK);
                }
            }

            public void coreIsolation(bool toggle)
            {
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity"))
                    {
                        if (key != null)
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
                catch (Exception ex)
                {
                    MessageBox.Show("Core Isolation not found on your device\nYour device may not support it!", "Error", MessageBoxButtons.OK);
                }
            }

            public string getPlanID(string name)
            {
                string list = executeCommandWithOutput("powercfg /list");
                string[] listToArray = list.Split();
                for (int i = 0; i < listToArray.Length; i++)
                {
                    //MessageBox.Show($"Value {i}: \n{listToArray[i]}");    Debug List Print Box
                    if (listToArray[i] == name || listToArray[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                    {
                        return listToArray[i - 2]; //space between the ID and the Name so -2 instead of -1
                    }
                }

                return null;
            }

            public void ultimatePower(bool toggle)
            {
                string doPower = "";
                if (executeCommandWithOutput("powercfg /list").ToString().Contains("Ultimate Performance"))
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
                foreach (string name in advanced)
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
                    if (key != null)
                    {

                    }
                    key.SetValue("PowerThrottlingOff", toggle == true ? 1 : 0, RegistryValueKind.DWord);
                }
            }

            public void driverSearch(bool toggle)
            {
                setRegisterLM(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", "SearchOrderConfig",
                    toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void networkThrottling(bool toggle)
            {
                setRegisterLM(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "NetworkThrottlingIndex",
                    toggle == true ? 0xFFFFFFFF : 0xFFFFFFFF, RegistryValueKind.DWord);
            }

            public void systemResponsiveness(bool toggle)
            {
                setRegisterLM(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile", "SystemResponsiveness",
                    toggle == true ? 0 : 0, RegistryValueKind.DWord);
            }

            public void gamingFrequencies(bool toggle)
            {
                foreach (NetworkInterface netIDs in NetworkInterface.GetAllNetworkInterfaces())
                {
                    setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + netIDs.Id, "TcpAckFrequency",
                        toggle == true ? 1 : 0, RegistryValueKind.DWord);
                }
            }

            public void noDelay(bool toggle)
            {
                foreach (NetworkInterface netIDs in NetworkInterface.GetAllNetworkInterfaces())
                {
                    setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + netIDs.Id, "TCPNoDelay",
                        toggle == true ? 1 : 0, RegistryValueKind.DWord);
                }
            }

            public void delayTicks(bool toggle)
            {
                foreach (NetworkInterface netIDs in NetworkInterface.GetAllNetworkInterfaces())
                {
                    setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + netIDs.Id, "TcpDelAckTicks",
                        toggle == true ? 0 : 0, RegistryValueKind.DWord);
                }
            }

            public string getActiveNetworkID()
            {
                foreach (NetworkInterface netIDs in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netIDs.OperationalStatus == OperationalStatus.Up)
                    {
                        var ipAddress = netIDs.GetIPProperties().UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily ==
                            System.Net.Sockets.AddressFamily.InterNetwork);

                        if (ipAddress != null)
                        {
                            return netIDs.Id;
                        }
                    }
                }

                return null;
            }

            public string getActiveNetworkName()
            {
                foreach (NetworkInterface netIDs in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netIDs.OperationalStatus == OperationalStatus.Up)
                    {
                        var ipAddress = netIDs.GetIPProperties().UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily ==
                            System.Net.Sockets.AddressFamily.InterNetwork);

                        if (ipAddress != null)
                        {
                            return netIDs.Name;
                        }
                    }
                }

                return null;
            }

            public string getActiveNetworkDesc()
            {
                foreach (NetworkInterface netIDs in NetworkInterface.GetAllNetworkInterfaces())
                {
                    if (netIDs.OperationalStatus == OperationalStatus.Up)
                    {
                        var ipAddress = netIDs.GetIPProperties().UnicastAddresses.FirstOrDefault(ip => ip.Address.AddressFamily ==
                            System.Net.Sockets.AddressFamily.InterNetwork);

                        if (ipAddress != null)
                        {
                            return netIDs.Description;
                        }
                    }
                }

                return null;
            }

            public void setTCPTuning(bool toggle)
            {
                string execute = executeCommandWithOutput("netsh int tcp set global autotuning=" +
                    (toggle == true ? "disabled" : "normal"));
            }

            public bool getNetInfo(string command, string key, string value)
            {
                string tcp = executeCommandWithOutput(command);

                string lookup = @"" + key + @"\s*:\s*(\w+)";
                Match str = Regex.Match(tcp, lookup, RegexOptions.IgnoreCase);

                if (str.Success)
                {
                    return Convert.ToString(str.Groups[1].Value).Equals(value, StringComparison.OrdinalIgnoreCase);
                }

                return false;
            }

            public void setHeuristics(bool toggle)
            {
                string execute = executeCommandWithOutput("netsh int tcp set heuristics " +
                    (toggle == true ? "disabled" : "enabled"));
            }

            public void setCongestion(bool toggle)
            {
                string execute = executePowershellWithOutput("netsh int tcp set supplemental template=custom congestionprovider=" +
                    (toggle == true ? "ctcp" : "default"));
            }

            public void setRSSandRSC(bool toggle)
            {
                string executeRSS = executeCommandWithOutput("netsh int tcp set global rss=" +
                    (toggle == true ? "enabled" : "disabled"));

                string executeRSC = executeCommandWithOutput("netsh int tcp set global rsc=" +
                    (toggle == true ? "disabled" : "enabled"));
            }

            public void setTTL(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DefaultTTL",
                    toggle == true ? 64 : 64, RegistryValueKind.DWord);
            }

            public void setECN(bool toggle)
            {
                string executeECN = executePowershellWithOutput("netsh int tcp set global ecncapability=" +
                    (toggle == true ? "enabled" : "disabled"));
            }

            public void setChimneyOffload(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DisableTaskOffload",
                    toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void setTCPChimney(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "EnableTCPChimney",
                    toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void setLargeOffload(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + getActiveNetworkID(), "*LsoV2IPv4",
                    toggle == true ? 0 : 1, RegistryValueKind.DWord);
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + getActiveNetworkID(), "*LsoV2IPv6",
                    toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void setTimestamps(bool toggle)
            {
                string executeTS = executePowershellWithOutput("netsh int tcp set global timestamps=" +
                    (toggle == true ? "enabled" : "disabled"));
            }

            public void setMaxConnections(bool toggle)
            {
                setRegisterCU(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPerServer",
                    toggle == true ? 10 : 10, RegistryValueKind.DWord);
                setRegisterCU(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPer1_0Server",
                    toggle == true ? 10 : 10, RegistryValueKind.DWord);
            }

            public void setHostPriorities(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider", "LocalPriority",
                    toggle == true ? 4 : 499, RegistryValueKind.DWord);
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider", "HostPriority",
                    toggle == true ? 5 : 500, RegistryValueKind.DWord);
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider", "DnsPriority",
                    toggle == true ? 6 : 2000, RegistryValueKind.DWord);
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider", "NetbtPriority",
                    toggle == true ? 7 : 2001, RegistryValueKind.DWord);
            }

            public void setTransmissions(bool toggle)
            {
                string executeSYN = executePowershellWithOutput("netsh int tcp set global maxsynretransmissions=" +
                    (toggle == true ? "2" : "4"));
                string executeRTT = executePowershellWithOutput("netsh int tcp set global nonsackrttresiliency=" +
                    (toggle == true ? "disabled" : "enabled"));
            }

            public void setQoS(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\nlasvc", "Start",
                    toggle == true ? 4 : 2, RegistryValueKind.DWord);
                setRegisterLM(@"SOFTWARE\Policies\Microsoft\Windows\Psched", "NonBestEffortLimit",
                    toggle == true ? 0 : 80, RegistryValueKind.DWord);
            }

            public void setNetworkAllocations(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "LargeSystemCache",
                    toggle == true ? 1 : 0, RegistryValueKind.DWord);
            }

            public void setPorts(bool toggle)
            {
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "MaxUserPort",
                    toggle == true ? 65534 : 65534, RegistryValueKind.DWord);
                setRegisterLM(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "TcpTimedWaitDelay",
                    toggle == true ? 30 : 30, RegistryValueKind.DWord);
            }

            public void disableAdvertID(bool toggle)
            {
                setRegisterCU(@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled",
                    toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void disableDiagnostics(bool toggle)
            {
                setRegisterLM(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry",
                    toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void disableAdvertising(bool toggle)
            {
                setRegisterCU(@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled",
                    toggle == true ? 0 : 1, RegistryValueKind.DWord);
                setRegisterLM(@"SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled",
                    toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void disableHandwriting(bool toggle)
            {
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"Software\Policies\Microsoft\Windows\TabletPC", true))
                    {
                        if(key != null)
                        {

                        }
                        key.SetValue("PreventHandwritingDataSharing", toggle == true ? 1 : 0, RegistryValueKind.DWord);
                    }
                }
                catch(Exception e)
                {

                }
            }

            public void disableTypingTransmission(bool toggle)
            {
                setRegisterCU(@"Software\Microsoft\Input\TIPC", "Enabled",
                toggle == true ? 0 : 1, RegistryValueKind.DWord);
            }

            public void disableAppTracking(bool toggle)
            {
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\EdgeUI", true))
                    {
                        if(key != null)
                        {

                        }
                        key.SetValue("DisableMFUTracking", toggle == true ? 1 : 0, RegistryValueKind.DWord);
                    }

                    using (RegistryKey key = Registry.CurrentUser.CreateSubKey(@"Software\Policies\Microsoft\Windows\EdgeUI", true))
                    {
                        if(key != null)
                        {
                            
                        }
                        key.SetValue("DisableMFUTracking", toggle == true ? 1 : 0, RegistryValueKind.DWord);
                    }
                }
                catch(Exception ex)
                {

                }
            }

        public void disableUserActivity(bool toggle)
        {
            setRegisterLM(@"SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities",
                toggle == true ? 0 : 1, RegistryValueKind.DWord);
        }

        public void disableSuggestions(bool toggle)
        {
            setRegisterCU(@"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SubscribedContent-338389Enabled",
                toggle == true ? 0 : 1, RegistryValueKind.DWord);
            setRegisterCU(@"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SystemPaneSuggestionsEnabled",
                toggle == true ? 0 : 1, RegistryValueKind.DWord);
            setRegisterCU(@"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager", "SoftLandingEnabled",
                toggle == true ? 0 : 1, RegistryValueKind.DWord);
        }

        public void disableClipboardHistory(bool toggle)
        {
            setRegisterLM(@"SOFTWARE\Policies\Microsoft\Windows\System", "AllowClipboardHistory",
                toggle == true ? 0 : 1, RegistryValueKind.DWord);
        }

            public void setRegisterCU(string path, string name, object value, RegistryValueKind type) //setting current user registry
            {
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path, true))
                    {
                        if (key == null)
                        {
                            MessageBox.Show($"{path}->{name} doesn't exist or could not be found!\n\nYour system may not support this feature!", "Error!", MessageBoxButtons.OK);
                        }
                        key.SetValue(name, value, type);
                    }
                }
                catch (Exception ex)
                {

                }
            }


            public void setRegisterLM(string path, string name, object value, RegistryValueKind type) //setting local machine user registry
            {
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path, true))
                    {
                        if (key == null)
                        {
                            MessageBox.Show($"{path}->{name} doesn't exist or could not be found!\n\nYour system may not support this feature!", "Error!", MessageBoxButtons.OK);
                        }
                        key.SetValue(name, value, type);
                    }
                }
                catch (Exception ex)
                {

                }
            }

            public int returnCurrentUserKeyValue(string path, string name)
            {
                int result = 0;
                try
                {
                    using (RegistryKey key = Registry.CurrentUser.OpenSubKey(path))
                    {
                        if (key != null)
                        {
                            if (key.GetValue(name) != null)
                            {
                                result = Convert.ToInt16(key.GetValue(name).ToString()); //turn object to string to get value then convert to int
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                //MessageBox.Show(result.ToString()); debug reg values
                return result;
            }

            public object returnLocalKeyValue(string path, string name)
            {
                object result = null;
                try
                {
                    using (RegistryKey key = Registry.LocalMachine.OpenSubKey(path))
                    {
                        if (key != null)
                        {
                            if (key.GetValue(name) != null)
                            {
                                result = key.GetValue(name);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {

                }
                //MessageBox.Show(result.ToString());
                return result;
            }
        }
    }
