using System;
using System.Text.RegularExpressions;



namespace PixelWorldsServer2
{ 

    // Token: 0x02000264 RID: 612
    public static class PlayerNameChecker
    {
        // Token: 0x0600190D RID: 6413 RVA: 0x000A60A0 File Offset: 0x000A44A0
        public static NameValidity Validate(string playerName)
        {
            if (playerName == null)
            {
                return NameValidity.IsNull;
            }
            if (playerName.Length < PlayerNameChecker.minPlayerNameLength)
            {
                return NameValidity.TooShort;
            }
            if (playerName.Length > PlayerNameChecker.maxPlayerNameLength)
            {
                return NameValidity.TooLong;
            }
            playerName = playerName.ToUpper();
            if (!Regex.IsMatch(playerName, "^([][A-Z_^{}][][A-Z_0-9^{}-]+)$"))
            {
                return NameValidity.ContainsIllegalChars;
            }
            if (!playerName.Validate())
            {
                return NameValidity.Banned;
            }
            return NameValidity.Ok;
        }

        // Token: 0x0600190E RID: 6414 RVA: 0x000A6104 File Offset: 0x000A4504
        public static string GetErrorMessage(NameValidity nameValidity)
        {
            string result = string.Empty;
            switch (nameValidity)
            {
                case NameValidity.IsNull:
                    result = "The word you entered in empty.";
                    break;
                case NameValidity.TooShort:
                    result = "The word you entered is too short (min " + PlayerNameChecker.minPlayerNameLength + " characters).";
                    break;
                case NameValidity.TooLong:
                    result = "The word you entered is too long (max " + PlayerNameChecker.maxPlayerNameLength + " characters).";
                    break;
                case NameValidity.ContainsIllegalChars:
                    result = "The word you entered contains illegal characters.";
                    break;
                case NameValidity.Banned:
                    result = "The word you entered is banned.";
                    break;
            }
            return result;
        }

        // Token: 0x04001B62 RID: 7010
        private static readonly int minPlayerNameLength = 2;

        // Token: 0x04001B63 RID: 7011
        private static readonly int maxPlayerNameLength = 15;
    }
    public enum NameValidity : byte
    {
        // Token: 0x04001BA0 RID: 7072
        Ok,
        // Token: 0x04001BA1 RID: 7073
        IsNull,
        // Token: 0x04001BA2 RID: 7074
        TooShort,
        // Token: 0x04001BA3 RID: 7075
        TooLong,
        // Token: 0x04001BA4 RID: 7076
        ContainsIllegalChars,
        // Token: 0x04001BA5 RID: 7077
        Banned,
        // Token: 0x04001BA6 RID: 7078
        NotValid,
        // Token: 0x04001BA7 RID: 7079
        ReservedName,
        // Token: 0x04001BA8 RID: 7080
        MaxChangeReached
    }
}