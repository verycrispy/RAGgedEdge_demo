using LMStudioClient;

namespace BlazorDemoApp.Features.Configuration;

public class ChangeModelHandler
{
    private readonly ConfigService _configService;

    public ChangeModelHandler(ConfigService configService)
    {
        _configService = configService;
    }

    public async Task HandleAsync(HttpContext context)
    {
        using var reader = new StreamReader(context.Request.Body);
        var model = await reader.ReadToEndAsync();
        _configService.UpdateModel(model);

        await context.Response.WriteAsync($"Model changed to: {model}");
    }
}