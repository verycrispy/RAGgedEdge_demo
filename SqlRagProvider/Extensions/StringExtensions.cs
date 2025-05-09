namespace SqlRagProvider.Extensions;
internal static class StingExtensions
{
    public static List<string> SplitString(this string text, int maxLength)
    {
        if (text.Length <= maxLength) return [text];

        List<string> result = new List<string>();

        for (int i = 0; i < text.Length; i += maxLength)
        {
            int length = Math.Min(maxLength, text.Length - i);
            result.Add(text.Substring(i, length));
        }

        return [result[0]];
    }
}
