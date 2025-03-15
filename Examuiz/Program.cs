using AI_Layer.AI_Models;
using AI_Layer.Interfaces;
using Examuiz.Settings;
using Microsoft.Extensions.Options;
using System.Text;
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});



#region Dependency Injection
builder.Services.AddHttpClient();
builder.Services.Configure<GeminiSettings>(builder.Configuration.GetSection("GeminiApi"));
builder.Services.AddSingleton<IGenerativeAI>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<GeminiSettings>>().Value;
    return new Gemini(settings.ApiKey);
});

#endregion

#region App initialization


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion