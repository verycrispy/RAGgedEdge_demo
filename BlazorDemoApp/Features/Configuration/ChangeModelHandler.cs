using LMStudioClient;
using Microsoft.Extensions.Options;

namespace BlazorDemoApp.wwwroot.Features.Configuration;

public class ChangeModelHandler
{
    private readonly LMStudioConfig _config;
    private readonly LmStudioClient _lmClient;

    public ChangeModelHandler(IOptions<LMStudioConfig> config)
    {
        _config = config.Value;
        _lmClient = new LmStudioClient(_config.Endpoint);
    }

    public async Task HandleAsync(HttpContext context)
    {
        context.Response.Headers.Append("Content-Type", "text/event-stream");
        //var messages = await CreateMessages(context);

        var answer = ""; // await _lmClient.GetChatCompletionsAsync(messages);

        await context.Response.WriteAsync(answer);
    }
}