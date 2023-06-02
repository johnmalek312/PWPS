using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PixelWorldsServer2
{
    public static class ProfanityFilter
    {
        // Token: 0x06001915 RID: 6421 RVA: 0x000A6251 File Offset: 0x000A4651
        public static void LoadWordLists(string defaultReplacement, Dictionary<string, int> partialWordsList = null, Dictionary<string, int> fullWordsList = null, string[] replacementWords = null)
        {
            ProfanityFilter.partialWordsList = partialWordsList;
            ProfanityFilter.fullWordsList = fullWordsList;
            ProfanityFilter.defaultReplacement = defaultReplacement;
            ProfanityFilter.replacementWords = replacementWords;
        }

        // Token: 0x06001916 RID: 6422 RVA: 0x000A626B File Offset: 0x000A466B
        public static void LoadWordLists(string[] partialWordsList, string[] fullWordsList, string defaultReplacement)
        {
        }

        // Token: 0x06001917 RID: 6423 RVA: 0x000A6270 File Offset: 0x000A4670
        public static string Censor(this string str)
        {
            if (ProfanityFilter.partialWordsList == null || ProfanityFilter.fullWordsList == null)
            {
                return string.Empty;
            }
            string[] array = new string[ProfanityFilter.partialWordsList.Keys.Count];
            ProfanityFilter.partialWordsList.Keys.CopyTo(array, 0);
            string[] array2 = new string[ProfanityFilter.fullWordsList.Keys.Count];
            ProfanityFilter.fullWordsList.Keys.CopyTo(array2, 0);
            MatchCollection matchCollection = Regex.Matches(str, string.Join("|", array), RegexOptions.IgnoreCase);
            MatchCollection matchCollection2 = Regex.Matches(str, "\\b" + string.Join("\\b|\\b", array2) + "\\b", RegexOptions.IgnoreCase);
            for (int num = matchCollection.Count; num != 0; num--)
            {
                str = Regex.Replace(str, matchCollection[num - 1].Value, ProfanityFilter.replacementWords[ProfanityFilter.partialWordsList[matchCollection[num - 1].Value.ToLower()]]);
            }
            for (int num2 = matchCollection2.Count; num2 != 0; num2--)
            {
                str = Regex.Replace(str, matchCollection2[num2 - 1].Value, ProfanityFilter.replacementWords[ProfanityFilter.fullWordsList[matchCollection2[num2 - 1].Value.ToLower()]]);
            }
            return str;
        }

        // Token: 0x06001918 RID: 6424 RVA: 0x000A63C4 File Offset: 0x000A47C4
        public static bool Validate(this string input)
        {
            if (ProfanityFilter.partialWordsList == null || ProfanityFilter.fullWordsList == null)
            {
                return true;
            }
            string[] array = new string[ProfanityFilter.partialWordsList.Keys.Count];
            ProfanityFilter.partialWordsList.Keys.CopyTo(array, 0);
            string[] array2 = new string[ProfanityFilter.fullWordsList.Keys.Count];
            ProfanityFilter.fullWordsList.Keys.CopyTo(array2, 0);
            return !Regex.IsMatch(input, string.Join("|", array), RegexOptions.IgnoreCase) && !Regex.IsMatch(input, "\\b" + string.Join("\\b|\\b", array2) + "\\b", RegexOptions.IgnoreCase);
        }

        // Token: 0x04001B64 RID: 7012
        private static string defaultReplacement = string.Empty;

        // Token: 0x04001B65 RID: 7013
        private static Dictionary<string, int> partialWordsList;

        // Token: 0x04001B66 RID: 7014
        private static Dictionary<string, int> fullWordsList;

        // Token: 0x04001B67 RID: 7015
        private static string[] replacementWords;

        // Token: 0x04001B68 RID: 7016
        public const bool USE_REPLACEMENTS = true;
    }

}
