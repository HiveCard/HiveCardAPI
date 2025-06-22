using HiveCardAPI.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(5000); // 👈 Required for EB to reverse proxy correctly
});
// Add services to the container.

var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "hivecard-db.cqxa6u088i5p.us-east-1.rds.amazonaws.com";
var dbName = Environment.GetEnvironmentVariable("DB_NAME") ?? "hivecard-db";
var dbUser = Environment.GetEnvironmentVariable("DB_USER") ?? "administrator";
var dbPass = Environment.GetEnvironmentVariable("DB_PASS") ?? "Pass4Database";

var connectionString = $"Host={dbHost};Port=5432;Database={dbName};Username={dbUser};Password={dbPass};SSL Mode=Require;Trust Server Certificate=true";

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
