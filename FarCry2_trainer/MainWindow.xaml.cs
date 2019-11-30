using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Diagnostics;
using System.IO.Compression;


using System.IO;
using System.Xml;

namespace FarCry2_trainer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        XmlDocument doc;
        ParseGameFiles pgf;
        MapWindow map;
        Validator v;


        public MainWindow()
        {
            InitializeComponent();


            LogMessage("Loaded.");
            doc = new XmlDocument();
            v = new Validator();
            pgf = new ParseGameFiles();

            gameLocation.Text = v.GetGamePath();
            patchLocation.Text = v.GetWorkingDir();
            binLocation.Text = v.GetBinariesDir();

            CheckStatus();
            UpdateXmlList();

        }


        #region Map 

        private void mapNorth_Click(object sender, RoutedEventArgs e)
        {
            if (map == null)
                map = new MapWindow();
            if (map.IsClosed)
                map = new MapWindow();
            map.DisplayImage("North Map - Leboa Sako", "pack://siteoforigin:,,,/Resources/NORTH_leboa_sako.png");
        }

        private void mapSouth_Click(object sender, RoutedEventArgs e)
        {
            if (map == null)
                map = new MapWindow();
            if (map.IsClosed)
                map = new MapWindow();
            map.DisplayImage("South Map - Bowa Seko", "pack://siteoforigin:,,,/Resources/SOUTH_bowa_seko.png");
        }

        private void owg_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.openworldgames.org/owg/forums/index.php?board=169.0");
        }

        #endregion


        #region Setup Files 

        private void CheckStatus()
        {
            try
            {                
                bool game = v.GetGameStatus(gameLocation.Text);                
                bool patch = v.GetWorkingDirStatus(patchLocation.Text);                
                bool bin = v.GetBinariesDirStatus(binLocation.Text);

                SetStatusDisplay(gameStatus, game);
                SetStatusDisplay(patchStatus, patch);
                SetStatusDisplay(binStatus, bin);

                if (game && patch && bin)
                    LogSuccess("all file checks passed");

                if (!game)
                    LogError("enter correct game directory");
                if (!bin)
                    LogError("still need to setup binaries");
                if (!patch)
                    LogError("still need to extract");                
                    
            }
            catch (Exception exp)
            {                
                LogError(exp.Message);
            }

        }

        private void SetStatusDisplay(TextBox tb, bool status)
        {
            if (status)
            {
                tb.Text = "OK";
                tb.Background = Brushes.LightGreen;
            }
            else
            {
                tb.Text = "files not found";
                tb.Background = Brushes.LightPink;
            }
        }
        
        private void checkStatusButton_Click(object sender, RoutedEventArgs e)
        {
            CheckStatus();
        }               

        private void extractButton_Click(object sender, RoutedEventArgs e)
        {

            string working_dir = patchLocation.Text;
            string bin_dir = binLocation.Text;
            string game_dir = gameLocation.Text;
            string source, dest;

            if (!v.GetGameStatus(game_dir))
            {
                LogError("incorrect game folder!");
                return;
            }
            if (!v.GetBinariesDirStatus(bin_dir))
            {
                LogError("setup not completed for Gibbed binaries!");
                return;
            }

            if (v.GetWorkingDirStatus(working_dir))
            {
                LogError("already extracted!");
                return;
            }

            LogMessage("backing up fc2 'patch.dat' and 'patch.fat'");            
            try
            {
                
                source = Path.Combine(game_dir, "Data_Win32", "patch.fat");
                dest = Path.Combine(game_dir, "Data_Win32", "_patch.fat");
                File.Copy(source, dest, true);

                source = Path.Combine(game_dir, "Data_Win32", "patch.dat");
                dest = Path.Combine(game_dir, "Data_Win32", "_patch.dat");
                File.Copy(source, dest, true);

                LogSuccess("complete");
            }
            catch (Exception exp)
            {
                LogError(exp.Message);
            }


            LogMessage("running Gibbed.Bootstrap extractor");
            try
            {
                string bootstrap_exe = Path.Combine(binLocation.Text, "Bootstrap.exe");
                if (!Directory.Exists(working_dir))
                    Directory.CreateDirectory(working_dir);


                Process proc = new Process();

                proc.StartInfo.FileName = bootstrap_exe;
                proc.StartInfo.Arguments = working_dir;
                proc.StartInfo.RedirectStandardOutput = true;
                proc.StartInfo.UseShellExecute = false;

                proc.Start();                
                string output = proc.StandardOutput.ReadToEnd(); 
                proc.WaitForExit();

                if (output.Contains("Could not detect Far Cry 2 install directory"))
                {
                    LogError(output);
                    return;
                }

                LogMessage(output);
                LogSuccess("complete");
            }
            catch (Exception exp)
            {
                LogError(exp.Message);
            }


            LogMessage("copying extra files to mod folders");
            try
            { 
                string gamemodes_dir = Path.Combine(working_dir, "mypatch", "engine", "gamemodes");
                if (!Directory.Exists(gamemodes_dir))
                    Directory.CreateDirectory(gamemodes_dir);


                source = Path.Combine(working_dir, "original", "patch", "engine", "gamemodes", "gamemodesconfig.xml");
                dest = Path.Combine(working_dir, "mypatch", "engine", "gamemodes", "gamemodesconfig.xml");
                File.Copy(source, dest, true);

                source = Path.Combine(working_dir, "libraries", "world1", "28_pickups.xml");
                dest = Path.Combine(working_dir, "mymod", "patch", "28_pickups.xml");
                File.Copy(source, dest, true);

                source = Path.Combine(working_dir, "libraries", "world1", "30_player.xml");
                dest = Path.Combine(working_dir, "mymod", "patch", "30_player.xml");
                File.Copy(source, dest, true);

                source = Path.Combine(working_dir, "libraries", "world1", "41_WeaponProperties.xml");
                dest = Path.Combine(working_dir, "mymod", "patch", "41_WeaponProperties.xml");
                File.Copy(source, dest, true);

                source = Path.Combine(game_dir, "Data_Win32", "patch.fat");
                dest = Path.Combine(working_dir, "_patch.fat");
                File.Copy(source, dest, true);

                source = Path.Combine(game_dir, "Data_Win32", "patch.dat");
                dest = Path.Combine(working_dir, "Data_Win32", "_patch.dat");
                File.Copy(source, dest, true);

                LogSuccess("complete");
            }
            catch (Exception exp)
            {
                LogError(exp.Message);
            }
            

            LogMessage("adding additional mod files to 'mymod/patch.xml'");
            try
            { 
                XmlDocument patch = new XmlDocument();
                string patch_path = Path.Combine(working_dir, "mymod", "patch.xml");
                patch.Load(patch_path);
                XmlNode file_list = patch.ChildNodes[0];

                foreach (string s in v.GetStandardFilesForModding())
                {
                    if (v.GetStandardPath(s).Contains("mypatch"))
                        continue;

                    XmlDocument doc = new XmlDocument();
                    doc.LoadXml(@"<object external=""" + s + @""" />");
                    XmlNode myNode = doc.DocumentElement;
                    XmlNode importNode = patch.ImportNode(myNode, true);
                    file_list.AppendChild(importNode);
                }
                patch.Save(patch_path);
                LogSuccess("complete");
            }
            catch (Exception exp)
            {                
                LogError(exp.Message);
            }

            CheckStatus();

        }

        private void setupBinButton_Click(object sender, RoutedEventArgs e)
        {

            LogMessage("unzipping Gibbed binaries to working folder");
            try
            {
                string bin_dir = binLocation.Text;

                if (!Directory.Exists(bin_dir))
                    Directory.CreateDirectory(bin_dir);

                string zip_path = Path.Combine(bin_dir, "gibbed.zip");
                File.WriteAllBytes(zip_path, FarCry2_trainer.Properties.Resources.gibbed);
                ZipFile.ExtractToDirectory(zip_path, bin_dir);

                LogSuccess("complete");

               
            }
            catch (Exception exp)
            {                
                LogError(exp.Message);
            }

            CheckStatus();
        }

        #endregion


        #region Edit Files

        private void LoadDoc()
        {
            string base_dir = patchLocation.Text;

            if (xmlFileComboBox.SelectedItem == null)
                return;

            LogMessage("loading xml file");

            try
            { 
                string file;
                if ((bool)usePathCheckBox.IsChecked)
                    file = Path.Combine(base_dir, folderTextBox.Text, xmlFileComboBox.SelectedItem.ToString());
                else
                    file = Path.Combine(base_dir, v.GetStandardPath(xmlFileComboBox.SelectedItem.ToString()));
                doc.Load(file);

                listBox.Items.Clear();


                if (xmlFileComboBox.SelectedItem.ToString().Contains("gamemodes"))
                {
                    LogError("no display for 'gamemodesconfig.xml'");
                    return;
                }

                List<string> results = pgf.GetDirectChildren(doc);
                foreach (string s in results)
                    listBox.Items.Add(s);
                LogSuccess("complete");
            }
            catch (Exception exp)
            {
                LogError(exp.Message);
            }


        }

        private void LoadOperations()
        {
            operationComboBox.Items.Clear();

            string xmlSelected = xmlFileComboBox.SelectedItem.ToString();

            switch (xmlSelected)
            {
                case "41_WeaponProperties.xml":
                    operationComboBox.Items.Add("Make silent");
                    operationComboBox.Items.Add("Make primary");
                    operationComboBox.Items.Add("Make secondary");
                    operationComboBox.Items.Add("Make special");
                    operationComboBox.Items.Add("Make dart rifle sound");
                    operationComboBox.Items.Add("Remove spread");
                    break;
                case "30_player.xml":
                    operationComboBox.Items.Add("Make climber");
                    break;
                case "28_pickups.xml":
                    operationComboBox.Items.Add("Make respawn");
                    break;
                case "gamemodesconfig.xml":
                    operationComboBox.Items.Add("Unlock weapons");
                    operationComboBox.Items.Add("Unlock equipment");
                    operationComboBox.Items.Add("Set manual cost to 1");
                    operationComboBox.Items.Add("Remove malaria");
                    operationComboBox.Items.Add("Unlimited sprint");
                    break;

            }
        }

        private void UpdateXmlList()
        {
            LogMessage("updating child list for xml doc");

            try
            {                

                List<string> results = new List<string> { };

                if (usePathCheckBox != null && (bool)usePathCheckBox.IsChecked)
                {
                    string folder = Path.Combine(patchLocation.Text, folderTextBox.Text);
                    results = v.GetFilesForModding(folder);
                    folderTextBox.IsReadOnly = false;
                }
                else
                {
                    results = v.GetStandardFilesForModding();
                    folderTextBox.IsReadOnly = true;
                }

                xmlFileComboBox.Items.Clear();

                foreach (string s in results)
                    xmlFileComboBox.Items.Add(s);

                LogSuccess("complete");
            }
            catch (Exception exp)
            {
                LogError(exp.Message);
            }
        }


        private bool ReadyToMod()
        {
            bool game = v.GetGameStatus(gameLocation.Text);
            bool patch = v.GetWorkingDirStatus(patchLocation.Text);
            bool bin = v.GetBinariesDirStatus(binLocation.Text);

            if (!game || !patch || !bin)
            {
                LogError("cannot edit files before setup complete");
                return false;
            }

            return true;
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ReadyToMod())
                return;

            string name = xmlFileComboBox.SelectedItem.ToString();
            string base_dir = patchLocation.Text;

            if (folderTextBox.Text.Contains("libraries"))
            {
                LogError("Cannot overwrite libraries!");
                return;
            }

            try
            {
                LogMessage("saving changes to " + name);
                string file;

                if ((bool)usePathCheckBox.IsChecked)
                    file = Path.Combine(base_dir, folderTextBox.Text, name);
                else
                    file = Path.Combine(base_dir, v.GetStandardPath(name));

                doc.Save(file);
                LogSuccess("complete");
            }
            catch (Exception exp)
            {
                LogError(exp.Message);
            }
        }

        private void resetButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ReadyToMod())
                return;

            try
            {
                string name = xmlFileComboBox.SelectedItem.ToString();
                LogMessage("reverting file " + name);                

                string base_dir = patchLocation.Text;
                string original_copy = Path.Combine(base_dir, v.GetStandardPath(name, true));
                string overwrite_path = Path.Combine(base_dir, v.GetStandardPath(name));

                File.Copy(original_copy, overwrite_path, true);

                LogSuccess("complete");
            }
            catch (Exception exp)
            {
                LogError(exp.Message);
            }
        }

        private void xmlFileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LoadDoc();
            LoadOperations();
        }

        private void listBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (listBox.Items.Count == 0)
                return;

            string name = listBox.SelectedItem.ToString();
            string file = xmlFileComboBox.SelectedItem.ToString();


            textBox.Text = "";

            Dictionary<string, string> dict = pgf.GetInfo(doc, name, file);

            foreach (string k in dict.Keys)
            {
                textBox.Text += k + "   " + dict[k] + Environment.NewLine;
            }


        }

        private void editButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ReadyToMod())
                return;

            string name = "";
            if (listBox.SelectedItem != null)
                name = listBox.SelectedItem.ToString();
            string operation = "";
            if (operationComboBox.SelectedItem != null)
                operation = operationComboBox.SelectedItem.ToString();

            try
            {
                LogMessage("attempting edit operation " + operation + " on item " + name);
                
                if (operation == "Make silent")                    pgf.MakeSilent(ref doc, name);
                if (operation == "Make primary")                   pgf.MakePrimary(ref doc, name);
                if (operation == "Make secondary")                 pgf.MakeSecondary(ref doc, name);
                if (operation == "Make special")                   pgf.MakeSpecial(ref doc, name);
                if (operation == "Make dart rifle sound")          pgf.MakeDartRifleSound(ref doc, name);
                if (operation == "Remove spread")                  pgf.RemoveSpread(ref doc, name);
                if (operation == "Make climber")                   pgf.MakeClimber(ref doc, name);
                if (operation == "Make respawn")                   pgf.MakeRespawn(ref doc, name);
                if (operation == "Unlock weapons")                 pgf.UnlockWeapons(ref doc);
                if (operation == "Unlock equipment")               pgf.UnlockEquipment(ref doc);
                if (operation == "Set manual cost to 1")           pgf.SetManualCost(ref doc, "1");
                if (operation == "Remove malaria")                 pgf.RemoveMalaria(ref doc);
                if (operation == "Unlimited sprint")               pgf.UnlimitedSprint(ref doc);

                LogSuccess("complete");
            }
            catch (Exception exp)
            {
                LogError(exp.Message);
            }

        }

        private void shipItButton_Click(object sender, RoutedEventArgs e)
        {
            if (!ReadyToMod())
                return;

            LogMessage("pushing xml changes to 'patch.dat' and 'patch.fat' in game directory");

            try
            { 
                string convert_binary_exe = Path.Combine(binLocation.Text, "Gibbed.Dunia.ConvertBinary.exe");
                string patch_xml = Path.Combine(patchLocation.Text, "mymod", "patch.xml");
                string patch_override_fcb = Path.Combine(patchLocation.Text, "mypatch", "generated", "entitylibrarypatchoverride.fcb");
                string convert_binaries_args = @"--fcb """ + patch_xml + @""" """ + patch_override_fcb + @"""";

                string pack_exe = Path.Combine(binLocation.Text, "Gibbed.Dunia.Pack.exe");
                string game_exe = gameLocation.Text;
                string patch_fat = Path.Combine(game_exe, "Data_Win32", "patch.fat");
                string patch_folder = Path.Combine(patchLocation.Text, "mypatch");
                string original_patch_folder = Path.Combine(patchLocation.Text, "original", "patch");
                string pack_args = @"-c """ + patch_fat + @""" """ + patch_folder + @""" """ + original_patch_folder + @"""";
 
                var proc = System.Diagnostics.Process.Start(convert_binary_exe, convert_binaries_args);
                proc.WaitForExit();
                proc = System.Diagnostics.Process.Start(pack_exe, pack_args);
                LogSuccess("complete");
            }
            catch (Exception exp)
            {
                LogError(exp.Message);
            }

        }

        private void usePathCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            UpdateXmlList();
        }

        private void folderTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateXmlList();
        }

        #endregion


        #region Log

        public void Log(string text, Brush color)
        {
            if (logTextBox == null)
                return;

            TextRange tr = new TextRange(logTextBox.Document.ContentEnd, logTextBox.Document.ContentEnd);
            tr.Text = text + Environment.NewLine;
           
            tr.ApplyPropertyValue(TextElement.ForegroundProperty, color);

            logTextBox.ScrollToEnd();
        }

        public void LogError(string text)
        {
            Log(text, Brushes.Red);
        }

        public void LogMessage(string text)
        {
            Log(text, Brushes.LightSkyBlue);

        }

        public void LogSuccess(string text)
        {
            Log(text, Brushes.LightGreen);

        }






        #endregion





    }
}
