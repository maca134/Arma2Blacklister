using IniParser;
using IniParser.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Arma2Blacklist
{
    public class Settings
    {
        private static IniData data;
        public static dynamic GetValue(String section, String key)
        {
            if (data == null)
            {
                FileIniDataParser inifile = new FileIniDataParser();
                String path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"\\config.ini";
                data = inifile.ReadFile(path);
            }
            if (data[section] == null)
            {
                return false;
            }
            if (data[section][key] == null)
            {
                return false;
            }
            return data[section][key];
        }
    }

    class SettingsException : Exception
    {
        public SettingsException() { }
        public SettingsException(string message) : base(message) { }
        public SettingsException(string message, Exception inner) : base(message, inner) { }
    }
}
