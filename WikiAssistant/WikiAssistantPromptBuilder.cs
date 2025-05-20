using System.Text;

namespace WikiAssistant;
public class WikiAssistantPromptBuilder
{
    public static string BuildChatSystemPrompt()
    {
        
        var sb = new StringBuilder();
        sb.AppendLine($"You are a software developer who helps collegues with information about the software development");
        sb.AppendLine($"Your recommendations are based on the similarity score included in the results returned from a vector search against a knowledge database.");
        sb.AppendLine($"Don't comment on other software development topics than the knowledge returned by the database.");
        sb.AppendLine($"Don't include information returned by the database that don't fit the user's question.");

        return sb.ToString();
    }


    public static string BuildQuestionPrompt(string question)
    {
        var sb = new StringBuilder();

        sb.AppendLine($"The knowledge database returned these articles after being asked '{question}'.");
        sb.AppendLine($"Generate an answer directly to your collegue using these articles.");
        sb.AppendLine($"Phrase your response as though you are helping a colleague directly. Show code samples if they are included in the articles.");
        sb.AppendLine($"After your answer to the users question, list the articles in order of most similar to least similar. Show the Title and subject, but don't show the similarity score or content in this list");
        
        return sb.ToString();
    }
}
