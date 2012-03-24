using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace HIT_JWC_Helper
{
    /// <summary>
    /// Not finish yet.
    /// </summary>
    class ClassSelectHelper
    {
        public void fetchClassInfo(string input)
        {
            MatchCollection general = Regex.Matches(input,
                "<div[^>]+>[^<]+</div>",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            MatchCollection time_and_location = Regex.Matches(input,
                "<div[^>]+>[^<]+<br>[^>]+</div>",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);
            MatchCollection other = Regex.Matches(input,
                "<div[^>]+>[^<]+<br>[^>]+</div>",
                RegexOptions.Multiline | RegexOptions.IgnoreCase);

        }
    }
}
