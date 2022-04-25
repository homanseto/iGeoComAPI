using System.Text.RegularExpressions;

namespace iGeoComAPI.Utilities
{
    public static class Regexs
    {

        public static Regex ExtractInfo(string input)
        {
            Regex reg = new Regex(input);

            return reg;
        }

        public static Regex ExtractC_Floor()
        {
            string c_Floor = "地下及閣樓|地下及地庫|閣樓|平臺|地舖|平台|地面|地下|[\\d+]層|地庫|地鋪|[\\d+]樓|[\\d+]字樓|[一｜二｜三｜四｜五|六|七|八|九|十]樓|[一｜二｜三｜四｜五|六|七|八|九|十|上｜下|底|低|高]層|LG|UG|G+\\/F";
            Regex reg = new Regex(c_Floor);

            return reg;
        }

        public static string TrimAllAndAdjustSpace(string input)
        {
            // at least 2 whitespace
            string spaceAndAdjust = "\\s\\s+";
            var result = Regex.Replace(input, spaceAndAdjust, String.Empty);
            return result;
        }
    }
}
