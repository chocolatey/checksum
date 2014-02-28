namespace checksum
{
    public static class StringExtensions
    {
        public static string ToLowerSafe(this string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return input;

            return input.ToLower();
        }
    }
}