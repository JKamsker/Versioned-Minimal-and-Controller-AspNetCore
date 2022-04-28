using Asp.Versioning.Conventions;
using Newtonsoft.Json.Linq;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(
        options =>
        {
            // reporting api versions will return the headers
            // "api-supported-versions" and "api-deprecated-versions"
            options.ReportApiVersions = true;
        })
        .AddMvc()
        .AddApiExplorer(
        options =>
        {
            // add the versioned api explorer, which also adds IApiVersionDescriptionProvider service
            // note: the specified format code will format the version as "'v'major[.minor][-status]"
            options.GroupNameFormat = "'v'VVV";

            // note: this option is only necessary when versioning by url segment. the SubstitutionFormat
            // can also be used to control the format of the API version in route templates
            options.SubstituteApiVersionInUrl = true;
        });

builder.Services
    .AddOpenApiDocument(settings =>
    {
        settings.Title = "Minimal API";
        settings.Version = "1";
        settings.ApiGroupNames = new[] { "v1" };
        settings.DocumentName = "v1";
    })
    .AddOpenApiDocument(settings =>
    {
        settings.Title = "Minimal API";
        settings.Version = "2";
        settings.ApiGroupNames = new[] { "v2" };

        settings.DocumentName = "v2";
    });

builder.Services.AddControllers();

var app = builder.Build();
app.UseRouting();

app.UseDeveloperExceptionPage();

app.MapControllers();
app.UseEndpoints(x =>
{
    x
        .DefineApi("People")
        .HasMapping(api =>
        {
            api.MapGet("/api/v{version:apiVersion}/people/{id:int}", (int id) => "Hello from a minimal api.")
              .Produces(response => response.Body<WeatherForecast>())
              .Produces(404)
              .HasApiVersion(1.0)
              .WithTags("people")
              ;
        });
});

app.UseOpenApi();

app.UseSwaggerUi3();
app.UseReDoc(c =>
{
    c.Path = "/api-docs/{documentName}";
});

app.Run();

public class WeatherForecast
{
    public DateTime Date { get; set; }

    public int TemperatureC { get; set; }

    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);

    public string? Summary { get; set; }
}