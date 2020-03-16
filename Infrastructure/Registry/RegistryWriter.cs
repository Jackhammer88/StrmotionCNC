using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Registry
{
    using Registry = Microsoft.Win32.Registry;

    public static class AppRegistryPath
    {
        public const string ApplicationBranchPath = @"HKEY_CURRENT_USER\Software\Strmotion";
        public const string LastLaserConfigurationKeyName = @"LastLaserConfigurationPath";
        public const string LastOpenedFilePath = @"LastDirectoryPath";
    }

    public static class RegistryWriter
    {
        public static void WriteValue(string key, string keyName, string value)
        {
            Registry.SetValue(key, keyName, value);
        }
    }

    public static class RegistryReader
    {
        public static string ReadStringValue(string key, string keyName)
        {
            return (string)Registry.GetValue(key, keyName, string.Empty);
        }
    }
}
