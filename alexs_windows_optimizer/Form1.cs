using System;
using System.Windows.Forms;
using MetroFramework.Forms;
using System.Management;
using System.Diagnostics;
using System.IO;

namespace alexs_windows_optimizer
{ 
    public partial class Form1 : MetroForm
    {

        public Optimizer o = new Optimizer();
        public Form1()
        {
            InitializeComponent();
            updatePCLabels(o.pcInfo[0], o.pcInfo[1], o.pcInfo[2], o.pcInfo[3], o.pcInfo[4]);
            string language = "";
            try
            {
                if (!File.Exists("awo.ini"))
                {
                    using (StreamWriter settings = new StreamWriter("awo.ini", false)) //do not append so that languages are saved
                    {
                        settings.WriteLine("Language=English");
                    }
                }

                using (StreamReader lang = new StreamReader("awo.ini"))
                {
                    language = lang.ReadToEnd();
                }

                language = language.Contains("English") ? "English" :
                    language.Contains("Français") ? "Français" :
                    language.Contains("Español") ? "Español" :
                    "English";
            }
            catch(Exception ex)
            {
                
            }

            //set the metroLanguageDropDown content
            metroComboBox1.SelectedItem = language;
            //set the language
            changeLanguage(language);

            //Showcase toggle settings from analysing current PC settings
            updateToggles();
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
                o.networkThrottling(metroToggle13.Checked);
                o.systemResponsiveness(metroToggle14.Checked);
                o.gamingFrequencies(metroToggle15.Checked);

                o.noDelay(metroToggle16.Checked);
                o.delayTicks(metroToggle17.Checked);
            }
        }

