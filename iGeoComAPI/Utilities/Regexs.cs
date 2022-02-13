using System.Text.RegularExpressions;

namespace iGeoComAPI.Utilities
{
    public class Regexs
    {
        public Regex ExtractLagLong()
        {
            string regMatch = "([^|]*)";
            Regex reg = new Regex(regMatch);

            return reg;
        }
    }
}
