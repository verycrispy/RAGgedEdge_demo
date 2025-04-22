using System.Text;

namespace WikiAssistent;
public class WikiAssistant
{
    public static string BuildChatPrompt()
    {
        var sb = new StringBuilder();

        sb.AppendLine($"You are a software developer who helps collegues with information about the software development");
        //sb.AppendLine($"Your demeanor is {DemoConfig.Instance.Demeanor}.");
        sb.AppendLine($"Your recommendations are based on the similarity score included in the results returned from a vector search against a knowledge database.");
        sb.AppendLine($"Don't comment on other software development topics than the knowledge returned by the database.");
        sb.AppendLine($"Don't include information returned by the database that don't fit the user's question.");
        //sb.AppendLine($"This is critical: If the database results do not include a similarity score, then apologize for the database having no matches, and show no results at all, EVEN IF some of the results match the user's query.");
        //sb.AppendLine($"Only include the following details of each article: title, subject, description, {DemoConfig.Instance.IncludeDetails}.");

        return sb.ToString();
    }


    public static string BuildChatResponse(string question)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"The knowledge database returned these articles after being asked '{question}'.");
        sb.AppendLine($"Generate an answer directly to your collegue using these articles.");
        sb.AppendLine($"Phrase your response as though you are helping a colleague directly. Show code samples if they are included in the articles.");
        sb.AppendLine($"After your answer to the users question, list the articles in order of most similar to least similar. Show the Title and subject, but don't show the similarity score or content in this list");
        //sb.AppendLine($"This is critical: If the database results do not include a similarity score, then apologize for the database having no matches, and show no results at all, EVEN IF some of the results match the user's query.");
        //sb.AppendLine($"Limit your response to the information returned by the database; do not embellish with any other information.");

        return sb.ToString();
    }
}
