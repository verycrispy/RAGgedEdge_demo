using BlazorDemoApp.Components;
using BlazorDemoApp.Features.Ask;
using BlazorDemoApp.Features.Configuration;
using LMStudioClient;
using SqlRagProvider;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddHttpClient();
builder.Services.Configure<LMStudioConfig>(builder.Configuration.GetSection("LMStudio"));
builder.Services.AddScoped<AskHandler>();
builder.Services.AddScoped<VectorizeDatabaseHandler>();
builder.Services.AddScoped<ChangeModelHandler>();
builder.Services.AddScoped<SqlDataVectorizer>();
builder.Services.AddScoped<Vectorizer>();
builder.Services.AddScoped<LmStudioClient>();
builder.Services.AddSingleton<ConfigService>();

WebApplication app = builder.Build();

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

app.MapAskEndpoints();
app.MapConfigurationEndpoints();

app.Run();