using Microsoft.EntityFrameworkCore;
using Web_Engineering_Gruppenprojekt.Data;
using Web_Engineering_Gruppenprojekt.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

// Database provider is configurable: "Sqlite" (default, local dev) or "SqlServer" (Azure SQL).
var dbProvider = builder.Configuration["Database:Provider"] ?? "Sqlite";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
{
    if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
        options.UseSqlServer(connectionString, sql => sql.EnableRetryOnFailure(
            maxRetryCount: 5, maxRetryDelay: TimeSpan.FromSeconds(10), errorNumbersToAdd: null));
    else
        options.UseSqlite(connectionString);
});

builder.Services.AddHttpClient<IGeminiService, GeminiService>()
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(30));

if (builder.Environment.IsProduction() &&
    !string.IsNullOrEmpty(builder.Configuration.GetConnectionString("AzureBlobStorage")))
    builder.Services.AddScoped<IFileStorageService, AzureBlobStorageService>();
else
    builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();

var app = builder.Build();

// Initialize the database on startup.
// SQLite (local): apply EF migrations. SQL Server (Azure): create schema and seed
// data directly from the model via EnsureCreated (no SQL-Server-specific migrations needed).
// Retry a few times: on a cold Azure SQL instance the first connection attempt right after
// deployment/idle-out can fail before the database has finished waking up.
// Note: this only covers startup — App Service should also have "Always On" enabled so the
// app itself doesn't get unloaded after inactivity (not something that can be set from code).
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    const int maxAttempts = 3;
    for (var attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            if (dbProvider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
                db.Database.EnsureCreated();
            else
                db.Database.Migrate();
            break;
        }
        catch when (attempt < maxAttempts)
        {
            Thread.Sleep(TimeSpan.FromSeconds(5));
        }
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthorization();
app.MapStaticAssets();

app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
