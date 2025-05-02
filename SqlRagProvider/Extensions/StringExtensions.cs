namespace SqlRagProvider.Extensions;
internal static class StingExtensions
{
    public static List<string> SplitString(this string str, int maxLength)
    {
        List<string> result = new List<string>();

        for (int i = 0; i < str.Length; i += maxLength)
        {
            int length = Math.Min(maxLength, str.Length - i);
            result.Add(str.Substring(i, length));
        }

        return result;
    }
}
