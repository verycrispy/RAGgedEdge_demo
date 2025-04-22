using BlazorDemoApp.Components;
using LMStudioClient;
using Microsoft.Extensions.Options;
using WikiAssistent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.Configure<LMStudioConfig>(
    builder.Configuration.GetSection("LMStudio"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MapPost("/askquestion", async (HttpContext context) =>
{
    context.Response.Headers.Add("Content-Type", "text/event-stream");

    using var reader = new StreamReader(context.Request.Body);
    var question = await reader.ReadToEndAsync();


    var promptSetup = new Message() { Content = WikiAssistant.BuildChatPrompt(), Role = "system" };
    var userQuestion = new Message() { Content = question, Role = "user" };
    var questionPrompt = new Message() { Content = WikiAssistant.BuildChatResponse(question), Role = "system" };

    var lmStudioConfig = context.RequestServices.GetRequiredService<IOptions<LMStudioConfig>>().Value;
    LmStudioClient lmStudioClient = new(lmStudioConfig.Endpoint);
    var answer = await lmStudioClient.GetChatCompletionsAsync(promptSetup, userQuestion, questionPrompt);

    var words = answer.Split(' ');
    // Simulate streaming a response from LM Studio
    foreach (var word in words)
    {
        await context.Response.WriteAsync(word + " ");
        await context.Response.Body.FlushAsync();
    }
});

app.Run();