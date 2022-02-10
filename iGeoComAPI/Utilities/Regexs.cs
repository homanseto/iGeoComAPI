using System.Text.RegularExpressions;

namespace iGeoComAPI.Utilities
{
    public static class Regexs
    {
        public static Regex ExtractLagLong()
        {
            string regMatch = "([^|]*)";
            Regex reg = new Regex(regMatch);

            return reg;
        }
    }
}
