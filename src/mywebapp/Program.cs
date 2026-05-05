using Microsoft.EntityFrameworkCore;
using mywebapp.Data;
using mywebapp.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSystemd();

builder.Configuration.AddJsonFile("/etc/mywebapp/config.json", optional: true, reloadOnChange: true);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? "Host=127.0.0.1;Database=mywebappdb;Username=postgres;Password=postgres";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.WebHost.ConfigureKestrel(options =>
{
    options.UseSystemd();
});

var app = builder.Build();

app.MapSystemEndpoints();
app.MapNotesEndpoints();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.Run();