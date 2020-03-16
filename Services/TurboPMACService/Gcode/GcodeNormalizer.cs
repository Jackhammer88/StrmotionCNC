using System.Linq;
using System.Text.RegularExpressions;

namespace ControllerService.GCode
{
    public static class GCodeNormalizer
    {
        public static string NormalizeString(string programString)
        {
            var replaced = Regex.Replace(programString, @"\(.*\)", string.Empty);
            if (string.IsNullOrWhiteSpace(replaced))
                return string.Empty;
            replaced = replaced.Replace(" ", string.Empty).Trim('\r', '\n');
            string result = string.Empty;

            foreach (var match in Infrastructure.MyRegex.PrecompiledRegexExec.Coordinates.Matches(replaced))
            {
                result += $"{match.ToString()} ";
            }
            return result;
        }
        public static string RemoveComments(string innerString)
        {
            while (innerString.Any(c => c == '('))
            {
                var str1 = innerString.TakeWhile(c => c != '(');
                var str2 = innerString.SkipWhile(c => c != ')').Skip(1);
                innerString = new string(str1.Concat(str2).ToArray());
            }
            return innerString;
        }
    }
}
