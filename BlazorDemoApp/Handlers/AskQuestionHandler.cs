using System.Text;
using System.Text.Json;
using LMStudioClient;
using LMStudioClient.Model;
using Microsoft.Extensions.Options;
using SqlRagProvider;
using WikiAssistent;

public class AskQuestionHandler
{
    private readonly LMStudioConfig _config;
    private readonly LmStudioClient _lmClient;

    public AskQuestionHandler(IOptions<LMStudioConfig> config)
    {
        _config = config.Value;
        _lmClient = new LmStudioClient(_config.Endpoint);
    }

    public async Task HandleAsync(HttpContext context)
    {
        context.Response.Headers.Append("Content-Type", "text/event-stream");

        var messages = await CreateMessages(context);

        var answer = await _lmClient.GetChatCompletionsAsync(messages);

        await context.Response.WriteAsync(answer);
    }

    public async Task HandleStreamedAsync(HttpContext context)
    {
        context.Response.Headers.Append("Content-Type", "text/event-stream");
        using var reader = new StreamReader(context.Request.Body);
        var question = await reader.ReadToEndAsync();

        var messages = await CreateMessages(context);

        await foreach (var part in _lmClient.StreamChatCompletionsAsync(messages))
        {
            await context.Response.WriteAsync(part);
            await context.Response.Body.FlushAsync();
        }
    }

    private async Task<Message[]> CreateMessages(HttpContext context)
    {
        var messages = new List<Message>();
        using var reader = new StreamReader(context.Request.Body);
        var question = await reader.ReadToEndAsync();

        var vectors = await Vectorizer.VectorizeQuestion(question);
        var results = await SqlRagDataFetcher.GetDatabaseResults(vectors);

        var sb = new StringBuilder();
        sb.Append(WikiAssistant.BuildChatResponse(question));
        sb.AppendLine();

        if (results.Length == 0)
        {
            sb.AppendLine($"The database has no recommendations.");
        }
        else
        {
            sb.AppendLine($"The database results are:");
            sb.AppendLine();

            foreach (var result in results)
            {
                sb.AppendLine(JsonSerializer.Serialize(result));
                sb.AppendLine();
            }
        }
        var userMessagePrompt = sb.ToString();

        var promptSetup = new Message { Content = WikiAssistant.BuildChatPrompt(), Role = "system" };
        var userQuestion = new Message { Content = question, Role = "user" };
        var questionPrompt = new Message { Content = userMessagePrompt, Role = "system" };
        messages.Add(questionPrompt);
        messages.Add(userQuestion);
        messages.Add(questionPrompt);

        return messages.ToArray();
    } 
}
