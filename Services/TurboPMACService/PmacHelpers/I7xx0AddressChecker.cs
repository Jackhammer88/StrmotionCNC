using System;
using System.Collections.Generic;
using System.Globalization;

namespace ControllerService.PmacHelpers
{
    internal class I7xx0AddressChecker
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Globalization", "CA1303:Не передавать литералы в качестве локализованных параметров", Justification = "<Ожидание>")]
        internal static List<int> ParseAddresses(string sI4900)
        {
            var result = new List<int> { 0, 0 };
            sI4900 = sI4900.Trim('$');
            int.TryParse(sI4900, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int intI4900);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 10; j++)
                {
                    uint mask = ((uint)(intI4900) & (uint)(1 << j)) >> j;
                    if (mask == 1)
                    {
                        result[i] = int.Parse($"7{j}00", CultureInfo.InvariantCulture);
                        intI4900 &= ~(1 << j);
                        break;
                    }
                }
            }

            if (result[0] == 0 && result[1] == 0)
                throw new ArgumentException("Invalid I4900 value");

            return result;
        }
    }
}