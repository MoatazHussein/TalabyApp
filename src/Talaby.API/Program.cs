using Microsoft.Extensions.FileProviders;
using Serilog;
using Talaby.API.Extensions;
using Talaby.API.Middlewares;
using Talaby.Application.Extensions;
using Talaby.Infrastructure.Extensions;
using Talaby.Infrastructure.Seeders;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddPresentation();
builder.Services.AddApplication(builder.Configuration);
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder => builder
        //.AllowAnyOrigin()
        .WithOrigins("http://localhost:3000", "https://talaby.vercel.app")
        .AllowAnyHeader()
        .AllowAnyMethod());
});

var app = builder.Build();

var scope = app.Services.CreateScope();
var seeder = scope.ServiceProvider.GetRequiredService<ITalabySeeder>();
await seeder.Seed();


// Configure the HTTP request pipeline.
app.UseMiddleware<ErrorHandlingMiddleware>();
//app.UseMiddleware<RequestTimeLoggingMiddleware>();

app.UseSerilogRequestLogging();

Console.WriteLine($"Environment: {builder.Environment.EnvironmentName}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("v1/swagger.json", "API V1.0");
    });
}

app.UseStaticFiles(new StaticFileOptions()

{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Storage")),
    RequestPath = new PathString("/Storage")
});


app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MapGet("/", () => "Server is running....");

app.Run();



