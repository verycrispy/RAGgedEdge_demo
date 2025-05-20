namespace BlazorDemoApp.Features.Ask;

public static class AskEndpoints
{
    public static void MapAskEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/askquestion", async (AskHandler handler, HttpContext context) =>
        {
            await handler.HandleAsync(context);
        });

        app.MapPost("/askquestion/stream", async (AskHandler handler, HttpContext context) =>
        {
            await handler.HandleStreamedAsync(context);
        });
    }
}