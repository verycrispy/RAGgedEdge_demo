namespace BlazorDemoApp.wwwroot.Features.Configuration;

public static class ConfigurationEndpoints
{

    public static void MapConfigurationEndpoints(this WebApplication app)
    {

        app.MapPost("/config/vectorize-db", async (VectorizeDatabaseHandler handler, HttpContext context) =>
        {
            await handler.HandleAsync(context);
        });
    }

}