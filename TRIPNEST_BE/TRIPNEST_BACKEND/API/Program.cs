using API.Config;
using API.Hubs;
using API.Services;
using APPLICATION.Interfaces.Notification;
using APPLICATION.Interfaces.Property;
using APPLICATION.Mapping;
using APPLICATION.Services;
using APPLICATION.Settings;
using INFRASTRUCTURE.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add DB Context
builder.Services.AddTripestDbContext(builder.Configuration);

// Add application services and repositories
builder.Services.AddAppAndReposervice(builder.Configuration);

// Add JWT Authentication
builder.Services.AddJwtAuthentication(builder.Configuration);

// Config Singal R
builder.Services.AddSignalR();
builder.Services.AddSingleton<INotificationSender, SignalRNotificationSender>();
builder.Services.AddSingleton<IUserIdProvider, NameBasedUserIdProvider>();

builder.Services.AddControllers();

// register LLM client using extension (reads config)
builder.Services.AddLocalLlmClient(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerAndCors(builder.Configuration);
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<UserMappingProfile>(); }, typeof(UserMappingProfile));
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<PropertiesProfile>(); }, typeof(PropertiesProfile));
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<MessageProfile>(); }, typeof(MessageProfile));
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<ItineraryProfile>(); }, typeof(ItineraryProfile));
builder.Services.AddAutoMapper(cfg => { cfg.AddProfile<NotificationProfile>(); }, typeof(NotificationProfile));

// Add URI LLM
builder.Services.AddHttpClient("LocalLLM", client =>
{
    client.BaseAddress = new Uri("http://localhost:8000/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

// Embedding
builder.Services.AddHttpClient(); // for OpenAI provider
builder.Services.AddSingleton<APPLICATION.Interfaces.Embedding.IEmbeddingProvider>(sp =>
{
    var config = sp.GetRequiredService<IConfiguration>();
    // choose provider by config
    var provider = config["Embedding:Provider"] ?? "openai"; // "local" or "openai"
    if (provider.Equals("local", StringComparison.OrdinalIgnoreCase))
    {
        return new APPLICATION.Embedding.LocalEmbeddingProvider(dim: 1536);
    }
    else
    {
        // OpenAi: register OpenAiEmbeddingProvider with an HttpClient
        var http = sp.GetRequiredService<IHttpClientFactory>().CreateClient();
        var logger = sp.GetService<ILogger< APPLICATION.Embedding.OpenAiEmbeddingProvider >>();
        return new APPLICATION.Embedding.OpenAiEmbeddingProvider(http, config, logger);
    }
});

// Authorization policy for worker
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("InternalWorker", policy =>
    {
        policy.RequireRole("Worker", "Admin");
    });
});

// Vector service
builder.Services.AddHttpClient("vectorService", client =>
{
    client.Timeout = TimeSpan.FromSeconds(30);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseRouting();
app.Use(async (ctx, next) =>
{
    var origin = ctx.Request.Headers["Origin"].ToString();
    // Log để kiểm tra
    Console.WriteLine($"[DEBUG CORS] Method={ctx.Request.Method} Path={ctx.Request.Path} Origin={origin}");

    if (string.Equals(ctx.Request.Method, "OPTIONS", StringComparison.OrdinalIgnoreCase))
    {
        // Trả preflight thủ công
        ctx.Response.StatusCode = 204; // No Content
        if (!string.IsNullOrEmpty(origin))
        {
            ctx.Response.Headers["Access-Control-Allow-Origin"] = origin;
        }
        else
        {
            ctx.Response.Headers["Access-Control-Allow-Origin"] = "*";
        }
        ctx.Response.Headers["Access-Control-Allow-Methods"] = "GET, POST, PUT, DELETE, OPTIONS";
        ctx.Response.Headers["Access-Control-Allow-Headers"] = "Content-Type, Authorization, X-Requested-With";
        ctx.Response.Headers["Access-Control-Allow-Credentials"] = "true";
        await ctx.Response.CompleteAsync();
        return;
    }

    await next();
});

// CORS policy
//app.UseCors("AllowLocalhostDev");
app.UseCors();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<ChatHub>("/hubs/chat");
app.MapHub<NotificationsHub>("/hubs/notifications");

app.MapControllers();

app.Run();