        private void metroButton3_Click(object sender, EventArgs e)
        {
            DialogResult msg = MessageBox.Show("These changes can impact your computer system!\nWould you like to proceed?", "Warning", MessageBoxButtons.YesNo);
            if (msg == DialogResult.Yes)
            {
                o.setTCPTuning(metroToggle18.Checked);
                o.setHeuristics(metroToggle19.Checked);
                o.setCongestion(metroToggle20.Checked);
                o.setRSSandRSC(metroToggle21.Checked);
                o.setTTL(metroToggle22.Checked);

                o.setECN(metroToggle23.Checked);
                o.setChimneyOffload(metroToggle24.Checked);
                o.setTCPChimney(metroToggle25.Checked);
                o.setLargeOffload(metroToggle26.Checked);
                o.setTimestamps(metroToggle27.Checked);

                o.setMaxConnections(metroToggle28.Checked);
                o.setHostPriorities(metroToggle29.Checked);
                o.setTransmissions(metroToggle30.Checked);
                o.setQoS(metroToggle31.Checked);
                o.setNetworkAllocations(metroToggle32.Checked);
                o.setPorts(metroToggle33.Checked);
            }
        }
        private void metroButton6_Click(object sender, EventArgs e)
        {
            DialogResult msg = MessageBox.Show("These changes can impact your computer system!\nWould you like to proceed?", "Warning", MessageBoxButtons.YesNo);
            if (msg == DialogResult.Yes)
            {
                o.disableAdvertID(metroToggle34.Checked);
                o.disableDiagnostics(metroToggle35.Checked);
                o.disableAdvertising(metroToggle36.Checked);
                o.disableHandwriting(metroToggle37.Checked);
                o.disableTypingTransmission(metroToggle38.Checked);
                o.disableAppTracking(metroToggle39.Checked);
                o.disableUserActivity(metroToggle40.Checked);
                o.disableSuggestions(metroToggle41.Checked);
            }
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

        private void metroComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                string lang = metroComboBox1.SelectedIndex == 0 ? "English" :
                    metroComboBox1.SelectedIndex == 1 ? "Español" :
                    metroComboBox1.SelectedIndex == 2 ? "Français" :
                    "English";

                using (StreamWriter settings = new StreamWriter("awo.ini", false)) //do not append so that languages are saved
                {
                    settings.WriteLine("Language=" + lang);
                }

                changeLanguage(lang);
            }
            catch (Exception ex)
            {

            }
        }

        private void metroButton4_Click(object sender, EventArgs e)
        {
            DialogResult msg = MessageBox.Show("These changes can impact your computer system!\nWould you like to proceed?", "Warning", MessageBoxButtons.YesNo);
            if (msg == DialogResult.Yes)
            {
                o.gameBar(true);
                o.windowsGameMode(true);
                o.eventTimer(true);
                o.ultimatePower(true);
                o.performanceVisuals(true);
                o.gpuScheduling(true);
                o.notifications(true);
                o.cpuParking(true);
                o.cpuThrottling(true);
                o.gamingFrequencies(true);
                o.driverSearch(true);
                o.networkThrottling(true);
                o.systemResponsiveness(true);
                o.noDelay(true);
                o.delayTicks(true);

                o.setTCPChimney(true);
                o.setNetworkAllocations(true);
                o.setTCPTuning(true);
                o.setRSSandRSC(true);
                o.setCongestion(true);
                o.setTTL(true);
                o.setECN(true);
                o.setChimneyOffload(true);
                o.setLargeOffload(true);
                o.setTimestamps(true);
                o.setPorts(true);
                o.setHeuristics(true);
                o.setMaxConnections(true);
                o.setQoS(true);
                o.setTransmissions(true);

                o.disableAdvertID(true);
                o.disableDiagnostics(true);
                o.disableClipboardHistory(true);
                o.disableUserActivity(true);
                o.disableAppTracking(true);
                o.disableTypingTransmission(true);

                Application.Restart();
                Environment.Exit(0);
            }
        }

        private void metroButton5_Click(object sender, EventArgs e)
        {
            DialogResult msg = MessageBox.Show("These changes can impact your computer system!\nWould you like to proceed?", "Warning", MessageBoxButtons.YesNo);
            if (msg == DialogResult.Yes)
            {
                o.windowsGameMode(false);
                o.gameBar(false);
                o.xboxLive(false);
                o.eventTimer(false);
                o.coreIsolation(false);
                o.ultimatePower(false);
                o.performanceVisuals(false);
                o.gpuScheduling(false);
                o.notifications(false);
                o.cpuParking(false);
                o.cpuThrottling(false);
                o.networkThrottling(false);
                o.driverSearch(false);
                o.systemResponsiveness(false);
                o.gamingFrequencies(false);
                o.noDelay(false);
                o.delayTicks(false);

                o.setTCPChimney(false);
                o.setTCPTuning(false);
                o.setHeuristics(false);
                o.setCongestion(false);
                o.setRSSandRSC(false);
                o.setTTL(false);
                o.setECN(false);
                o.setChimneyOffload(false);
                o.setLargeOffload(false);
                o.setTimestamps(false);
                o.setMaxConnections(false);
                o.setNetworkAllocations(false);
                o.setHostPriorities(false);
                o.setTransmissions(false);
                o.setPorts(false);
                o.setQoS(false);

                o.disableAdvertID(false);
                o.disableAdvertising(false);
                o.disableAppTracking(false);
                o.disableClipboardHistory(false);
                o.disableDiagnostics(false);
                o.disableHandwriting(false);
                o.disableSuggestions(false);
                o.disableTypingTransmission(false);
                o.disableClipboardHistory(false);
                o.disableUserActivity(false);

                Application.Restart();
                Environment.Exit(0);
            }
        }

        #region update Text for Language
        public void changeLanguage(string language)
        {
            if(language == "English")
            {
                changeToEnglish();
            }
            else if(language == "Español")
            {
                changeToSpanish();
            }
            else if(language == "Français")
            {
                changeToFrench();
            }
        }


        public void changeToEnglish()
        {
            this.Text = "Alex's Windows Optimizer";
            metroLabel1.Text = "Language";
            metroButton4.Text = "Use Recommended Settings?";
            metroButton5.Text = "Reset All Settings?";
            metroButton2.Text = "Create A System Restore Point?";

            groupBox1.Text = "Gaming Optimizations [May Increase FPS]";
            metroLabel2.Text = "Gaming Mode:";
            metroLabel3.Text = "Turn off Xbox Game Bar?";
            metroLabel4.Text = "Turn off Xbox Live?";
            metroLabel5.Text = "Disable Precision Timer?";
            metroLabel6.Text = "Disable Core Isolation?";
            metroLabel7.Text = "Utilise Ultimate Power Plan?";
            metroLabel8.Text = "Apply Best Performance Visuals?";
            metroLabel9.Text = "Enable GPU Scheduling?";
            metroLabel10.Text = "Disable Notification Popups?";
            metroLabel11.Text = "Disable CPU Parking?";
            metroLabel12.Text = "Disable Frequency Scaling?";
            metroLabel13.Text = "Disable Driver Searching?";
            metroLabel40.Text = "Optimize Network Throttling?";
            metroLabel41.Text = "Optimize System Responsiveness?";
            metroLabel42.Text = "Disable Gaming Frequencies?";
            metroLabel43.Text = "Enable No Delay?";
            metroLabel44.Text = "Disable Gaming Ticks?";
            metroButton1.Text = "Apply Optimizations";

            groupBox2.Text = "Network Optimizations";
            metroLabel24.Text = "Normalise TCP Auto Tuning?";
            metroLabel25.Text = "Normalise Scaling Heuristics?";
            metroLabel26.Text = "Enable CTCP Congestion?";
            metroLabel27.Text = "Enable RSS and RSC?";
            metroLabel28.Text = "Utilise 64 Time To Live?";
            metroLabel29.Text = "Disable ECN Capabilities?";
            metroLabel30.Text = "Disable Checksum Offloading?";
            metroLabel31.Text = "Disable TCP Chimney Offload?";
            metroLabel32.Text = "Disable Large Send Offload?";
            metroLabel33.Text = "Disable TCP Timestamps?";
            metroLabel34.Text = "Fix Connections Per Server?";
            metroLabel35.Text = "Fix Host Priorities?";
            metroLabel36.Text = "Adjust Retransmissions?";
            metroLabel37.Text = "Optimise Quality of Service?";
            metroLabel38.Text = "Sort Network Allocations?";
            metroLabel39.Text = "Adjust Dynamic Ports?";
            metroButton3.Text = "Apply Optimizations";

            groupBox3.Text = "Current System Information";
            metroLabel14.Text = "PC Name:";
            metroLabel15.Text = "CPU:";
            metroLabel16.Text = "GPU:";
            metroLabel17.Text = "OS:";
            metroLabel18.Text = "Paging Size:";

            groupBox4.Text = "Privacy and Tracking Optimizations";
            metroLabel45.Text = "Disable Location Services?";
            metroLabel46.Text = "Disable Diagnostics?";
            metroLabel47.Text = "Disable Advertising ID?";
            metroLabel48.Text = "Disable Handwriting Data?";
            metroLabel49.Text = "Disable Typing Data?";
            metroLabel50.Text = "Disable Tracking of Apps?";
            metroLabel51.Text = "Disable Activity History?";
            metroLabel52.Text = "Disable Suggestions?";
            metroLabel53.Text = "Disable Clipboard History?";
            metroButton6.Text = "Apply Optimizations";

            metroLabel55.Text = "Version 1.0 | Developed By Alex Walker";
        }

        public void changeToFrench()
        {
            this.Text = "Optimiseur Windows d'Alex";
            metroLabel1.Text = "Langue";
            metroButton4.Text = "Utiliser les paramètres recommandés?";
            metroButton5.Text = "Réinitialiser tous les paramètres?";
            metroButton2.Text = "Créer un point de restauration du système?";

            groupBox1.Text = "Optimisations pour les jeux [Peut augmenter le nombre de FPS]";
            metroLabel2.Text = "Mode jeu:";
            metroLabel3.Text = "Désactiver la barre de jeu Xbox?";
            metroLabel4.Text = "Désactiver Xbox Live?";
            metroLabel5.Text = "Désactiver la minuterie de précision?";
            metroLabel6.Text = "Désactiver l'isolation du noyau?";
            metroLabel7.Text = "Utiliser le plan Ultimate Power?";
            metroLabel8.Text = "Appliquer les meilleurs visuels de performance?";
            metroLabel9.Text = "Activer la planification du GPU?";
            metroLabel10.Text = "Désactiver les fenêtres pop-up de notification?";
            metroLabel11.Text = "Désactiver le stationnement de l'unité centrale?";
            metroLabel12.Text = "Désactiver la mise à l'échelle des fréquences?";
            metroLabel13.Text = "Désactiver la recherche de pilotes?";
            metroLabel40.Text = "Optimiser l'étranglement du réseau?";
            metroLabel41.Text = "Optimiser la réactivité du système?";
            metroLabel42.Text = "Désactiver les fréquences de jeu?";
            metroLabel43.Text = "Activer l'option Pas de délai?";
            metroLabel44.Text = "Désactiver les tics de jeu?";
            metroButton1.Text = "Appliquer les optimisations";

            groupBox2.Text = "Optimisation du réseau";
            metroLabel24.Text = "Normaliser TCP Auto Tuning?";
            metroLabel25.Text = "Normaliser l'heuristique de mise à l'échelle?";
            metroLabel26.Text = "Activer la congestion CTCP?";
            metroLabel27.Text = "Activer RSS et RSC?";
            metroLabel28.Text = "Utiliser 64 fois le temps de vivre?";
            metroLabel29.Text = "Désactiver les capacités ECN?";
            metroLabel30.Text = "Désactiver le déchargement de la somme de contrôle?";
            metroLabel31.Text = "Désactiver l'option TCP Chimney Offload?";
            metroLabel32.Text = "Désactiver le délestage des envois volumineux?";
            metroLabel33.Text = "Désactiver les horodatages TCP?";
            metroLabel34.Text = "Fixer les connexions par serveur?";
            metroLabel35.Text = "Fixer les priorités en matière d'accueil?";
            metroLabel36.Text = "Ajuster les retransmissions?";
            metroLabel37.Text = "Optimiser la qualité de service?";
            metroLabel38.Text = "Trier les attributions de réseau?";
            metroLabel39.Text = "Ajuster les ports dynamiques?";
            metroButton3.Text = "Appliquer les optimisations";

            groupBox3.Text = "Informations sur le système actuel";
            metroLabel14.Text = "Nom du PC:";
            metroLabel15.Text = "CPU:";
            metroLabel16.Text = "GPU:";
            metroLabel17.Text = "OS:";
            metroLabel18.Text = "Paging Size:";

            groupBox4.Text = "Optimisation de la confidentialité et du suivi";
            metroLabel45.Text = "Désactiver les services de localisation?";
            metroLabel46.Text = "Désactiver les diagnostics?";
            metroLabel47.Text = "Désactiver l'identification publicitaire?";
            metroLabel48.Text = "Désactiver les données d'écriture manuscrite?";
            metroLabel49.Text = "Désactiver les données de saisie?";
            metroLabel50.Text = "Désactiver le suivi des applications?";
            metroLabel51.Text = "Désactiver l'historique des activités?";
            metroLabel52.Text = "Désactiver les suggestions?";
            metroLabel53.Text = "Désactiver l'historique du presse-papiers?";
            metroButton6.Text = "Appliquer les optimisations";

            metroLabel55.Text = "Version 1.0 | Développé par Alex Walker";
        }

        public void changeToSpanish()
        {
            this.Text = "Optimizador de Windows de Alex";
            metroLabel1.Text = "Idioma:";
            metroButton4.Text = "¿Utilizar la configuración recomendada?";
            metroButton5.Text = "¿Restablecer todos los ajustes?";
            metroButton2.Text = "¿Crear un punto de restauración del sistema?";

            groupBox1.Text = "Optimizaciones para juegos [Pueden aumentar los FPS].";
            metroLabel2.Text = "Modo de juego:";
            metroLabel3.Text = "¿Desactivar la barra de juegos de Xbox?";
            metroLabel4.Text = "¿Desactivar Xbox Live?";
            metroLabel5.Text = "¿Desactivar el temporizador de precisión?";
            metroLabel6.Text = "¿Desactivar el aislamiento del núcleo?";
            metroLabel7.Text = "¿Utilizar Ultimate Power Plan?";
            metroLabel8.Text = "¿Aplicar los mejores visuales de rendimiento?";
            metroLabel9.Text = "¿Habilitar la programación de la GPU?";
            metroLabel10.Text = "¿Desactivar las ventanas emergentes de notificación?";
            metroLabel11.Text = "¿Desactivar el aparcamiento de la CPU?";
            metroLabel12.Text = "¿Desactivar la escala de frecuencias?";
            metroLabel13.Text = "¿Desactivar la búsqueda de controladores?";
            metroLabel40.Text = "¿Optimizar el estrangulamiento de la red?";
            metroLabel41.Text = "¿Optimizar la capacidad de respuesta del sistema?";
            metroLabel42.Text = "¿Desactivar las frecuencias de juego?";
            metroLabel43.Text = "¿Activar Sin Retraso?";
            metroLabel44.Text = "¿Desactivar Gaming Ticks?";
            metroButton1.Text = "Aplicar optimizaciones";

            groupBox2.Text = "Optimización de redes";
            metroLabel24.Text = "¿Normalizar el ajuste automático del TCP?";
            metroLabel25.Text = "¿Normalizar la heurística de escala?";
            metroLabel26.Text = "¿Habilitar congestión CTCP?";
            metroLabel27.Text = "¿Habilitar RSS y RSC?";
            metroLabel28.Text = "¿Utilizar 64 horas de vida?";
            metroLabel29.Text = "¿Desactivar las capacidades ECN?";
            metroLabel30.Text = "¿Desactivar la descarga de sumas de comprobación?";
            metroLabel31.Text = "¿Desactivar TCP Chimney Offload?";
            metroLabel32.Text = "¿Desactivar la descarga de envíos grandes?";
            metroLabel33.Text = "¿Desactivar marcas de tiempo TCP?";
            metroLabel34.Text = "¿Fijar conexiones por servidor?";
            metroLabel35.Text = "¿Arreglar las prioridades de los anfitriones?";
            metroLabel36.Text = "¿Ajustar las retransmisiones?";
            metroLabel37.Text = "¿Optimizar la calidad del servicio?";
            metroLabel38.Text = "¿Ordenar las asignaciones de red?";
            metroLabel39.Text = "¿Ajustar puertos dinámicos?";
            metroButton3.Text = "Aplicar optimizaciones";

            groupBox3.Text = "Información actual del sistema";
            metroLabel14.Text = "Nombre del PC:";
            metroLabel15.Text = "CPU:";
            metroLabel16.Text = "GPU:";
            metroLabel17.Text = "OS:";
            metroLabel18.Text = "Tamaño de paginación:";

            groupBox4.Text = "Optimizaciones de privacidad y seguimiento";
            metroLabel45.Text = "¿Desactivar los servicios de localización?";
            metroLabel46.Text = "¿Desactivar Diagnóstico?";
            metroLabel47.Text = "¿Desactivar ID de publicidad?";
            metroLabel48.Text = "¿Desactivar los datos de escritura?";
            metroLabel49.Text = "¿Desactivar la introducción de datos?";
            metroLabel50.Text = "¿Desactivar el seguimiento de aplicaciones?";
            metroLabel51.Text = "¿Desactivar el historial de actividades?";
            metroLabel52.Text = "¿Desactivar sugerencias?";
            metroLabel53.Text = "¿Desactivar el historial del portapapeles?";
            metroButton6.Text = "Aplicar optimizaciones";

            metroLabel55.Text = "Versión 1.0 | Desarrollado por Alex Walker;
        }
        #endregion

        #region updateToggles
        public void updateToggles()
        {
            metroToggle1.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\GameBar", "AllowAutoGameMode") == 1 &&
                o.returnCurrentUserKeyValue(@"Software\Microsoft\GameBar", "AutoGameModeEnabled") == 1 ? true : false;

            metroToggle2.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AllowGameDVR") == 0 &&
                o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\GameDVR", "AppCatureEnabled") == 0 &&
                o.returnCurrentUserKeyValue(@"System\GameConfigStore", "GameDVR_Enabled") == 0 ? true : false;

            metroToggle3.Checked = Convert.ToInt16(o.returnLocalKeyValue(o.serviceLocation + "XblAuthManager", "Start")) == 4 &&
                Convert.ToInt16(o.returnLocalKeyValue(o.serviceLocation + "XblGameSave", "Start")) == 4 &&
                Convert.ToInt16(o.returnLocalKeyValue(o.serviceLocation + "xboxgip", "Start")) == 4 &&
                Convert.ToInt16(o.returnLocalKeyValue(o.serviceLocation + "XboxGipSvc", "Start")) == 4 &&
                Convert.ToInt16(o.returnLocalKeyValue(o.serviceLocation + "XboxNetApiSvc", "Start")) == 4 ? true : false;

            metroToggle4.Checked = o.returnCurrentUserKeyValue(@"Software\AWO_Optimizer", "eventTimerOn") == 0 ? true : false;
            metroToggle5.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Control\DeviceGuard\Scenarios\HypervisorEnforcedCodeIntegrity",
                "Enabled")) == 0 ? true : false;
            metroToggle6.Checked = o.executeCommandWithOutput(" powercfg /getactivescheme").ToString().Contains("Ultimate Performance") ? true : false;

            metroToggle7.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\Explorer\VisualEffects", "VisualFXSetting") == 3 ? true : false;
            metroToggle8.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Control\GraphicsDrivers", "HwSchMode")) == 2 ? true : false;
            metroToggle9.Checked = o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\PushNotifications", "ToastEnabled") == 0 ? true : false;

            metroToggle10.Checked = Convert.ToInt16(o.returnLocalKeyValue(
                @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583",
                 "Attributes")) == 0 && Convert.ToInt16(o.returnLocalKeyValue(
                @"SYSTEM\CurrentControlSet\Control\Power\PowerSettings\54533251-82be-4824-96c1-47b60b740d00\0cc5b647-c1df-4637-891a-dec35c318583",
                "ValueMax")) == 0 ? true : false;

            metroToggle11.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Control\Power\PowerThrottling", "PowerThrottlingOff")) == 1 ? true : false;
            metroToggle12.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\DriverSearching", "SearchOrderConfig")) == 0 ? true : false;
            metroToggle13.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
                "NetworkThrottlingIndex")) == 0 ? true : false;
            metroToggle14.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Multimedia\SystemProfile",
                "SystemResponsiveness")) == 0 ? true : false;

            metroToggle15.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + o.getActiveNetworkID(),
                "tcpAckFrequency")) == 1 ? true : false;
            metroToggle16.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + o.getActiveNetworkID(),
                "TCPNoDelay")) == 1 ? true : false;
            metroToggle17.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + o.getActiveNetworkID(),
                "TcpDelAckTicks")) == 0 ? true : false;
            metroToggle18.Checked = o.getNetInfo("netsh int tcp show global", "Receive Window Auto-Tuning Level", "disabled");
            metroToggle19.Checked = o.getNetInfo("netsh int tcp show heuristics", "Window Scaling heuristics", "disabled");
            metroToggle20.Checked = o.getNetInfo("netsh int tcp show supplemental", "Congestion Control Provider", "ctcp");

            metroToggle21.Checked = o.getNetInfo("netsh int tcp show global", "Receive-Side Scaling State", "enabled") &&
                o.getNetInfo("netsh int tcp show global", "Receive Segment Coalescing State", "disabled");
            metroToggle22.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DefaultTTL")) == 64 ? true : false;
            metroToggle23.Checked = o.getNetInfo("netsh int tcp show global", "ECN Capability", "enabled");
            metroToggle24.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "DisableTaskOffload")) == 0 ? true : false;
            metroToggle25.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "EnableTCPChimney")) == 0 ? true : false;

            metroToggle26.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + o.getActiveNetworkID(),
                "*LsoV2IPv4")) == 0 && Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters\Interfaces\" + o.getActiveNetworkID(),
                "*LsoV2IPv6")) == 0 ? true : false;
            metroToggle27.Checked = o.getNetInfo("netsh int tcp show global", "RFC 1323 Timestamps", "enabled");
            metroToggle28.Checked = Convert.ToInt16(o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPerServer"))
                == 10 && Convert.ToInt16(o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\Internet Settings", "MaxConnectionsPer1_0Server")) == 10 ?
                true : false;
            metroToggle29.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider", "LocalPriority")) == 4 &&
                Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider", "HostPriority")) == 5 &&
                Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider", "DnsPriority")) == 6 &&
                Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\ServiceProvider", "NetbtPriority")) == 7 ? true : false;
            metroToggle30.Checked = o.getNetInfo("netsh int tcp show global", "Max SYN Retransmissions", "2") &&
                o.getNetInfo("netsh int tcp show global", "Non Sack Rtt Resiliency", "disabled");
            metroToggle31.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SOFTWARE\Policies\Microsoft\Windows\Psched", "NonBestEffortLimit")) == 0
                && Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\nlasvc", "Start")) == 4 ? true : false;

            metroToggle32.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Control\Session Manager\Memory Management", "LargeSystemCache")) == 1
                ? true : false;
            metroToggle33.Checked = Convert.ToInt32(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "MaxUserPort")) == 65534
                && Convert.ToInt16(o.returnLocalKeyValue(@"SYSTEM\CurrentControlSet\Services\Tcpip\Parameters", "TcpTimedWaitDelay")) == 30 ? true : false;
            metroToggle34.Checked = Convert.ToInt16(o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled")) == 0 ?
                true : false;
            metroToggle35.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SOFTWARE\Policies\Microsoft\Windows\DataCollection", "AllowTelemetry")) == 0 ?
                true : false;
            metroToggle36.Checked = Convert.ToInt16(o.returnCurrentUserKeyValue(@"Software\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled")) == 0
                && Convert.ToInt16(o.returnLocalKeyValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\AdvertisingInfo", "Enabled")) == 0 ?
                true : false;

            metroToggle37.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"Software\Policies\Microsoft\Windows\TabletPC", "PreventHandwritingDataSharing")) == 1 ?
                true : false;
            metroToggle38.Checked = Convert.ToInt16(o.returnCurrentUserKeyValue(@"Software\Microsoft\Input\TIPC", "Enabled")) == 0 ? true : false;
            metroToggle39.Checked = Convert.ToInt16(o.returnCurrentUserKeyValue(@"Software\Policies\Microsoft\Windows\EdgeUI", "DisableMFUTracking")) == 1 &&
                Convert.ToInt16(o.returnLocalKeyValue(@"SOFTWARE\Policies\Microsoft\Windows\EdgeUI", "DisableMFUTracking")) == 1 ? true : false;
            metroToggle40.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SOFTWARE\Policies\Microsoft\Windows\System", "PublishUserActivities")) == 0 ? true : false;
            metroToggle41.Checked = Convert.ToInt16(o.returnCurrentUserKeyValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                "SubscribedContent-338389Enabled")) == 0 && Convert.ToInt16(o.returnCurrentUserKeyValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                "SystemPaneSuggestionsEnabled")) == 0 && Convert.ToInt16(o.returnCurrentUserKeyValue(@"SOFTWARE\Microsoft\Windows\CurrentVersion\ContentDeliveryManager",
                "SoftLandingEnabled")) == 0 ? true : false;
            metroToggle42.Checked = Convert.ToInt16(o.returnLocalKeyValue(@"SOFTWARE\Policies\Microsoft\Windows\System", "AllowClipboardHistory")) == 0 ? true : false;

        }
        #endregion

        #region ALL GUI EMPTY FUNCTIONS

        private void Form1_Load(object sender, EventArgs e)
        {

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

        private void metroToggle13_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle14_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle15_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle16_CheckedChanged(object sender, EventArgs e)
        {
        }

        private void metroToggle18_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle20_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle21_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle22_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle23_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle24_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle26_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle28_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle29_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle30_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle31_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle32_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle33_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            Process.Start("https://github.com/AlexW03x?tab=repositories");
        }

        private void metroLabel19_Click(object sender, EventArgs e)
        {

        }

        private void metroToggle35_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle36_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle37_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle38_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle39_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle40_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void metroToggle41_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void metroToggle42_CheckedChanged(object sender, EventArgs e)
        {

        }
        #endregion
    }
}
