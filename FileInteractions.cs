using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Collections;
using System.Windows;

// Final Project for CS50
// (C)opyright 2023 Jonathan E. Styles

namespace TimeCheck
{
    class FileInteractions
    {
        // Get environment storage directory.
        string userFiles = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) + "\\TimeCheck\\";

        // INI Dictionary (for loading INI into memory)
        Dictionary<string, Dictionary<string, string>> INI = new Dictionary<string, Dictionary<string, string>>();

        /// <summary>
        /// Used for basic File I/O functions in TimeCheck.
        /// </summary>
        public FileInteractions()
        {
            // If our application doesn't have a file storage directory, create it.
            if (!Directory.Exists(userFiles))
            {
                Directory.CreateDirectory(userFiles);
                write_log("AppData directory was not located. Created.");
            }

            load_ini();
        }

        // Write to the settings INI file.
        /// <summary>
        /// Writes specifically to the [settings] area of the INI.
        /// </summary>
        /// <param name="tag">The name of the data you are writing to.</param>
        /// <param name="val">The value you wish to write.</param>
        public void write_setting(string tag, string val)
        {
            bool inSection = false;
            string[] f = File.ReadAllLines(userFiles + "config.ini");
            for (int i = 0; i < f.Length; i++)
            {
                if ((f[i].StartsWith("[")) && (f[i].EndsWith("]")))
                {
                    if (f[i].Contains("settings"))
                    {
                        inSection = true;
                    }
                    if (!inSection)
                    {
                        continue;
                    }
                }

                if (inSection)
                {
                    if (f[i].StartsWith(tag))
                    {
                        f[i] = tag + " = " + val;
                        File.WriteAllLines(userFiles + "config.ini", f);
                        reload_ini();
                        break;
                    }
                }
            }
        }

        private void write_ini(string section, string tag, string val)
        {
            if (File.Exists(userFiles + "config.ini"))
            {
                INI[section][tag] = val;
            }
            else
            {
                create_default_ini();
                load_ini();
            }
        }

        /// <summary>
        /// Clears the INI data in memory and reloads it. (Handy for when file is updated.)
        /// </summary>
        private void reload_ini()
        {
            INI.Clear();
            load_ini();
        }

        /// <summary>
        /// Reads a value from the given key in [settings]
        /// </summary>
        /// <param name="tag">The key you wish to look for and get data from.</param>
        /// <returns></returns>
        public string read_setting(string tag)
        {
            string val = read_ini("settings", tag);
            return val;
        }

        /// <summary>
        /// Returns a specific value from anywhere in the INI using the given section and key.
        /// </summary>
        /// <param name="section">The [section] in the INI this value lives in.</param>
        /// <param name="tag">The key in this section the value should be listed in.</param>
        /// <returns></returns>
        private string read_ini(string section, string tag)
        {
            string ret = INI[section][tag];
            return ret;
        }

        /// <summary>
        /// Writes the saved INI file into memory (Nested Dictionary) for quick calls.
        /// </summary>
        private void load_ini()
        {
            if (File.Exists(userFiles + "config.ini"))
            {
                string[] fm = File.ReadAllLines(userFiles + "config.ini");
                string workingSection = "";
                for (int i = 0; i < fm.Length; i++)
                {
                    if (!fm[i].StartsWith(";"))     // Ignore any commented lines.
                    {
                        if ((fm[i].StartsWith("[")) && (fm[i].EndsWith("]")))   // INI Section Headers
                        {
                            workingSection = fm[i].Trim('[', ']');
                            INI.Add(workingSection, new Dictionary<string, string>());
                            continue;
                        }

                        if ((fm[i] != "") && (fm[i] != "\n"))
                        {
                            string[] d = fm[i].Split('=');
                            INI[workingSection].Add(d[0].Trim(), d[1].Trim());

                        }

                    }

                }
            }
            else
            {
                create_default_ini();
                load_ini();
            }
        }

        /// <summary>
        /// Used for creation of a "default" INI file in case one does not exist in the filesystem.
        /// </summary>
        /// <returns></returns>
        public int create_default_ini()
        {
            try
            {
                ArrayList f = new ArrayList();
                f.Add("; Settings section for TimeCheck application.");
                f.Add("[settings]");
                f.Add("Last Location = 0,0");
                f.Add("Position Lock = 0");

                string[] output = new string[f.Count];
                for (int r = 0; r < output.Length; r++)
                {
                    output[r] = f[r].ToString();
                }

                File.WriteAllLines(userFiles + "config.ini", output);
                write_log("Settings INI not found. Successfully generated default in user Local Application Data.");
            }
            catch
            {
                write_log("There was an issue attempting to create the settings INI file.");
                return 1;   // Return showing failure to create INI file.
            }

            return 0;
        }

        /// <summary>
        /// Used for writing text strings into the diagnostic log.
        /// </summary>
        /// <param name="data">Text to write into log.</param>
        public void write_log(string data)
        {
            File.AppendAllText(userFiles + "diag.log", "[" + System.DateTime.Now + "]: " + data + "\n");
        }
    }
}
