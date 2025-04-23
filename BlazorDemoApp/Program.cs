using BlazorDemoApp.Components;
using LMStudioClient;
using Microsoft.Extensions.Options;
using WikiAssistent;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.Configure<LMStudioConfig>(builder.Configuration.GetSection("LMStudio"));
builder.Services.AddScoped<AskQuestionHandler>();

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

app.MapPost("/askquestion", async (AskQuestionHandler handler, HttpContext context) =>
{
    await handler.HandleAsync(context);
});

app.MapPost("/askquestion/stream", async (AskQuestionHandler handler, HttpContext context) =>
{
    await handler.HandleStreamedAsync(context);
});

app.Run();