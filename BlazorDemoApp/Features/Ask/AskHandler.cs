using LMStudioClient;
using LMStudioClient.Model;

public class AskHandler
{
    private readonly LmStudioClient _lmClient;
    private readonly Vectorizer _vectorizer;

    public AskHandler(LmStudioClient lmStudioClient, Vectorizer vectorizer)
    {
        _lmClient = lmStudioClient;
        _vectorizer = vectorizer;
    }

    public async Task HandleAsync(HttpContext context)
    {
        context.Response.Headers.Append("Content-Type", "text/event-stream");
        string question = await ReadQuestion(context);
        var messages = await CreateMessages(question);

        var answer = await _lmClient.GetChatCompletionsAsync(messages);

        await context.Response.WriteAsync(answer);
    }

    private static async Task<string> ReadQuestion(HttpContext context)
    {
        using var reader = new StreamReader(context.Request.Body);
        var question = await reader.ReadToEndAsync();
        return question;
    }

    private async Task<Message[]> CreateMessages(string question)
    {      
        var userQuestion = new Message { Content = question, Role = "user" };

        return [userQuestion];
    }
}
