using LogMonitorService;
using Serilog;

Console.WriteLine("Log Monitor Service started");

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.ConfigureLogging((hostContext, loggingBuilder) =>
{
    loggingBuilder.ClearProviders();
});
builder.Host.UseSerilog((ctx, lc) => lc
    .Enrich.FromLogContext()
    .ReadFrom.Configuration(ctx.Configuration));

// Configure Startup 
var startup = new Startup(builder.Configuration);

// Add services
startup.ConfigureServices(builder.Services);

// Configure HTTP request pipeline
var app = builder.Build();
startup.Configure(app, app.Environment); 

// Start
app.Run();