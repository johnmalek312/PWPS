namespace PixelWorldsServer2
{

    public static class PlayerIdNameHelper
    {
        // Token: 0x06001907 RID: 6407 RVA: 0x000A5FDE File Offset: 0x000A43DE
        public static string CombineIdAndName(string playerId, string name)
        {
            return playerId + PlayerIdNameHelper.splitChar + name;
        }

        // Token: 0x06001908 RID: 6408 RVA: 0x000A5FF1 File Offset: 0x000A43F1
        public static string[] SplitCombined(string combinedString)
        {
            return combinedString.Split(new char[]
            {
            PlayerIdNameHelper.splitChar
            });
        }

        // Token: 0x06001909 RID: 6409 RVA: 0x000A6007 File Offset: 0x000A4407
        public static string GetPlayerIdFromCombined(string combinedString)
        {
            return combinedString.Split(new char[]
            {
            PlayerIdNameHelper.splitChar
            })[0];
        }

        // Token: 0x0600190A RID: 6410 RVA: 0x000A6020 File Offset: 0x000A4420
        public static bool IsValidCombined(string combinedString)
        {
            int num = -1;
            for (int i = 0; i < combinedString.Length; i++)
            {
                if (combinedString[i] == PlayerIdNameHelper.splitChar)
                {
                    if (num != -1)
                    {
                        return false;
                    }
                    num = i;
                }
            }
            return num >= 1 && num != combinedString.Length - 1 && PlayerNameChecker.Validate(combinedString.Substring(num + 1)) == NameValidity.Ok;
        }

        // Token: 0x0600190B RID: 6411 RVA: 0x000A608F File Offset: 0x000A448F
        public static char GetSplitChar()
        {
            return PlayerIdNameHelper.splitChar;
        }

        // Token: 0x04001B61 RID: 7009
        private static readonly char splitChar = ';';
    }

}