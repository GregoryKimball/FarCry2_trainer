using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security;
using Microsoft.Win32;
using System.IO;

namespace FarCry2_trainer
{
    class Validator
    {


        public string GetGamePath()
        {


            string keyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Ubisoft\\Far Cry 2";
            string valueName = "InstallDir";
            string locationPath = "";
            bool failed = true;

            try
            {
                var value = (string)Registry.GetValue(keyName, valueName, null);
                if (value != null && Directory.Exists(value) == true)
                {
                    locationPath = value;
                    failed = false;
                }
            }
            catch (SecurityException)
            {
                failed = true;
                throw;
            }

            return locationPath;
        }


        public bool GetGameStatus(string game_path)
        {

            string game_exe = Path.Combine(game_path, "bin", "FarCry2.exe");

            string patch_dat = Path.Combine(game_path, "Data_Win32", "patch.dat");
            string common_dat = Path.Combine(game_path, "Data_Win32", "common.dat");

            if (File.Exists(game_exe) && File.Exists(patch_dat) && File.Exists(common_dat))
                return true;
            else
                return false;
        }


        public string GetWorkingDir()
        {
            string user_folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string path = Path.Combine(user_folder, "FarCry2_trainer");
            return path;
        }


        public bool GetWorkingDirStatus(string working_dir)
        {
            string lib_file = Path.Combine(working_dir, "libraries", "world1", "41_WeaponProperties.xml");
            string mymod_dir = Path.Combine(working_dir, "mymod");
            string mypatch_dir = Path.Combine(working_dir, "mypatch");
            string orig_gamemodes = Path.Combine(working_dir, "original", "patch", "engine", "gamemodes", "gamemodesconfig.xml");

            if (File.Exists(lib_file) && File.Exists(orig_gamemodes) &&
                Directory.Exists(mymod_dir) && Directory.Exists(mymod_dir))
                return true;
            else
                return false;
        }



        public string GetBinariesDir()
        {
            string user_folder = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
            string path = Path.Combine(user_folder, "FarCry2_trainer", "gibbed_binaries");
            return path;
        }

        public bool GetBinariesDirStatus(string bin_dir)
        {
            string f1 = Path.Combine(bin_dir, "Bootstrap.exe");
            string f2 = Path.Combine(bin_dir, "Gibbed.Dunia.Pack.exe");
            string f3 = Path.Combine(bin_dir, "projects", "Far Cry 2.xml");

            if (File.Exists(f1) && File.Exists(f2) && File.Exists(f3))
                return true;
            else
                return false;

        }


        public List<string> GetFilesForModding(string folder)
        {
            List<string> results = new List<string> { };

            if (Directory.Exists(folder))
            {                

                string[] files = Directory.GetFiles(folder);

                for (int i = 0; i < files.Length; i++)
                {
                    results.Add(files[i].Split('\\')[files[i].Split('\\').Length - 1]);
                }

            }

            return results;
        }

        public List<string> GetStandardFilesForModding()
        {
            List<string> results = new List<string> { };
            results.Add("gamemodesconfig.xml");
            results.Add("28_pickups.xml");
            results.Add("30_player.xml");
            results.Add("41_WeaponProperties.xml");
            return results;
        }

        public string GetStandardPath(string file, bool reference=false)
        {
            if (reference)
            {
                switch (file)
                {
                    case "28_pickups.xml":
                    case "30_player.xml":
                    case "41_WeaponProperties.xml":
                        return Path.Combine("libraries", "world1", file);

                    case "gamemodesconfig.xml":
                        return Path.Combine("original", "patch", "engine", "gamemodes", file);
                }
            }
            else
            {
                switch (file)
                {
                    case "28_pickups.xml":
                    case "30_player.xml":
                    case "41_WeaponProperties.xml":
                        return Path.Combine("mymod", "patch", file);

                    case "gamemodesconfig.xml":
                        return Path.Combine("mypatch", "engine", "gamemodes", file);
                }
            }

            return "ERROR";
        }

    }
}
