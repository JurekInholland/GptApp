using Api;
using Domain;
using Models;
using Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpClient();
builder.Services.AddSignalR();

builder.Services.AddSingleton<ChatHub>();

builder.Services.AddScoped<IOpenAiService, OpenAiService>();
builder.Services.AddScoped<ISignalRService, SignalRService>();
builder.Services.AddSingleton<ITokenizerService, TokenizerService>();

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile("appsettings.Development.json", true, true)
    .AddEnvironmentVariables()
    .Build();

builder.Services.Configure<AppConfig>(cfg =>
{
    cfg.ApiKey = config.GetValue<string>("OpenAiApiKey") ?? string.Empty;
    cfg.OrganizationId = config.GetValue<string>("OpenAiOrganization") ?? string.Empty;
    cfg.PythonPath = config.GetValue<string>("PythonPath") ?? "/var/lib/python";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/api/signalr");

await app.RunAsync();
