using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DasBot.App_Code
{
    public static class Distance
    {
        public static double ModifyedLevensteinDistance(string a, string b) => 2 * Accord.Math.Distance.Levenshtein(a, b) / (a.Length + b.Length);
    }
}
