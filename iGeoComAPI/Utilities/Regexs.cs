using System.Text.RegularExpressions;

namespace iGeoComAPI.Utilities
{
    public static class Regexs
    {

        public static Regex ExtractInfo(string info)
        {
            Regex reg = new Regex(info);

            return reg;
        }

        public static Regex ExtractC_Floor()
        {
            string c_Floor = @"(?<floor>地舖|平台|地面|地下|G+\/F|[\d+]層|地庫|地鋪|[\d+]樓
                             |[一｜二｜三｜四｜五|六|七|八|九|十]樓|
                             [一｜二｜三｜四｜五|六|七|八|九|十|上｜下|底|低|高]層|地下及[閣樓]|LG)";
            Regex reg = new Regex(c_Floor);

            return reg;
        }
    }
}
