namespace HalloDoc.Common.Utility
{
    public static class CommonHelper
    {
        public static string ToTitleCase(string input)
        {
            if (string.IsNullOrEmpty(input)) return input;
            return System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input.ToLower());
        }
        // Add more helpers as needed
    }
} 