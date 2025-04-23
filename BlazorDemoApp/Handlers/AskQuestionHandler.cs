using LMStudioClient;
using Microsoft.Extensions.Options;
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

        using var reader = new StreamReader(context.Request.Body);
        var question = await reader.ReadToEndAsync();

        var promptSetup = new Message { Content = WikiAssistant.BuildChatPrompt(), Role = "system" };
        var userQuestion = new Message { Content = question, Role = "user" };
        var questionPrompt = new Message { Content = WikiAssistant.BuildChatResponse(question), Role = "system" };

        var answer = await _lmClient.GetChatCompletionsAsync(promptSetup, userQuestion, questionPrompt);

        await context.Response.WriteAsync(answer);
    }

    public async Task HandleStreamedAsync(HttpContext context)
    {
        context.Response.Headers.Append("Content-Type", "text/event-stream");

        using var reader = new StreamReader(context.Request.Body);
        var question = await reader.ReadToEndAsync();

        var promptSetup = new Message { Content = WikiAssistant.BuildChatPrompt(), Role = "system" };
        var userQuestion = new Message { Content = question, Role = "user" };
        var questionPrompt = new Message { Content = WikiAssistant.BuildChatResponse(question), Role = "system" };

        await foreach (var part in _lmClient.StreamChatCompletionsAsync(promptSetup, userQuestion, questionPrompt))
        {
            await context.Response.WriteAsync(part);
            await context.Response.Body.FlushAsync();
        }
    }
}
