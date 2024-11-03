using Microsoft.OpenApi.Models;
using DataExtractor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Patient Data Processor API", Version = "v1" });
});

// Register the DataProcessorService
builder.Services.AddScoped<IDataProcessorService, DataProcessorService>();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactNative",
        builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient Data Processor API v1"));
}

app.UseCors("AllowReactNative");
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();