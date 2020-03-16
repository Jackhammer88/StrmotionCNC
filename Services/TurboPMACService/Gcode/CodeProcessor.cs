using Light.GuardClauses;
using Infrastructure.Interfaces.CNCControllerService;
using Infrastructure.MyRegex;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace ControllerService.Gcode
{
    public class CodeProcessor : ICodeCalculator
    {
        readonly Dictionary<double[], int> _gCodes = new Dictionary<double[], int>
        {
            {new double[] { 0, 1, 2, 3, 38.2, 80, 81, 82, 83, 84, 85, 86, 87, 88, 89 }, 0 },
            {new double[] { 4, 10, 28, 30, 53, 92, 92.1, 92.2, 92.3 }, 1 },
            {new double[] { 17, 18, 19 }, 2 },
            {new double[] { 90, 91 }, 3 },
            {new double[] { 93, 94 }, 4 },
            {new double[] { 20, 21 }, 5 },
            {new double[] { 40, 41, 42 }, 6 },
            {new double[] { 43, 49 }, 7 },
            {new double[] { 98, 99 }, 8 },
            {new double[] { 54, 55, 56, 57, 58, 59, 59.1, 59.2, 59.3 }, 9 },
            {new double[] { 61, 61.1, 64 }, 10 },
        };
        readonly Dictionary<int[], int> _mCodes = new Dictionary<int[], int>
        {
            {new int[] { 0, 1, 2, 30, 60 }, 0 },
            {new int[] { 6 }, 1 },
            {new int[] { 3, 4, 5 }, 2 },
            {new int[] { 7, 8, 9 }, 3 },
            {new int[] { 48, 49 }, 4 }
        };
        public int GetGGroup(double code)
        {
            var gCodeGroup = _gCodes.FirstOrDefault(p => p.Key.Contains(code));
            gCodeGroup.MustNotBeNullReference(nameof(gCodeGroup));
            return gCodeGroup.Value;
        }
        public int GetMGroup(int code)
        {
            var mCodeGroup = _mCodes.Where(p => p.Key.Contains(code)).First();
            mCodeGroup.MustNotBeNullReference(nameof(mCodeGroup));
            return mCodeGroup.Value;
        }
        public int? GetTCode(string programString)
        {
            Match match = PrecompiledRegexExec.TCode.Match(programString);
            if (match != null && match.Value.Contains("T") || match.Value.Contains("t"))
                return Convert.ToInt32(match.Value.Replace("t", string.Empty).Replace("T", string.Empty), CultureInfo.InvariantCulture);
            else
                return null;
        }
        public void GetMCodes(string programString, Dictionary<int, int?> mGroup)
        {
            if (mGroup == null) throw new ArgumentNullException(nameof(mGroup));
            foreach (Match match in PrecompiledRegexExec.MCodes.Matches(programString))
            {
                var parsedValue = match.Value.Replace("m", string.Empty).Replace("M", string.Empty);
                var convertedValue = Convert.ToInt32(parsedValue, CultureInfo.InvariantCulture);
                var currentMGroup = GetMGroup(convertedValue);
                mGroup[currentMGroup] = convertedValue;
            }
        }
        public void GetGCodes(string programString, Dictionary<int, double?> gGroups)
        {
            if (gGroups == null) throw new ArgumentNullException(nameof(gGroups));
            foreach (Match match in PrecompiledRegexExec.GCodes.Matches(programString))
            {
                var parsedValue = match.Value.Replace("g", string.Empty).Replace("G", string.Empty);
                var convertedValue = Convert.ToDouble(parsedValue, CultureInfo.InvariantCulture);
                var currentGGroup = GetGGroup(convertedValue);
                gGroups[currentGGroup] = convertedValue;
            }
        }
    }
}