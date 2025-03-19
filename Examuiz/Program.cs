using AI_Layer.AI_Models;
using AI_Layer.Interfaces;
using Examuiz.Exceptions;
using Examuiz.Settings;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using System.Net;
using System.Text;
using Microsoft.Data.SqlClient;


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


// Exception Handling Middleware
app.UseExceptionHandler(config =>
{
    config.Run(async context =>
    {
        var error = context.Features.Get<IExceptionHandlerFeature>();
        if (error != null)
        {
            var ex = error.Error;

            // Default to 500 Internal Server Error
            context.Response.StatusCode = (int)HttpStatusCode.BadRequest;

            if (ex.GetBaseException().GetType() == typeof(SqlException))
            {
                

                int ErrorCode = ((SqlException)ex.InnerException).Number;

                switch (ErrorCode)
                {
                    case 2627:  // Unique constraint error
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case 547:   // Constraint check violation
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;

                    case 2601:  // Duplicated key row error
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    default:
                        break;
                }
            }
            else
            {
                // Check for specific exceptions
                switch (ex)
                {
                    case UnauthorizedAccessException:
                        context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                        break;
                    case KeyNotFoundException:
                        context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case NotImplementedException:
                        context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                        break;
                }
            }



            context.Response.ContentType = "application/json";
            
            var errorResponse = new ErrorModel
            {
                StatusCode = context.Response.StatusCode,
                ErrorMessage = ex.InnerException == null ? ex.Message : ex.InnerException.Message
            };

            await context.Response.WriteAsync(errorResponse.ToString());
        }
    });
});




app.MapControllers();

app.Run();

#endregion