using CarPark.Application.Interfaces;
using CarPark.Application.Services;
using CarPark.Infrastructure;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// preserve PascalCase property names to match spec exactly
builder.Services.AddControllers().AddJsonOptions(opts =>
{
    opts.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Db connection (read from appsettings)
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<CarParkDbContext>(options =>
    options.UseNpgsql(conn));

// DI
builder.Services.AddScoped<IParkingService, ParkingService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Apply migrations at startup (easy dev experience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CarParkDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();
app.Run();
