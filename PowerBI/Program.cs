using PowerBILab01.Options;
using PowerBILab01.Services;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton<IConfigValidator, ConfigValidator>();
builder.Services.AddSingleton<IAzureADAccessService, AzureADAccessService>();
builder.Services.AddSingleton<IPowerBIEmbeddedService, PowerBIEmbeddedService>();


builder.Services.AddControllersWithViews();

var configuration = builder.Configuration;

// Loading appsettings.json in C# Model classes
builder.Services.AddOptions();
builder.Services.Configure<AzureAdOptions>(configuration.GetSection("AzureAd"))
        .Configure<PowerBIOptions>(configuration.GetSection("PowerBI"))
        .Configure<List<ReportOptions>>(configuration.GetSection("Reports"));

// Register the Swagger services
builder.Services.AddSwaggerDocument();

var app = builder.Build();

var localDevAllowSpecificOrigins = "_localDevOrigins";
// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();

    //setup localhost DEV CORS

    builder.Services.AddCors(options =>
    {
        options.AddPolicy(name: localDevAllowSpecificOrigins,
        builder =>
        {
            builder.WithOrigins("https://localhost:7162");
        });
    });
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(localDevAllowSpecificOrigins);

// Register the Swagger generator and the Swagger UI middlewares
app.UseOpenApi();
app.UseSwaggerUi3();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller}/{action=Index}/{id?}");

app.MapFallbackToFile("index.html");;

app.Run();
