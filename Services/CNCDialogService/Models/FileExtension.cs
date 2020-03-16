using System;

namespace CNCDialogService.Models
{
    public class FileExtension
    {
        public string Description { get; set; }
        public string Extension { get; set; }
        public bool CompareExtensions(string innerExtension)
        {
            if (string.IsNullOrEmpty(Extension))
                return true;
            return string.Compare(innerExtension, Extension, StringComparison.InvariantCultureIgnoreCase) == 0;
        }
        public static FileExtension Default => new FileExtension { Description = "All files", Extension = string.Empty };
    }
}
